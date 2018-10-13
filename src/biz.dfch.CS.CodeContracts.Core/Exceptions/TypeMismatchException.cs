/**
 * Copyright 2018 d-fens GmbH
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 * http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;

namespace biz.dfch.CS.CodeContracts.Core.Exceptions
{
    public class TypeMismatchException : NotSupportedException
    {
        private const string TYPE_MISMATCH_EXCEPTION_MESSAGE = "Expected type '{0}' does not match actual type '{1}'.";

        private static string FormatMessage(Type expectedType, Type actualType)
        {
            return string.Format(TYPE_MISMATCH_EXCEPTION_MESSAGE, expectedType.FullName, actualType.FullName);
        }

        public TypeMismatchException(Type expectedType, Type actualType)
            : base(FormatMessage(expectedType, actualType))
        {
            // N/A
        }
    }
}
