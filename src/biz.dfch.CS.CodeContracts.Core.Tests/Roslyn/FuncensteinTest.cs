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
using System.IO;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    [TestClass]
    public class FuncensteinTest
    {
        [TestMethod]
        public void InsertingStatementInMethodSucceeds()
        {
            // Arrange
            var message = "tralala";
            var identifier = nameof(message);
            var sut = CodeContractsFactory.AssertNotNull(identifier);

            var fileInfo = new FileInfo(@"C:\src\biz.dfch.CS.EA.ProductModeler\src\biz.dfch.CS.EA.ProductModeler.Tests\ScratchPad\Roslyn\Funcenstein.cs");
            var source = File.ReadAllText(fileInfo.FullName);
            var tree = CSharpSyntaxTree.ParseText(source);
            var root = tree.GetCompilationUnitRoot();

            // Act
            // get first statement in method / assuming this file has only one class
            var statement = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Select(b => b.DescendantNodes().OfType<BlockSyntax>()
                        .Select(n => n.DescendantNodes()
                        .First()))
                    .First()
                .First();

            var statementToBeInserted = SyntaxFactory.ParseStatement(sut.SyntaxTree.ToString());
            var statements = new []
            {
                statementToBeInserted.WithLeadingTrivia(statement.GetLeadingTrivia().ToSyntaxTriviaList()),
                statement,
            };

            Assert.IsFalse(root.SyntaxTree.ToString().Contains(statementToBeInserted.ToString()));
            var result = root.ReplaceNode(statement, statements);

            // Assert
            Assert.IsTrue(result.ToString().Contains(statementToBeInserted.ToString()));
            Trace.WriteLine(result.ToString());
        }
    }
}
