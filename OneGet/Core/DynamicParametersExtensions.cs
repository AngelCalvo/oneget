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

namespace Microsoft.PowerShell.OneGet.Core {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Management.Automation;
    using Microsoft.OneGet.Core.Extensions;
    using Microsoft.OneGet.Core.Packaging;

    internal static class DynamicParametersExtensions {
        public static RuntimeDefinedParameter CreateRuntimeDynamicParameter(this MetadataDefinition definition ) {
            var values = definition.PossibleValues.ToArray();

            Type parameterType;
            switch ((definition.Type ?? "string").ToLower(CultureInfo.CurrentCulture) ) {
                case "switch":
                    parameterType = typeof(SwitchParameter);
                    break;
                case "uri":
                    parameterType = typeof(Uri);
                    break;
                case "string[]":
                    parameterType = typeof(string[]);
                    break;
                case "int":
                    parameterType = typeof(int);
                    break;
                case "double":
                    parameterType = typeof(double);
                    break;
                case "bool":
                    parameterType = typeof(bool?);
                    break;
                
                default:
                    parameterType = typeof(string);
                    break;
            }
            

            if (values.IsNullOrEmpty()) {
                return new RuntimeDefinedParameter(definition.Name, parameterType, new Collection<Attribute> {
                    new ParameterAttribute()
                });
            }

            return new RuntimeDefinedParameter(definition.Name, parameterType, new Collection<Attribute> {
                new ParameterAttribute(),
                new ValidateSetAttribute(values)
            });
        }
    }
}