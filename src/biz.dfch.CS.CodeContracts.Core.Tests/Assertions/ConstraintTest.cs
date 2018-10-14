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
using biz.dfch.CS.CodeContracts.Core.Assertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Assertions
{
    public class ArbitraryClass
    {
        public string ReturnNonEmptyString(bool returnNullOnFalse)
        {
            return Constraint.Result(LocalFunction);
            string LocalFunction()
            {
                return returnNullOnFalse
                    ? "arbitraryString"
                    : default(string);
            }
        }
    }
    [TestClass]
    public class ConstraintTest
    {
        [TestMethod]
        public void ConstraintAssertNotNullSucceeds()
        {
            Constraint.Assert(42 != 667);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ConstraintAssertWithNullThrows()
        {
            Constraint.Assert(42 == 667);
        }

        [TestMethod]
        public void ConstraintRequiresNotNullSucceeds()
        {
            Constraint.Requires(42 != 667);
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ConstraintRequiresWithNullThrows()
        {
            Constraint.Requires(42 == 667);
        }

        [TestMethod]
        public void ConstraintEnsuresNotNullSucceeds()
        {
            var result = new ArbitraryClass().ReturnNonEmptyString(returnNullOnFalse: true);
            Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        }

        [ExpectedException(typeof(ArgumentNullException))]
        [TestMethod]
        public void ConstraintEnsuresWithNullThrows()
        {
            var result = new ArbitraryClass().ReturnNonEmptyString(returnNullOnFalse: false);
            Assert.IsTrue(string.IsNullOrWhiteSpace(result));
        }

    }
}
