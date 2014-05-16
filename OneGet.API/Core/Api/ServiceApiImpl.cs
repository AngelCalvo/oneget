//
//  Copyright (c) Microsoft Corporation. All rights reserved.
//  Licensed under the Apache License, Version 2.0 (the "License");
//  you may not use this file except in compliance with the License.
//  You may obtain a copy of the License at
//  http://www.apache.org/licenses/LICENSE-2.0
//
//  Unless required by applicable law or agreed to in writing, software
//  distributed under the License is distributed on an "AS IS" BASIS,
//  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//  See the License for the specific language governing permissions and
//  limitations under the License.
//

namespace Microsoft.OneGet.Core.Api {
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Runtime.Remoting;
    using System.Threading;
    using System.Threading.Tasks;
    using Extensions;
    using Platform;
    using Tasks;
#if FRAMEWORKv40
    using Compression;
#endif
    using Callback = System.Func<string, System.Collections.Generic.IEnumerable<object>, object>;

    /// <summary>
    ///     PROTOTYPE -- This API is nowhere near what the actual provider API will resemble
    ///     The Provider API is designed for optional use by 'enlightened' package providers to
    ///     access (and extend) low-level feaures like
    ///     - HTTP/FTP/*** Downloads
    ///     - Use of the DTF APIs (ie, for MSIs)
    ///     - File Manipulations: Zip, Unzip, file copy, etc.
    ///     - Creating Shell links, updating environment variables, etc.
    /// </summary>
    public class ServiceApiImpl : MarshalByRefObject {

        private void Error(string category, string message, params object[] args) {
            Event<GetHostDelegate>.Raise().Lookup<Error>()(category, message, args);
        }
        private void Warning(string category, string message, params object[] args) {
            Event<GetHostDelegate>.Raise().Lookup<Warning>()(category, message, args);
        }

        [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Still in development!")]
        private void Message(string category, string message, params object[] args) {
            Event<GetHostDelegate>.Raise().Lookup<Message>()(category, message, args);
        }
        private void Verbose(string category, string message, params object[] args) {
            Event<GetHostDelegate>.Raise().Lookup<Verbose>()(category, message, args);
        }

        private void Progress(int activityId, string activity, int progress, string message, params object[] args) {
            Event<GetHostDelegate>.Raise().Lookup<Progress>()(activityId, activity, progress,message, args);
        }

        private void ProgressComplete(int activityId, string activity, string message, params object[] args) {
            Event<GetHostDelegate>.Raise().Lookup<ProgressComplete>()(activityId, activity, message, args);
        }

        internal ServiceApiImpl() {
        }

        #region implement service-api-callbacks
[Implementation]
        public string DownloadFile(string remoteLocation, string localLocation) {
            if (localLocation == null) {
                localLocation = Path.Combine( FilesystemExtensions.TempPath, "file.bin");
            }
            localLocation = Path.GetFullPath(localLocation);
            if (Directory.Exists(localLocation)) {
                localLocation = Path.Combine(localLocation, "file.bin");
            }
            
            var folder = Path.GetDirectoryName(localLocation);
            if (!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            if (File.Exists(localLocation)) {
                localLocation.TryHardToDelete();
            }
            
            Verbose("Downloading", "'{0}' to '{1}'", remoteLocation , localLocation);
            WebClient webClient = new WebClient();

            // Apparently, places like Codeplex know to let this thru!
            webClient.Headers.Add("user-agent", "chocolatey command line");

            ManualResetEvent done = new ManualResetEvent(false);
            
            webClient.DownloadFileCompleted += (sender, args) => {
                ProgressComplete(2, "Downloading {0}".format(remoteLocation), "Completed");
                if (args.Cancelled || args.Error != null) {
                    localLocation = null;
                }
                done.Set();
            };
            webClient.DownloadProgressChanged += (sender, args) => {
                long percent = (args.BytesReceived*100)/args.TotalBytesToReceive;
                Progress(2, "Downloading {0}".format(remoteLocation),(int)percent, "Downloading {0} of {1} bytes", args.BytesReceived, args.TotalBytesToReceive );
            };
            webClient.DownloadFileAsync(new Uri( remoteLocation) , localLocation );
            done.WaitOne();
            if (File.Exists(localLocation)) {
                return localLocation;
            }
            return null;
        }
        [Implementation]
        public void AddPinnedItemToTaskbar(string item) {
        }
        [Implementation]
        public void RemovePinnedItemFromTaskbar(string item) { 
            //comment
        }
        [Implementation]
        public bool CreateShortcutLink(string linkPath, string targetPath, string description, string workingDirectory, string arguments) {
            if (File.Exists(linkPath)) {
                Verbose("Creating Shortcut", "'{0}' => '{1}'", linkPath, targetPath );
                ShellLink.CreateShortcut(linkPath, targetPath, description, workingDirectory, arguments);
                return true;
            } 
            Error("Unable to create shortcut", "target '{0}' does not exist", targetPath );
            return false;
        }

        [Implementation]
        public IEnumerable<string> UnzipFile(string zipFile, string folder) {
            return UnzipFileIncremental(zipFile, folder).ToArray();
        }

        [Implementation]
        public IEnumerable<string> UnzipFileIncremental(string zipFile, string folder) {
            Directory.CreateDirectory(folder);
            if (File.Exists(zipFile)) {
                using (var zipStream = File.Open(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    if (zipStream.ReadByte() != 0x50 || zipStream.ReadByte() != 0x4b) {
                        // not a zip file. 
                        throw new Exception("File '{0}' is not a zip file.".format(zipFile));
                    }
                    zipStream.Seek(0, SeekOrigin.Begin);
                    ZipArchive za;
                    try {
                        za = new ZipArchive(zipStream, ZipArchiveMode.Read);
                    } catch (Exception e) {
                        e.Dump();
                        throw;
                    }
#if IF_ONLY_IT_WAS_THREADSAFE
                    var items = za.Entries.Select(zipEntry => {
                        var outputFilename = Path.Combine(folder, zipEntry.Name);
                        var inputStream = zipEntry.Open();
                        try {
                            Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));
                            var outputStream = File.Create(outputFilename);
                            return inputStream.CopyToAsync(outputStream).ContinueWith((a) => {outputStream.Close(); inputStream.Close();}).ContinueWith((a) => outputFilename);
                                    
                        } catch (Exception e) {
                            e.Dump();
                        }
                        
                        return null;
                    }).WhereNotNull().ToArray();

                    foreach (var item in items) {
                        yield return item.Result;
                    }
#endif
                    var total = za.Entries.Count;

                    var index = 0;
                    foreach (var zipEntry in za.Entries) {
                        index++;
                        var outputFilename = Path.Combine(folder, zipEntry.Name);
                        using (var stream = zipEntry.Open() ) {
                            Progress(2, "Unzipping", (index * 100) / total, "Processing {0} of {1} -- '{2}'", index, total, outputFilename);
                            try {
                                Directory.CreateDirectory(Path.GetDirectoryName(outputFilename));
                                using (var outputStream = File.Create(outputFilename)) {
                                    stream.CopyTo(outputStream,65536);
                                }
                            } catch (Exception e) {
                                e.Dump();
                                continue;
                            }
                        }
                        yield return outputFilename;
                    } 
                }
                ProgressComplete(2, "Unzipping", "Completed");
                yield break;
            }
            throw new Exception("Zipfile '{0}' missing.".format(zipFile));
        }
        [Implementation]
        public void AddFileAssociation() {
        }
        [Implementation]
        public void RemoveFileAssociation() {
        }
        [Implementation]
        public void AddExplorerMenuItem() {
        }
        [Implementation]
        public void RemoveExplorerMenuItem() {
        }
        [Implementation]
        public bool SetEnvironmentVariable(string variable, string value, string context) {
            if (string.IsNullOrEmpty(value)) {
                return RemoveEnvironmentVariable(variable, context);
            }
            context = context ?? string.Empty;
            switch (context.ToLowerInvariant()) {
                case "machine":
                    if (!IsElevated()) {
                        Warning("SetEnvironmentVariable Failed", "Admin Elevation required to set variable '{0}' in machine context", variable);
                        return false;
                    }
                    Verbose("SetEnvironmentVariable (machine)", "'{0}' = '{1}'", variable , value); 
                    Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.Machine);
                    break;
                default:
                    Verbose("SetEnvironmentVariable (user)", "'{0}' = '{1}'", variable, value ); 
                    Environment.SetEnvironmentVariable(variable, value, EnvironmentVariableTarget.User);
                    break;
            }
            Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Process);
            return true;
        }
        [Implementation]
        public bool RemoveEnvironmentVariable(string variable, string context) {
            context = context ?? string.Empty;
            if (string.IsNullOrEmpty(variable)) {
                return false;
            }
            switch (context.ToLowerInvariant()) {
                case "user":
                    Verbose("RemoveEnvironmentVariable (user)", "'{0}'", variable ); 
                    Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.User);
                    break;
                case "machine":
                    if (!IsElevated()) {
                        Warning("RemoveEnvironmentVariable Failed", "Admin Elevation required to remove variable '{0}' from machine context", variable );
                        return false;
                    }
                    Verbose("RemoveEnvironmentVariable (machine)", "'{0}'", variable ); 
                    Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
                    break;
                default:
                    Verbose("RemoveEnvironmentVariable (all)", "'{0}'", variable ); 
                    Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.User);
                    Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Machine);
                    break;
            }
            Environment.SetEnvironmentVariable(variable, null, EnvironmentVariableTarget.Process);
            return true;
        }
        [Implementation]
        public void AddFolderToPath() {
        }
        [Implementation]
        public void RemoveFolderFromPath() {
        }
        [Implementation]
        public void InstallMSI() {
        }
        [Implementation]
        public void RemoveMSI() {
        }
        [Implementation]
        public void StartProcess() {
        }
        [Implementation]
        public void InstallVSIX() {
        }
        [Implementation]
        public void UninstallVSIX() {
        }
        [Implementation]
        public void InstallPowershellScript() {
        }
        [Implementation]
        public void UninstallPowershellScript() {
        }
        [Implementation]
        public void SearchForExecutable() {
        }
        [Implementation]
        public void GetUserBinFolder() {
        }
        [Implementation]
        public void GetSystemBinFolder() {
        }
        [Implementation]
        public bool CopyFile(string sourcePath, string destinationPath) {
            if (sourcePath == null) {
                throw new ArgumentNullException("sourcePath");
            }
            if (destinationPath == null) {
                throw new ArgumentNullException("destinationPath");
            }
            if (File.Exists(destinationPath)) {
                destinationPath.TryHardToDelete();
                if (File.Exists(destinationPath)) {
                    return false;
                }
            }
            File.Copy( sourcePath, destinationPath);
            return File.Exists(destinationPath);
        }
        [Implementation]
        public void CopyFolder() {
        }
        [Implementation]
        public void DeleteFolder(string folder) {
            if (string.IsNullOrEmpty(folder)) {
                return;
            }
            if (Directory.Exists(folder)) {
                folder.TryHardToDelete();
            }
        }
        [Implementation]
        public void DeleteFile(string filename) {
            if (string.IsNullOrEmpty(filename)) {
                return;
            }

            if (File.Exists(filename)) {
                filename.TryHardToDelete();
            }
        }
        [Implementation]
        public IEnumerable<string> GetConfigStrings(string name) {
            return null;
        }
        
        [Implementation]
        public string GetNuGetExePath() {
            var c = Event<GetHostDelegate>.Raise();
            return c == null ? null : Bootstrapper.GetNuGetExePath(this,c);
        }
        
        [Implementation]
        public string GetNuGetDllPath(){
            var c = Event<GetHostDelegate>.Raise();
            return c == null ? null : Bootstrapper.GetNuGetDllPath(this, c);
        }
        
        [Implementation]
        public string GetKnownFolder(string knownFolder){
            if (!string.IsNullOrEmpty(knownFolder)) {
                if (knownFolder.Equals("tmp", StringComparison.OrdinalIgnoreCase) || knownFolder.Equals("temp", StringComparison.OrdinalIgnoreCase)) {
                    return FilesystemExtensions.TempPath;
                }
                    KnownFolder folder;
                if (Enum.TryParse(knownFolder, true, out folder)) {
                    return KnownFolders.GetFolderPath(folder);
                }
            }
            return null;
        }
        
        [Implementation]
        public bool IsElevated() {
            Verbose("Current Process is", AdminPrivilege.IsElevated? "Elevated": "NOT Elevated");
            return AdminPrivilege.IsElevated;
        }
        
        [Implementation]
        public void Delete(string path){
            if (string.IsNullOrEmpty(path)) {
                return;
            }

            path.TryHardToDelete();
        }
        
        [Implementation]
        public void CreateFolder(string folder) {
            if (!Directory.Exists(folder)) {
                try {
                    Directory.CreateDirectory(folder);
                    Verbose("CreateFolder Success", folder);
                    return;
                } catch (Exception e ) {
                    Error( "CreateFolder Failed", "'{0}' -- {1}", folder, e.Message );
                    return;
                }
            }
            Verbose("CreateFolder -- Already Exists", folder);
        }
        // AFTER_CTP
        [Implementation]
        public void BeginTransaction() {
        }
        [Implementation]
        public void AbortTransaction() {
        }
        [Implementation]
        public void EndTransaction() {
        }
        [Implementation]
        public void GenerateUninstallScript() {
        }
        // end AFTER_CTP

        #endregion


        private InvokableDispatcher _dispatcher;

        internal Callback Invoke {
            get {
                return _dispatcher ?? (_dispatcher =
                    new InvokableDispatcher {
                        #region generate-call service-api-callbacks
                    (GetNuGetExePath)GetNuGetExePath,
                    (GetNuGetDllPath)GetNuGetDllPath,
                    (DownloadFile)DownloadFile,
                    (AddPinnedItemToTaskbar)AddPinnedItemToTaskbar,
                    (RemovePinnedItemFromTaskbar)RemovePinnedItemFromTaskbar,
                    (CreateShortcutLink)CreateShortcutLink,
                    (UnzipFileIncremental)UnzipFileIncremental,
                    (UnzipFile)UnzipFile,
                    (AddFileAssociation)AddFileAssociation,
                    (RemoveFileAssociation)RemoveFileAssociation,
                    (AddExplorerMenuItem)AddExplorerMenuItem,
                    (RemoveExplorerMenuItem)RemoveExplorerMenuItem,
                    (SetEnvironmentVariable)SetEnvironmentVariable,
                    (RemoveEnvironmentVariable)RemoveEnvironmentVariable,
                    (AddFolderToPath)AddFolderToPath,
                    (RemoveFolderFromPath)RemoveFolderFromPath,
                    (InstallMSI)InstallMSI,
                    (RemoveMSI)RemoveMSI,
                    (StartProcess)StartProcess,
                    (InstallVSIX)InstallVSIX,
                    (UninstallVSIX)UninstallVSIX,
                    (InstallPowershellScript)InstallPowershellScript,
                    (UninstallPowershellScript)UninstallPowershellScript,
                    (SearchForExecutable)SearchForExecutable,
                    (GetUserBinFolder)GetUserBinFolder,
                    (GetSystemBinFolder)GetSystemBinFolder,
                    (CopyFile)CopyFile,
                    (CopyFolder)CopyFolder,
                    (Delete)Delete,
                    (DeleteFolder)DeleteFolder,
                    (CreateFolder)CreateFolder,
                    (DeleteFile)DeleteFile,
                    (BeginTransaction)BeginTransaction,
                    (AbortTransaction)AbortTransaction,
                    (EndTransaction)EndTransaction,
                    (GenerateUninstallScript)GenerateUninstallScript,
                    (GetKnownFolder)GetKnownFolder,
                    (IsElevated)IsElevated,
                    #endregion
                    });

            }
        }
    }
    public class ImplementationAttribute : Attribute {
    }
}
