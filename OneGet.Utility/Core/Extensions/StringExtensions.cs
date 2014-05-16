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

namespace Microsoft.OneGet.Core.Extensions {
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;

    
    public static class StringExtensions {
        // ReSharper disable InconsistentNaming
        /// <summary>
        ///     Formats the specified format string.
        /// </summary>
        /// <param name="formatString"> The format string. </param>
        /// <param name="args"> The args. </param>
        /// <returns> </returns>
        /// <remarks>
        /// </remarks>
        public static string format(this string formatString, params object[] args) {
            if (args == null || args.Length == 0) {
                return formatString;
            }

            try {
                return String.Format(CultureInfo.CurrentCulture, formatString, args);
            } catch (Exception) {
                return formatString.Replace('{', '[').Replace('}', ']');
            }
        }

        public static string formatWithIEnumerable(this string formatString, IEnumerable<object> args) {
            if (args.IsNullOrEmpty()) {
                return formatString;
            }
            return string.Format(CultureInfo.CurrentCulture, formatString, args.ToArray());
        }

        public static bool Is(this string str) {
            return !string.IsNullOrEmpty(str);
        }

        public static bool IsEmptyOrNull(this string str) {
            return string.IsNullOrEmpty(str);
        }

        public static string DashedToCamelCase(this string dashedText, char separator) {
            return dashedText.IndexOf('-') == -1 ? dashedText : new string(dashedToCamelCase(dashedText, separator).ToArray());
        }
        public static string DashedToCamelCase(this string dashedText) {
            return dashedText.DashedToCamelCase('-');
        }

        private static IEnumerable<char> dashedToCamelCase(this string dashedText, char separator = '-', bool pascalCase = false) {
            var nextIsUpper = pascalCase;
            foreach (var ch in dashedText) {
                if (ch == '-') {
                    nextIsUpper = true;
                } else {
                    yield return nextIsUpper ? char.ToUpper(ch) : ch;
                    nextIsUpper = false;
                }
            }
        }

        /// <summary>
        ///     Encodes the string as an array of UTF8 bytes.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns> </returns>
        /// <remarks>
        /// </remarks>
        public static byte[] ToByteArray(this string text) {
            return Encoding.UTF8.GetBytes(text);
        }

        /// <summary>
        ///     Creates a string from a collection of UTF8 bytes
        /// </summary>
        /// <param name="bytes"> The bytes. </param>
        /// <returns> </returns>
        /// <remarks>
        /// </remarks>
        public static string ToUtf8String(this IEnumerable<byte> bytes) {
            return Encoding.UTF8.GetString(bytes.ToArray());
        }

        public static string ToBase64(this string text) {
            if (text == null) {
                return null;
            }

            return Convert.ToBase64String(text.ToByteArray());
        }

        public static string FromBase64(this string text) {
            if (text == null) {
                return null;
            }
            return Convert.FromBase64String(text).ToUtf8String();
        }


        public static bool IsTrue(this string text) {
            return text.Is() && text.Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }

        /// <summary>
        ///     coerces a string to an int32, defaults to zero.
        /// </summary>
        /// <param name="str"> The STR. </param>
        /// <param name="defaultValue"> The default value if the string isn't a valid int. </param>
        /// <returns> </returns>
        /// <remarks>
        /// </remarks>
        public static int ToInt32(this string str, int defaultValue) {
            int i;
            return Int32.TryParse(str, out i) ? i : defaultValue;
        }

        public static bool EqualsIgnoreCase(this string str, string str2) {
            if (str == null && str2 == null) {
                return true;
            }

            if (str == null || str2 == null) {
                return false;
            }

            return str.Equals(str2, StringComparison.OrdinalIgnoreCase);
        }
        // ReSharper restore InconsistentNaming
    }
}