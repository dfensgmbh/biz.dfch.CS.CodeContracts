#r "C:\src\biz.dfch.CS.EA.ProductModeler\src\packages\Microsoft.CodeAnalysis.Common.2.0.0\lib\netstandard1.3\Microsoft.CodeAnalysis.dll"
#r "C:\src\biz.dfch.CS.EA.ProductModeler\src\packages\Microsoft.CodeAnalysis.CSharp.2.0.0\lib\netstandard1.3\Microsoft.CodeAnalysis.CSharp.dll"
#r "C:\src\biz.dfch.CS.EA.ProductModeler\src\biz.dfch.CS.EA.ProductModeler.Tests\bin\Debug\biz.dfch.CS.EA.ProductModeler.Tests.dll"
#r "C:\src\biz.dfch.CS.EA.ProductModeler\src\packages\MSTest.TestFramework.1.3.2\lib\net45\Microsoft.VisualStudio.TestPlatform.TestFramework.dll"
#r "C:\src\biz.dfch.CS.EA.ProductModeler\src\packages\MSTest.TestFramework.1.3.2\lib\net45\Microsoft.VisualStudio.TestPlatform.TestFramework.Extensions.dll"
#r "C:\Program Files (x86)\Sparx Systems\EA\Interop.EA.dll"

using System;
using System.Diagnostics.Contracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using biz.dfch.CS.EA.ProductModeler.Tests.AddIn;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using biz.dfch.CS.EA.ProductModeler.Tests.ScratchPad.Roslyn;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

var message = "tralala";
var identifier = nameof(message);
var sut = CodeContractsFactory.AssertNotNull(identifier);

var fileInfo = new FileInfo(@"C:\src\biz.dfch.CS.EA.ProductModeler\src\biz.dfch.CS.EA.ProductModeler.Tests\ScratchPad\Roslyn\Funcenstein.cs");
var source = File.ReadAllText(fileInfo.FullName);
var tree = CSharpSyntaxTree.ParseText(source);
var root = tree.GetCompilationUnitRoot();

// var statement = root.DescendantNodes().OfType<MethodDeclarationSyntax>().First(e => e.Identifier.Text == "Recurse");
var statement = root.DescendantNodes().OfType<MethodDeclarationSyntax>()
                    .Select(b => b.DescendantNodes().OfType<BlockSyntax>()
                        .Select(n => n.DescendantNodes().First()))
                        .First()
                        .First();
var statementToBeInserted = SyntaxFactory.ParseStatement(sut.SyntaxTree.ToString());

var statements = new SyntaxNode[]
{
    statementToBeInserted.WithLeadingTrivia(statement.GetLeadingTrivia().ToSyntaxTriviaList()),
	statement,
};
var replaced = root.ReplaceNode(statement, statements);
