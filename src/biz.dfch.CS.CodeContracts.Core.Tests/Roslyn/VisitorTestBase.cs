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

using System.IO;
using biz.dfch.CS.CodeContracts.Core.Assertions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    public abstract class VisitorTestBase : IVisitorTest
    {
        private readonly object syncRoot = new object();

        public const string FILE_NAME_DEFAULT = @"..\..\Roslyn\CSharpSyntaxWalkerSource.cs";

        public bool IsInitialised { get; private set; }
        public FileInfo FileInfo { get; private set; }
        public string Source { get; private set; }
        public SyntaxTree Tree { get; private set; }
        public CompilationUnitSyntax Root { get; private set; }

        public IVisitorTest Initialise(string fileName)
        {
            Constraint.Requires(File.Exists(fileName));

            if (IsInitialised) return this;

            lock (syncRoot)
            {
                if (IsInitialised) return this;

                FileInfo = new FileInfo(fileName);
                Source = File.ReadAllText(FileInfo.FullName);
                Tree = CSharpSyntaxTree.ParseText(Source);
                Root = Tree.GetCompilationUnitRoot();
                IsInitialised = null != Root;

                return this;
            }
        }

        public IVisitorTest Reset()
        {
            if (!IsInitialised) return this;

            lock (syncRoot)
            {
                if (!IsInitialised) return this;

                FileInfo = default(FileInfo);
                Tree = default(SyntaxTree);
                Root = default(CompilationUnitSyntax);
                IsInitialised = null != Root;

                return this;
            }
        }
    }
}
