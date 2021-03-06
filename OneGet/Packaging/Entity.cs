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

namespace Microsoft.OneGet.Packaging {
    using System.Xml.Linq;

    public class Entity : Meta {
        internal Entity(XElement element) : base(element) {
        }

        public string Name {
            get {
                return this[Iso19770_2.NameAttribute.LocalName];
            }
        }

        public string Role {
            get {
                return this[Iso19770_2.RoleAttribute.LocalName];
            }
        }

        public string Thumbprint {
            get {
                return this[Iso19770_2.ThumbprintAttribute.LocalName];
            }
        }

        public string RegId {
            get {
                return this[Iso19770_2.RegIdAttribute.LocalName];
            }
        }
    }
}