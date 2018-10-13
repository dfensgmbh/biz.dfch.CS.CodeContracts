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
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.CodeContracts.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Attributes
{
    [TestClass]
    public class StringLengthAttributeTest
    {
        [DataTestMethod]
        [DataRow((uint) 0, (uint) 10)]
        [DataRow((uint) 0, (uint) 11)]
        [DataRow((uint) 1, (uint) 10)]
        [DataRow((uint) 10, (uint) 10)]
        [DataRow((uint) 1, (uint) 11)]
        [DataRow((uint)0, uint.MaxValue)]
        public void ValidatingStringLengthReturnsTrue(uint minLength, uint maxLength)
        {
            // Arrange
            var value = "1234567890";
            var sut = new StringLengthAttribute(minLength: minLength, maxLength: maxLength);
            
            // Act
            var result = sut.TryValidate(value);

            // Assert
            Assert.IsTrue(result);
        }

        [DataTestMethod]
        [DataRow((uint)0, (uint)9)]
        [DataRow((uint)11, (uint)10)]
        [DataRow(uint.MaxValue, (uint)10)]
        [DataRow((uint) 0, uint.MinValue)]
        public void ValidatingStringLengthReturnsFalse(uint minLength, uint maxLength)
        {
            // Arrange
            var value = "1234567890";
            var sut = new StringLengthAttribute(minLength: minLength, maxLength: maxLength);

            // Act
            var result = sut.TryValidate(value);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
