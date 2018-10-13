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
using biz.dfch.CS.CodeContracts.Core.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Attributes
{
    [TestClass]
    public class NotNullAttributeTest
    {
        [TestMethod]
        public void ValidatingStringReturnsTrue()
        {
            // Arrange
            var value = "arbitraryString";
            var sut = new NotNullAttribute();

            // Act
            var result = sut.TryValidate(value);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidatingNullStringReturnsFalse()
        {
            // Arrange
            var value = default(string);
            var sut = new NotNullAttribute();

            // Act
            var result = sut.TryValidate(value);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ValidatingObjectReturnsTrue()
        {
            // Arrange
            var value = new object();
            var sut = new NotNullAttribute();

            // Act
            var result = sut.TryValidate(value);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void ValidatingNullObjectReturnsFalse()
        {
            // Arrange
            var value = default(object);
            var sut = new NotNullAttribute();

            // Act
            var result = sut.TryValidate(value);

            // Assert
            Assert.IsFalse(result);
        }
    }
}
