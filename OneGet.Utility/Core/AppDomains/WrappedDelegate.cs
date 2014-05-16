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
    using System.Globalization;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;
    using Extensions;
    using Callback = System.Func<string, System.Collections.Generic.IEnumerable<object>, object>; 

    public static class WrappedDelegate {
        internal static Delegate CreateProxiedDelegate(this Type expectedDelegateType, object instance, MethodInfo method) {
            #region DEAD CODE

            // we need our public delegate to be calling an object that is MarshalByRef
            // instead, we're creating a delegate thats getting bound to the DTI, which isn't
            // extra hoops not needed:
            // var actualDelegate = Delegate.CreateDelegate(expectedDelegateType, duckTypedInstance, method);
            //   var huh = (object)Delegate.CreateDelegate(
            //    proxyDelegateType,
            //    actualDelegate.Target,
            //    actualDelegate.Method,
            //    true);

            #endregion

            // the func/action type for the proxied delegate.
            var proxyDelegateType = GetFuncOrActionType(expectedDelegateType.GetDelegateParameterTypes(), expectedDelegateType.GetDelegateReturnType());

            // create the actual delegate with the function/action instead
            var actualDelegate = Delegate.CreateDelegate(proxyDelegateType, instance, method);

            // the wrapped delegate class
            var wrappedType = method.GetWrappedDelegateType();

            // Create an instance of the WrappedDelegate object (this ties keeps the actual delegate in the right appdomain)
            // and exposes a delegate that is tied to a MarshalByRef object.
            var proxyObject = wrappedType.GetConstructor(new[] {
                proxyDelegateType
            }).Invoke(new[] {
                actualDelegate
            });

            // the delegate to the proxy object
            return Delegate.CreateDelegate(expectedDelegateType, proxyObject, "Invoke");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Called from Friend Assembly")]
        internal static object DynamicInvoke(this Type delegateType, object targetDelegate, object[] args) {
            return ((Invokable)delegateType.CreateWrappedProxy((Delegate)targetDelegate)).DynamicInvoke(args);
        }

        internal static object CreateWrappedProxy(this Type expectedDelegateType, Delegate dlg) {
            // the func/action type for the proxied delegate.
            var proxyDelegateType = GetFuncOrActionType(expectedDelegateType.GetDelegateParameterTypes(), expectedDelegateType.GetDelegateReturnType());

            // create the actual delegate with the function/action instead
            // we already have a viable delegate to use.
            // var actualDelegate = Delegate.CreateDelegate(proxyDelegateType, instance, method);
            var actualDelegate = (object)Delegate.CreateDelegate(
                proxyDelegateType,
                dlg.Target,
                dlg.Method,
                true);

            // the wrapped delegate class
            var wrappedType = dlg.GetWrappedDelegateType();

            // Create an instance of the WrappedDelegate object (this ties keeps the actual delegate in the right appdomain)
            // and exposes a delegate that is tied to a MarshalByRef object.
            return wrappedType.GetConstructor(new[] {
                proxyDelegateType
            }).Invoke(new[] {
                actualDelegate
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Still in development.")]
        internal static Delegate Wrap(this Delegate dlg) {
            return Delegate.CreateDelegate(dlg.GetType(), dlg.GetType().CreateWrappedProxy(dlg), "Invoke");
        }

        internal static Delegate CreateProxiedDelegate(this Type expectedDelegateType, Delegate dlg) {
            // the delegate to the proxy object
            return Delegate.CreateDelegate(expectedDelegateType, expectedDelegateType.CreateWrappedProxy(dlg), "Invoke");
        }

        public static Type GetWrappedDelegateType(this MethodInfo method) {
            if (method.ReturnType == typeof (void)) {
                return GetWrappedActionType(method.GetParameterTypes());
            }
            return GetWrappedFunctionType(method.GetParameterTypes(), method.ReturnType);
        }

        public static Type GetWrappedDelegateType(this Delegate dlg) {
            var delegateType = dlg.GetType();

            var returnType = delegateType.GetDelegateReturnType();
            if (returnType == typeof (void)) {
                return GetWrappedActionType(delegateType.GetDelegateParameterTypes());
            }
            return GetWrappedFunctionType(delegateType.GetDelegateParameterTypes(), returnType);
        }

        public static Type GetFuncOrActionType(IEnumerable<Type> argTypes, Type returnType) {
            return returnType == typeof (void) ? Expression.GetActionType(argTypes.ToArray()) : Expression.GetFuncType(argTypes.ConcatSingleItem(returnType).ToArray());
        }

        public static Type GetWrappedActionType(IEnumerable<Type> argTypes) {
            var types = argTypes.ToArray();

            switch (types.Length) {
                case 0:
                    return typeof (WrappedAction);
                case 1:
                    return typeof (WrappedAction<>).MakeGenericType(types);
                case 2:
                    return typeof (WrappedAction<,>).MakeGenericType(types);
                case 3:
                    return typeof (WrappedAction<,,>).MakeGenericType(types);
                case 4:
                    return typeof (WrappedAction<,,,>).MakeGenericType(types);
                case 5:
                    return typeof (WrappedAction<,,,,>).MakeGenericType(types);
                case 6:
                    return typeof (WrappedAction<,,,,,>).MakeGenericType(types);
                case 7:
                    return typeof (WrappedAction<,,,,,,>).MakeGenericType(types);
                case 8:
                    return typeof (WrappedAction<,,,,,,,>).MakeGenericType(types);
                case 9:
                    return typeof (WrappedAction<,,,,,,,,>).MakeGenericType(types);
                case 10:
                    return typeof (WrappedAction<,,,,,,,,,>).MakeGenericType(types);
                case 11:
                    return typeof (WrappedAction<,,,,,,,,,,>).MakeGenericType(types);
                case 12:
                    return typeof (WrappedAction<,,,,,,,,,,,>).MakeGenericType(types);
                case 13:
                    return typeof (WrappedAction<,,,,,,,,,,,,>).MakeGenericType(types);
                case 14:
                    return typeof (WrappedAction<,,,,,,,,,,,,,>).MakeGenericType(types);
                case 15:
                    return typeof (WrappedAction<,,,,,,,,,,,,,,>).MakeGenericType(types);
                case 16:
                    return typeof (WrappedAction<,,,,,,,,,,,,,,,>).MakeGenericType(types);
                default:
                    return (Type)null;
            }
        }

        public static Type GetWrappedFunctionType(IEnumerable<Type> argTypes, Type returnType) {
            var types = argTypes.ConcatSingleItem(returnType).ToArray();
            switch (types.Length) {
                case 1:
                    return typeof (WrappedFunc<>).MakeGenericType(types);
                case 2:
                    return typeof (WrappedFunc<,>).MakeGenericType(types);
                case 3:
                    return typeof (WrappedFunc<,,>).MakeGenericType(types);
                case 4:
                    return typeof (WrappedFunc<,,,>).MakeGenericType(types);
                case 5:
                    return typeof (WrappedFunc<,,,,>).MakeGenericType(types);
                case 6:
                    return typeof (WrappedFunc<,,,,,>).MakeGenericType(types);
                case 7:
                    return typeof (WrappedFunc<,,,,,,>).MakeGenericType(types);
                case 8:
                    return typeof (WrappedFunc<,,,,,,,>).MakeGenericType(types);
                case 9:
                    return typeof (WrappedFunc<,,,,,,,,>).MakeGenericType(types);
                case 10:
                    return typeof (WrappedFunc<,,,,,,,,,>).MakeGenericType(types);
                case 11:
                    return typeof (WrappedFunc<,,,,,,,,,,>).MakeGenericType(types);
                case 12:
                    return typeof (WrappedFunc<,,,,,,,,,,,>).MakeGenericType(types);
                case 13:
                    return typeof (WrappedFunc<,,,,,,,,,,,,>).MakeGenericType(types);
                case 14:
                    return typeof (WrappedFunc<,,,,,,,,,,,,,>).MakeGenericType(types);
                case 15:
                    return typeof (WrappedFunc<,,,,,,,,,,,,,,>).MakeGenericType(types);
                case 16:
                    return typeof (WrappedFunc<,,,,,,,,,,,,,,,>).MakeGenericType(types);
                case 17:
                    return typeof (WrappedFunc<,,,,,,,,,,,,,,,,>).MakeGenericType(types);
                default:
                    return (Type)null;
            }
        }
    }

   

    public class WrappedCallback : Invokable {
        private readonly Callback _func;

        public WrappedCallback() {
        }

        public WrappedCallback(Callback func) {
            _func = func;
        }

        public object Invoke(string s, IEnumerable<object> i) {
            try {
                return _func.Invoke(s, i);
            } catch (Exception e) {
                throw new Exception(string.Format(CultureInfo.CurrentCulture, "{0}/{1}\r\n{2}", e.GetType().Name, e.Message, e.StackTrace));
            }
            // return default(TRet);
        }

        public override object DynamicInvoke(object[] args) {
            if (args == null) {
                throw new ArgumentNullException("args");
            }
            if (args.Length < 2) {
                throw new Exception("DynamicInvoke with too few args");
            }
            return Invoke((string)args[0], (IEnumerable<string>)args[1]);
        }
    }
}
