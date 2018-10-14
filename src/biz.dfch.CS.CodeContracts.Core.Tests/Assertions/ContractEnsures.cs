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

namespace biz.dfch.CS.CodeContracts.Core.Tests.Assertions
{
    public static class ContractEnsures
    {
        public static T Result<T>(Func<T> func)
        {
            var result = default(T);

            try
            {
                result = func.Invoke();
                return result;
            }
            finally
            {
                // ReSharper disable once InconsistentNaming
                // ReSharper disable once IdentifierTypo
                var Postcondition_failed = null != result;
                if (!Postcondition_failed)
                {
                    var message = string.Format(": null != Contract.Result<{0}>(). {1}: {2}", typeof(T).Name, func.Target, func.Method);
                    System.Diagnostics.Contracts.Contract.Assert(Postcondition_failed, message);
                }
            }
        }

        public static T Result<T>(Func<T> func, Func<T, bool> postCondition)
        {
            var result = default(T);

            try
            {
                result = func.Invoke();
                return result;
            }
            finally
            {
                // ReSharper disable once InconsistentNaming
                // ReSharper disable once IdentifierTypo
                var Postcondition_failed = postCondition.Invoke(result);
                if (!Postcondition_failed)
                {
                    var message = string.Format(": Contract.Result<{0}>(). {1}: {2}", typeof(T).Name, func.Target, func.Method);
                    System.Diagnostics.Contracts.Contract.Assert(Postcondition_failed, message);
                }
            }
        }
    }
}
