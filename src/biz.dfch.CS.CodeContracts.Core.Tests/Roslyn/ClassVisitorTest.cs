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

using System.Diagnostics;
using biz.dfch.CS.Commons.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    [TestClass]
    public class ClassVisitorTest : VisitorTestBase
    {
        [TestMethod]
        public void InvokingVisitorReturnsAllClasses()
        {
            // Arrange
            Reset().Initialise(FILE_NAME_DEFAULT);
            var sut = new ClassVisitor();

            // Act
            sut.Visit(Root);
            var result = sut.Nodes;

            // Assert
            sut.Nodes.ForEach(e => Trace.WriteLine(e.Identifier.Text));
            Assert.AreEqual(3, result.Count);
        }
    }
}
