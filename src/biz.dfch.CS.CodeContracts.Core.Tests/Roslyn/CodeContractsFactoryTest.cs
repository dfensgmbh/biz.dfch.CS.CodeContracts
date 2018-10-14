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

using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    [TestClass]
    public class CodeContractsFactoryTest
    {
        [TestMethod]
        public void Test()
        {
            #region CodeContracts
                        System.Diagnostics.Trace.Assert(null != this);
            #endregion

            // Arrange
            var message = "tralala";
            var identifier = nameof(message);

            // Act
            // Assert
            var sut = CodeContractsFactory.AssertNotNull(identifier);
            Assert.IsNotNull(sut);

            var result = sut.SyntaxTree.ToString();
            Assert.IsTrue(result.Contains(identifier));

            var tree = sut.SyntaxTree;
            var root = tree.GetRoot();
            var statement = (StatementSyntax) root.FindNode(root.GetLocation().SourceSpan);
            var statements = new SyntaxNode[]
            {
                statement,
                statement,
            };
            var replaced = root.ReplaceNode(statement, statements);
            var firstNodeInList = sut.SyntaxTree.GetRoot().DescendantNodesAndSelf().First();
            //var xyz = sut.SyntaxTree.GetRoot().FindNode(new TextSpan(0, sut.SyntaxTree.GetText().Length));
            var xyz = sut.SyntaxTree.GetRoot().FindNode(sut.GetLocation().SourceSpan);
            var newNodes = new List<SyntaxNode> {firstNodeInList};
            var newTree = sut.SyntaxTree.GetRoot().InsertNodesBefore(xyz, newNodes);

            var newResult = newTree.SyntaxTree.ToString();
            Assert.IsTrue(newResult.Contains(identifier));
        }
    }
}
