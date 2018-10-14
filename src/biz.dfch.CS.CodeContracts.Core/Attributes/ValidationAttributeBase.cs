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

namespace biz.dfch.CS.CodeContracts.Core.Attributes
{
    public abstract class ValidationAttributeBase : Attribute, ICodeContractValidation
    {
        public const string MESSAGE_DEFAULT = "Contract validation failed.";

        public string Message { get; set; }

        public virtual Type Type { get; protected set; }

        protected ValidationAttributeBase()
        {
            Message = MESSAGE_DEFAULT;
        }

        protected ValidationAttributeBase(string message)
        {
            Message = message;
        }

        public virtual bool TryValidate<T>(T value)
        {
            Type = typeof(T);

            throw new NotImplementedException();
        }
    }
}
