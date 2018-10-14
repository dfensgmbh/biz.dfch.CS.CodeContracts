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

using System.Collections.Concurrent;
using System.Linq;
using biz.dfch.CS.Commons.Linq;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    public class ClassVisitor : CSharpSyntaxWalker
    {
        public ConcurrentBag<ClassDeclarationSyntax> Nodes { get; } = new ConcurrentBag<ClassDeclarationSyntax>();

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            Nodes.Add(node);

            // resolve nested classes separately
            node.DescendantNodes().AsParallel().ForEach(e =>
            {
                var visitor = new ClassVisitor();
                
                visitor.Visit(e);
                visitor.Nodes.ForEach(Nodes.Add);
            });
        }
    }
}
