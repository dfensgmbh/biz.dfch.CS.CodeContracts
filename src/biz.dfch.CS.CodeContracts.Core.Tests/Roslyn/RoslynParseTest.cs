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
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using biz.dfch.CS.CodeContracts.Core.Assertions;
using biz.dfch.CS.CodeContracts.Core.Attributes;
using biz.dfch.CS.Commons.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    public static class ClassDeclarationSyntaxExtensions
    {
        public const string NESTED_CLASS_DELIMITER = "+";
        public const string NAMESPACE_CLASS_DELIMITER = ".";

        public static string GetFullName([NotNull] this ClassDeclarationSyntax source)
        {
            Constraint.Requires(null != source);

            var items = new List<string>();
            var parent = source.Parent;
            while (parent.IsKind(SyntaxKind.ClassDeclaration))
            {
                var parentClass = parent as ClassDeclarationSyntax;
                Contract.Assert(null != parentClass);
                items.Add(parentClass.Identifier.Text);

                parent = parent.Parent;
            }

            var nameSpace = parent as NamespaceDeclarationSyntax;
            Contract.Assert(null != nameSpace);
            var sb = new StringBuilder().Append(nameSpace.Name).Append(NAMESPACE_CLASS_DELIMITER);
            items.Reverse();
            items.ForEach(i => { sb.Append(i).Append(NESTED_CLASS_DELIMITER); });
            sb.Append(source.Identifier.Text);

            var result = sb.ToString();
            return result;
        }

        public static string GetNamespace([NotNull] this ClassDeclarationSyntax source)
        {
            Contract.Requires(null != source);

            var parent = source.Parent;
            while (parent.IsKind(SyntaxKind.ClassDeclaration))
            {
                Contract.Assert(parent is ClassDeclarationSyntax parentClass);

                parent = parent.Parent;
            }

            var nameSpace = parent as NamespaceDeclarationSyntax;
            Contract.Assert(null != nameSpace);

            var result = nameSpace.Name.ToString();
            return result;
        }

        public static SyntaxList<UsingDirectiveSyntax> GetUsings([NotNull] this ClassDeclarationSyntax source)
        {
            Contract.Requires(null != source);

            var root = default(CompilationUnitSyntax);
            var parent = source.Parent;
            while (null != parent)
            {
                root = parent as CompilationUnitSyntax;
                if (null != root) break;

                parent = parent.Parent;
            }
            Contract.Assert(null != root);

            var result = root.Usings;
            return result;
        }
    }

    public static class InterfaceDeclarationSyntaxExtensions
    {
        public const string NAMESPACE_CLASS_DELIMITER = ".";

        public static string GetFullName(this InterfaceDeclarationSyntax source)
        {
            Contract.Requires(null != source);

            var parent = source.Parent;
            while (!parent.IsKind(SyntaxKind.CompilationUnit))
            {
                parent = parent.Parent;
            }

            var result = parent is NamespaceDeclarationSyntax nameSpace
                ? nameSpace.Name + NAMESPACE_CLASS_DELIMITER + source.Identifier.Text
                : source.Identifier.Text;
            return result;
        }

        //public static string GetNamespace(this InterfaceDeclarationSyntax source)
        //{
        //    Contract.Requires(null != source);

        //    var parent = source.Parent;
        //    while (parent.IsKind(SyntaxKind.ClassDeclaration))
        //    {
        //        Contract.Assert(parent is InterfaceDeclarationSyntax parentClass);

        //        parent = parent.Parent;
        //    }

        //    var nameSpace = parent as NamespaceDeclarationSyntax;
        //    Contract.Assert(null != nameSpace);

        //    var result = nameSpace.Name.ToString();
        //    return result;
        //}

        //public static SyntaxList<UsingDirectiveSyntax> GetUsings(this InterfaceDeclarationSyntax source)
        //{
        //    Contract.Requires(null != source);

        //    var root = default(CompilationUnitSyntax);
        //    var parent = source.Parent;
        //    while (null != parent)
        //    {
        //        root = parent as CompilationUnitSyntax;
        //        if (null != root) break;

        //        parent = parent.Parent;
        //    }
        //    Contract.Assert(null != root);

        //    var result = root.Usings;
        //    return result;
        //}
    }

    public class TypeInfo
    {
        public SyntaxKind Kind { get; set; }
        public FileInfo FileInfo { get; set; }
        public string Identifier { get; set; }
    }

    [TestClass]
    public class RoslynParseTest
    {
        private const string NESTED_CLASS_DELIMITER = "+";
        private const string NAMESPACE_CLASS_DELIMITER = ".";

        public Dictionary<string, TypeInfo> typeMap = new Dictionary<string, TypeInfo>();

        public Dictionary<string, FileInfo> fileMap = new Dictionary<string, FileInfo>();

        [TestMethod]
        public void Test2()
        {
            var result = CodeContractsFactory.AssertNotNull("tralala");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void Test()
        {
            // Arrange
            var pwd = Directory.GetCurrentDirectory();
            var projectDirectory = @"C:\src\biz.dfch.CS.EA.ProductModeler\src\biz.dfch.CS.EA.ProductModeler.Tests";

            // first read all source files
            var directoryInfo = new DirectoryInfo(projectDirectory);
            ParseDirectory(directoryInfo);

            // second extract all types (now only classes and interfaces) from source files
            fileMap.Values.ForEach(ExtractTypes);

            // Act
            typeMap.ForEach(ProcessType);

            // Assert
            Assert.IsNotNull(typeMap);
        }

        private void ParseDirectory(DirectoryInfo directoryInfo)
        {
            Contract.Requires(null != directoryInfo);

            directoryInfo.GetDirectories(@"*.*").ForEach(ParseDirectory);

            directoryInfo.GetFiles(@"*.cs").ForEach(ParseFile);
        }

        private void ParseFile(FileInfo fileInfo)
        {
            Contract.Requires(null != fileInfo);

            fileMap.Add(fileInfo.FullName, fileInfo);
        }

        private void ExtractTypes(FileInfo fileInfo)
        {
            var source = File.ReadAllText(fileInfo.FullName);
            var tree = CSharpSyntaxTree.ParseText(source);
            var root = tree.GetCompilationUnitRoot();

            // global:: types are missing
            //root.DescendantNodesAndSelf().OfType<ClassDeclarationSyntax>().ForEach(f =>
            //{
            //    Trace.WriteLine($"global:: Class found: {f.Identifier.Text}. Kind {f.Kind()}. Type {f.GetType().FullName}");

            //    typeMap.Add(f.GetFullName(), new TypeInfo
            //    {
            //        Kind = f.Kind(),
            //        Identifier = f.Identifier.Text,
            //        FileInfo = fileInfo,
            //    });
            //});

            //root.DescendantNodesAndSelf().OfType<InterfaceDeclarationSyntax>().ForEach(f =>
            //{
            //    Trace.WriteLine($"global:: Interface found: {f.Identifier.Text}. Kind {f.Kind()}. Type {f.GetType().FullName}");

            //    typeMap.Add(f.GetFullName(), new TypeInfo
            //    {
            //        Kind = f.Kind(),
            //        Identifier = f.Identifier.Text,
            //        FileInfo = fileInfo,
            //    });
            //});

            root.DescendantNodesAndSelf().OfType<NamespaceDeclarationSyntax>().ForEach(e =>
            {
                Trace.WriteLine($"Namespace found: {e.Name}. Kind {e.Kind()}. Type {e.GetType().FullName}");

                e.DescendantNodes().OfType<ClassDeclarationSyntax>().ForEach(f =>
                {
                    Trace.WriteLine($"Class found: {f.Identifier.Text}. Kind {f.Kind()}. Type {f.GetType().FullName}");

                    typeMap.Add(f.GetFullName(), new TypeInfo
                    {
                        Kind = f.Kind(),
                        Identifier = f.Identifier.Text,
                        FileInfo = fileInfo,
                    });
                });

                e.DescendantNodes().OfType<InterfaceDeclarationSyntax>().ForEach(f =>
                {
                    Trace.WriteLine($"Interface found: {f.Identifier.Text}. Kind {f.Kind()}. Type {f.GetType().FullName}");

                    var fullName = $"{e.Name}.{f.Identifier.Text}";
                    typeMap.Add(fullName, new TypeInfo
                    {
                        Kind = f.Kind(),
                        Identifier = f.Identifier.Text,
                        FileInfo = fileInfo,
                    });
                });
            });
        }

        private void ProcessType(KeyValuePair<string, TypeInfo> item)
        {
            var typeInfo = item.Value;
            using (var sr = new StreamReader(typeInfo.FileInfo.FullName))
            {
                var source = sr.ReadToEnd();
                var tree = CSharpSyntaxTree.ParseText(source);
                var root = tree.GetCompilationUnitRoot();

                switch (typeInfo.Kind)
                {
                    case SyntaxKind.ClassDeclaration:
                        var classDeclaration = root.DescendantNodes().OfType<ClassDeclarationSyntax>()
                            .First(e => e.Identifier.Text == typeInfo.Identifier && e.GetFullName() == item.Key);
                        ProcessClass(classDeclaration, item);
                        break;
                    case SyntaxKind.InterfaceDeclaration:
                        root.DescendantNodes().OfType<InterfaceDeclarationSyntax>()
                            .Where(e => e.Identifier.Text == typeInfo.Identifier)
                            .ForEach(e => ProcessInterface(e, item));
                        break;
                    default:
                        Contract.Assert(!Enum.IsDefined(typeof(SyntaxKind), typeInfo.Kind), "Unexpected typeInfo.Kind.");
                        break;
                }
            }
        }

        private void ProcessBaseListIdentifierName(IdentifierNameSyntax identifierNameSyntax, KeyValuePair<string, TypeInfo> item, SyntaxList<UsingDirectiveSyntax> usings, string nameSpace)
        {
            Contract.Requires(null != identifierNameSyntax);

            Trace.WriteLine($"Class {item.Key} implements/inherits {identifierNameSyntax.Identifier.Text}");

            var fullIdentifier = nameSpace + "." + identifierNameSyntax.Identifier.Text;
            if (!typeMap.ContainsKey(fullIdentifier))
            {
                var usingNames = usings.Select(u => u.Name.ToFullString() + "." + identifierNameSyntax.Identifier.Text);
                fullIdentifier = usingNames.SingleOrDefault(u => typeMap.ContainsKey(u));
                if (null == fullIdentifier) return;
            }

            Trace.WriteLine($"Class {item.Key} implements/inherits {fullIdentifier} in {typeMap[fullIdentifier].FileInfo.FullName}");
        }

        private void ProcessBaseListQualifiedNameSyntax(QualifiedNameSyntax qualifiedNameSyntax, KeyValuePair<string, TypeInfo> item, SyntaxList<UsingDirectiveSyntax> usings, string nameSpace)
        {
            Contract.Requires(null != qualifiedNameSyntax);

            Trace.WriteLine($"Class {item.Key} implements/inherits {qualifiedNameSyntax.Left}{qualifiedNameSyntax.DotToken}{qualifiedNameSyntax.Right}");

            var usingDirectiveSyntax = usings.Where(e => null != e.Alias)
                .FirstOrDefault(e => e.Alias.Name.Identifier.Text == qualifiedNameSyntax.Left.ToString());
            if (!(usingDirectiveSyntax?.Name is IdentifierNameSyntax identifierNameSyntax)) return;

            var fullIdentifier = identifierNameSyntax.Identifier.Text + "." + qualifiedNameSyntax.Right;
            if (!typeMap.ContainsKey(fullIdentifier)) return;

            Trace.WriteLine($"Class {item.Key} implements/inherits {fullIdentifier} in {typeMap[fullIdentifier].FileInfo.FullName}");

        }

        private void ProcessBaseListAliasQualifiedNameSyntax(AliasQualifiedNameSyntax aliasQualifiedNameSyntax, KeyValuePair<string, TypeInfo> item, SyntaxList<UsingDirectiveSyntax> usings, string nameSpace)
        {
            Contract.Requires(null != aliasQualifiedNameSyntax);

            Trace.WriteLine($"Class {item.Key} implements/inherits {aliasQualifiedNameSyntax.Alias}{aliasQualifiedNameSyntax.ColonColonToken}{aliasQualifiedNameSyntax.Name.Identifier.Text}");

            var usingDirectiveSyntax = usings.FirstOrDefault();
        }

        private void ProcessClass(ClassDeclarationSyntax syntaxNode, KeyValuePair<string, TypeInfo> item)
        {
            var nameSpace = syntaxNode.GetNamespace();
            var usings = syntaxNode.GetUsings();
            syntaxNode.BaseList?.Types.ForEach(e =>
            {
                switch (e.Type.Kind())
                {
                    case SyntaxKind.IdentifierName:
                        ProcessBaseListIdentifierName(e.Type as IdentifierNameSyntax, item, usings, nameSpace);
                        break;

                    case SyntaxKind.QualifiedName:
                        ProcessBaseListQualifiedNameSyntax(e.Type as QualifiedNameSyntax, item, usings, nameSpace);
                        break;

                    case SyntaxKind.AliasQualifiedName:
                        ProcessBaseListAliasQualifiedNameSyntax(e.Type as AliasQualifiedNameSyntax, item, usings, nameSpace);
                        break;

                    default:
                        Trace.WriteLine($"Class {item.Key} implements/inherits {e.Type.ToFullString()}");
                        break;
                }
            });
        }

        private void ProcessInterface(InterfaceDeclarationSyntax syntaxNode, KeyValuePair<string, TypeInfo> item)
        {

        }
    }
}
