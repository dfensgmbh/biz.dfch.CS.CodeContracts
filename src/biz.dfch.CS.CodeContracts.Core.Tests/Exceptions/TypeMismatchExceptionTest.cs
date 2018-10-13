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
using biz.dfch.CS.CodeContracts.Core.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Exceptions
{
    [TestClass]
    public class TypeMismatchExceptionTest
    {
        [TestMethod]
        public void TypeMismatchExceptionContainsExpectedAndActualTypeNames()
        {
            // Arrange
            var expectedType = typeof(string);
            var actualType = typeof(int);

            // Act
            var sut = new TypeMismatchException(expectedType, actualType);

            // Assert
            Assert.IsTrue(sut.Message.Contains(expectedType.Name)); 
            Assert.IsTrue(sut.Message.Contains(actualType.Name)); 
        }
    }
}
