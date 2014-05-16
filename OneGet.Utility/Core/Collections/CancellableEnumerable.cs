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

namespace Microsoft.OneGet.Core.Collections {
    using System;
    using System.Collections.Generic;
    using System.Threading;

    public class CancellableEnumerable<T> : ByRefEnumerable<T>, IDisposable, ICancellable {
        protected CancellationTokenSource _cancellationTokenSource;

        public CancellableEnumerable(CancellationTokenSource cts, IEnumerable<T> enumerable) : base(enumerable) {
            _cancellationTokenSource = cts;
        }

        public bool IsCancelled {
            get {
                return _cancellationTokenSource.Token.IsCancellationRequested;
            }
        }

        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            Cancel();
            if (disposing) {
                
                if (_cancellationTokenSource != null) {
                    _cancellationTokenSource.Dispose();
                }
                _cancellationTokenSource = null;
            }
        }

        public void Cancel() {
            if (_cancellationTokenSource != null) {
                _cancellationTokenSource.Cancel();
            }   
        }

        public override IEnumerator<T> GetEnumerator() {
            return new CancellableEnumerator<T>(_cancellationTokenSource, _enumerable.GetEnumerator());
        }
    }
}