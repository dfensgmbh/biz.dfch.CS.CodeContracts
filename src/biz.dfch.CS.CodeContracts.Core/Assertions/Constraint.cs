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
using System.Diagnostics;

namespace biz.dfch.CS.CodeContracts.Core.Assertions
{
    public static class ConstraintInternal
    {
        public static void Assert(bool condition, string message, string detailMessage)
        {
            if(condition) return;

            var s = $"{message}. Details: '{detailMessage}'. StackTrace:\r\n{new StackTrace()}.";
            Trace.WriteLine(s);

            throw new ArgumentNullException(s);
        }

    }
    public static class Constraint
    {
        private const string CONSTRAINT_ASSERT_FAILED = "Constraint.Assert failed";
        private const string CONSTRAINT_REQUIRES_FAILED = "Constraint.Requires failed";

        public static void Assert(bool condition)
        {
            ConstraintInternal.Assert(condition, CONSTRAINT_ASSERT_FAILED, string.Empty);
        }

        public static void Assert(bool condition, string message)
        {
            ConstraintInternal.Assert(condition, CONSTRAINT_ASSERT_FAILED, message);
        }

        public static void Requires(bool condition)
        {
            ConstraintInternal.Assert(condition, CONSTRAINT_REQUIRES_FAILED, string.Empty);
        }

        public static void Requires(bool condition, string message)
        {
            ConstraintInternal.Assert(condition, CONSTRAINT_REQUIRES_FAILED, message);
        }

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
                    var message = $": null != Contract.Result<{typeof(T).Name}>(). {func.Target}: {func.Method}";
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    ConstraintInternal.Assert(Postcondition_failed, nameof(Postcondition_failed), message);
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
                    var message = $": Contract.Result<{typeof(T).Name}>(). {func.Target}: {func.Method}";
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    ConstraintInternal.Assert(Postcondition_failed, nameof(Postcondition_failed), message);
                }
            }
        }
    }
}
