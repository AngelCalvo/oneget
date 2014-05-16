﻿//
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

namespace Microsoft.OneGet.Core.AppDomains {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using Extensions;

    internal partial class PluginDomain : IDisposable {
        private AppDomain _appDomain;
        private string _identity;

        private Proxy<PluginAssemblyResolver> _proxyResolver;
        private PluginAssemblyResolver _resolver;

        internal PluginDomain() :
            this(new AppDomainSetup {
                ApplicationBase = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                PrivateBinPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
            }) {
        }

        internal PluginDomain(AppDomainSetup appDomainSetup) {
            if (appDomainSetup == null) {
                throw new ArgumentNullException("appDomainSetup");
            }
            _identity = Guid.NewGuid().ToString();
            appDomainSetup.ApplicationName = appDomainSetup.ApplicationName ?? "PluginDomain" + _identity;

            _appDomain = AppDomain.CreateDomain(_identity, null, appDomainSetup);

            _resolver = new PluginAssemblyResolver();
            _resolver.AddPath(appDomainSetup.ApplicationBase);
            _resolver.AddPath(appDomainSetup.PrivateBinPath);
            AppDomain.CurrentDomain.AssemblyResolve += _resolver.Resolve;

            _proxyResolver = new Proxy<PluginAssemblyResolver>(this);
            ((PluginAssemblyResolver)_proxyResolver).AddPath(appDomainSetup.ApplicationBase);
            ((PluginAssemblyResolver)_proxyResolver).AddPath(appDomainSetup.PrivateBinPath);

            Invoke(resolver => {AppDomain.CurrentDomain.AssemblyResolve += resolver.Resolve;}, ((PluginAssemblyResolver)_proxyResolver));
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public virtual void Dispose(bool disposing) {
            if (disposing) {
                if (_proxyResolver != null) {
                    _proxyResolver.Dispose();
                    _proxyResolver = null;
                }

                _resolver = null;
                if (_appDomain != null) {
                    try {
                        AppDomain.Unload(_appDomain);
                    } catch (Exception e) {
                        e.Dump();
                    }
                }
                _appDomain = null;
            }
        }

        public Assembly LoadFrom(string path) {
            return ((PluginAssemblyResolver)_proxyResolver).LoadFrom(path);
        }

        public Assembly LoadFile(string path) {
            return ((PluginAssemblyResolver)_proxyResolver).LoadFile(path);
        }

        public Assembly LoadBinary(string path) {
            return ((PluginAssemblyResolver)_proxyResolver).LoadBinary(path);
        }

        public IList<Assembly> LoadFromWithReferences(string path) {
            return ((PluginAssemblyResolver)_proxyResolver).LoadFromWithReferences(path);
        }

        public IList<Assembly> LoadFileWithReferences(string path) {
            return ((PluginAssemblyResolver)_proxyResolver).LoadFileWithReferences(path);
        }

        public IList<Assembly> LoadBinaryWithReferences(string path) {
            return ((PluginAssemblyResolver)_proxyResolver).LoadBinaryWithReferences(path);
        }

        public static implicit operator AppDomain(PluginDomain domain) {
            if (domain == null) {
                throw new ArgumentNullException("domain");
            }

            return domain._appDomain;
        }
    }
}
