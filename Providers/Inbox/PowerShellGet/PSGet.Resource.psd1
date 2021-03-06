#########################################################################################
#
# Copyright (c) Microsoft Corporation. All rights reserved.
#
# Localized PSGet.Resource.psd1
#
#########################################################################################

ConvertFrom-StringData @'
###PSLOC
        InstallModulewhatIfMessage=Version '{1}' of module '{0}'
        UpdateModulewhatIfMessage=Version '__OLDVERSION__' of module '{0}', updating to version '{1}'
        PublishModulewhatIfMessage=Version '{0}' of module '{1}'
        MinimumVersionAndRequiredVersionCannotBeSpecifiedTogether=The parameters MinimumVersion and RequiredVersion cannot be used in the same command. Specify only one of these parameters in your command.
        RequiredVersionAllowedOnlyWithSingleModuleName=The RequiredVersion parameter is allowed only when a single module name is specified as the value of the Name parameter, without any wildcard characters.
        InstallModuleNeedsCurrentUserScopeParameterForNonAdminUser=Administrator rights are required to install modules in '{0}'. Log on to the computer with an account that has Administrator rights, and then try again, or install '{1}' by adding "-Scope CurrentUser" to your command. You can also try running the Windows PowerShell session with elevated rights (Run as Administrator).
        VersionParametersAreAllowedOnlyWithSingleModule=The RequiredVersion or MinimumVersion parameters are allowed only when a single module name is specified as the value of the Name parameter, without any wildcard characters. To install multiple modules with specific versions, pipe in multiple objects that are returned by the Find-Module cmdlet.
        PathIsNotADirectory=The specified path '{0}' is not a valid directory.
        ModuleAlreadyInstalled=Version '{0}' of module '{1}' is already installed at '{2}'. To delete version '{3}' and install version '{4}', run Install-Module, and add the -Force parameter.
        ModuleAlreadyInstalledVerbose=Version '{0}' of module '{1}' is already installed at '{2}'.
        InvalidPSModule=Module '{0}' cannot be installed because it is not a properly-formed module. To force installation, use the -Force parameter.
        ModuleNotInstalledOnThiseMachine=Module '{0}' was not updated because no valid module was found in the module directory. Verify that the module is located in the folder specified by $env:PSModulePath.
        AdminPrivilegesRequiredForUpdate=Module '{0}' (installed at'{1}') cannot be updated because Administrator rights are required to change that directory. Log on to the computer with an account that has Administrator rights, and then try again. You can also try running the Windows PowerShell session with elevated rights (Run as Administrator).
        ModuleNotInstalledUsingPowerShellGet=Module '{0}' was not installed by using Install-Module, so it cannot be updated.
        DownloadingModuleFromGallery=Downloading module '{0}' with version '{1}' from the repository '{2}'.
        NoUpdateAvailable=No updates were found for module '{0}'.
        FoundModuleUpdate=An update for the module '{0}' was found with version '{1}'.
        InvalidPSModuleDuringUpdate= Module '{0}' was not updated because the module in the repository '{1}' is not a valid Windows PowerShell module.
        ModuleGotUpdated=Module '{0}' has been updated successfully.
        TestingModuleInUse=Testing if the module to update is in use.
        ModuleDestination= The specified module will be installed in '{0}'.
        ModuleIsInUse=Module '{0}' is in currently in use.
        ModuleInstalledSuccessfully= Module '{0}' was installed successfully.
        CheckingForModuleUpdate= Checking for updates for module '{0}'.
        ModuleInUseWithProcessDetails=Module '{0}' is currently in use. Retry the operation after closing the following applications: '{1}'.
        ModuleNotAvailableLocally= The specified module '{0}' was not published because no module with that name was found in any module directory.
        AmbiguousModuleName=Modules with the name '{0}' are available under multiple paths. Add the -Path parameter to specify the module to publish.
        PublishModuleLocation=Module'{0}' was found in '{1}'.
        InvalidModuleToPublish=Module '{0}' cannot be published because it does not have a module manifest file. Run New-ModuleManifest -Path <PathName> to create a module manifest with metadata before publishing.         
        MissingRequiredManifestKeys=Module '{0}' cannot be published because it is missing required metadata. Verify that the module manifest specifies Description and Author.
        ModuleVersionShouldBeGreaterThanGalleryVersion=Module '{0}' with version '{1}' cannot be published. The version must exceed the current version '{2}' that exists in the repository '{3}'.
        CouldNotInstallNuGetBinaries=NuGet-anycpu.exe is required to interact with NuGet based galleries.  Please ensure that NuGet-anycpu.exe is available under '{0}' or '{1}'.
        InstallNuGetBinariesShouldContinueQuery=PowerShellGet requires NuGet-anycpu.exe to interact with NuGet based galleries. NuGet-anycpu.exe must be available in '{0}' or '{1}'. For more information about NuGet provider, see http://oneget.org/NuGet.html. Do you want PowerShellGet to download NuGet-anycpu.exe now?
        InstallNuGetBinariesShouldContinueCaption= NuGet-anycpu.exe is required to continue.
        DownloadingNugetBinaries=Downloading NuGet-anycpu.exe.        
        ModuleNotFound=Module '{0}' was not found.        
        FailedToCreateCompressedModule=Failed to generate the compressed module file for module '{0}'.
        FailedToPublish=Failed to publish module '{0}'.
        PublishedSuccessfully=Successfully published module '{0}' to the module publish location '{1}'.
        InvalidWebUri=The specified Uri '{0}' for parameter '{1}' is an invalid Web Uri. Please ensure that it meets the Web Uri requirements.
        RepositoryAlreadyRegistered=The repository could not be registered because there exists a registered repository with Name '{0}' and SourceLocation '{1}'. To register another repository with Name '{2}', please unregister the existing repository using the Unregister-PSRepository cmdlet.
        RepositoryToBeUnregisteredNotFound=The repository '{0}' was not removed because no repository was found with that name. Please run Get-PSRepository and ensure that a repository of that name is present.
        RepositoryCannotBeUnregistered=The specified repository '{0}' cannot be unregistered.
        RepositoryNotFound=No repository with the name '{0}' was found.
        RepositoryNameContainsWildCards=The repository name '{0}' should not have wildcards, correct it and try again.
        InvalidRepository=The specified repository '{0}' is not a valid registered repository name. Please ensure that '{1}' is a registered repository.
        RepositoryRegistered=Successfully registered the repository '{0}' with source location '{1}'.
        RepositoryUnregistered=Successfully unregistered the repository '{0}'.
        PSGalleryPublishLocationIsMissing=The specified repository '{0}' does not have a valid PublishLocation. Retry after setting the PublishLocation for repository '{1}' to a valid NuGet publishing endpoint using the Set-PSRepository cmdlet.
        PublishModuleSupportsOnlyNuGetBasedPublishLocations=Publish-Module only supports the NuGet-based publish locations. The PublishLocation '{0}' of the repository '{1}' is not a NuGet-based publish location. Retry after setting the PublishLocation for repository '{1}' to a valid NuGet publishing endpoint using the Set-PSRepository cmdlet.        
        DynamicParameterHelpMessage=The dynamic parameter '{0}' is required for Find-Module and Install-Module when using the OneGet provider '{1}' and source location '{2}'. Please enter your value for the '{3}' dynamic parameter:
        ProviderApiDebugMessage=In PSModule Provider - '{0}'.
        ModuleUninstallNotSupported=Module uninstallation is not supported. To remove a module, please delete the module folder.
        FastPackageReference=The FastPackageReference is '{0}'.
        OneGetProviderIsNotAvailable=The specified OneGet provider '{0}' is not available.
        SpecifiedSourceName=Using the specified source names : '{0}'.
        SpecifiedLocationAndOGP=The specified Location is '{0}' and OneGetProvider is '{1}'.
        NoSourceNameIsSpecified=The -Repository parameter was not specified.  PowerShellGet will use all of the registered repositories.
        GettingOneGetProviderObject=Getting the provider object for the OneGet Provider '{0}'.        
        InvalidInputObjectValue=Invalid value is specified for InputObject parameter.
        SpecifiedInstallationScope=The installation scope is specified to be '{0}'.
        SourceLocationValueForPSGalleryCannotBeChanged=The SourceLocation value for the PSGallery repository can not be changed.
        PublishLocationValueForPSGalleryCannotBeChanged=The PublishLocation value for the PSGallery repository can not be changed.
        SpecifiedProviderName=The specified OneGet provider name '{0}'.
        ProviderNameNotSpecified=User did not specify the OneGet provider name, trying with the provider name '{0}'.
        SpecifiedProviderNotAvailable=The specified OneGet provider '{0}' is not available.        
        SpecifiedProviderDoesnotSupportPSModules=The specified OneGet Provider '{0}' does not support PowerShell Modules. OneGet Providers must support the 'supports-powershell-modules' feature.
        PollingOneGetProvidersForLocation=Polling available OneGet Providers to find one that can support the specified source location '{0}'.
        PollingSingleProviderForLocation=Resolving the source location '{0}' with OneGet Provider '{1}'.
        FoundProviderForLocation=The OneGet provider '{0}' supports the source location '{1}'.
        SpecifiedLocationCannotBeRegistered=The specified location '{0}' cannot be registered.
        RepositoryDetails=Repository details, Name = '{0}', Location = '{1}'; IsTrusted = '{2}'; IsRegistered = '{3}'.
        NotSupportedPowerShellGetFormatVersion=The specified module '{0}' with PowerShellGetFormatVersion '{1}' is not supported by the current version of PowerShellGet. Get the latest version of the PowerShellGet module to install this module, '{2}'.
        PathNotFound=Cannot find the path '{0}' because it does not exist.
        ModuleIsNotTrusted=You are installing the module '{0}' from an untrusted repository. If you trust this repository, change its InstallationPolicy value by running the Set-PSRepository cmdlet.
	    #ScriptErrorsOrWarningsReturnedByPSScriptAnalyzer=Module cannot be published as Invoke-ScriptAnalyzer returns errors and/or warnings when analyzing the script files in the module. Please correct the errors and/or warnings before publishing.
###PSLOC
'@
