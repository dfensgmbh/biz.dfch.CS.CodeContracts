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
using System.Runtime.Remoting.Messaging;
using biz.dfch.CS.CodeContracts.Core.Exceptions;

namespace biz.dfch.CS.CodeContracts.Core.Attributes
{
    public class StringLengthAttribute : ParameterValidationAttributeBase
    {
        public override Type Type { get; protected set; } = typeof(string);

        public uint MinLength { get; }
        public uint MaxLength { get; }

        public StringLengthAttribute(string message)
            : this(uint.MinValue, uint.MaxValue, message)
        {
            // N/A
        }

        public StringLengthAttribute(uint minLength, uint maxLength)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }

        public StringLengthAttribute(uint minLength, uint maxLength, string message)
            : base(message)
        {
            MinLength = minLength;
            MaxLength = maxLength;

            Message = message;
        }

        public override bool TryValidate<T>(T value)
        {
            var valueAsString = (string) (object) value;

            return valueAsString.Length >= MinLength && valueAsString.Length <= MaxLength;
        }
    }
}
