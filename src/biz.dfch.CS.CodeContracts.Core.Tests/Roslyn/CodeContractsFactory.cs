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

using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace biz.dfch.CS.CodeContracts.Core.Tests.Roslyn
{
    public static class CodeContractsFactory
    {
        // http://roslynquoter.azurewebsites.net/
        public static CSharpSyntaxNode AssertNotNull(string identifier)
        {
            // Trace.Assert(null != f); /// f -- > identifier
            return SyntaxFactory.CompilationUnit()
.WithMembers(
    SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.IndexerDeclaration(
            SyntaxFactory.IdentifierName(
                SyntaxFactory.MissingToken(
                    SyntaxFactory.TriviaList
                    (new[]{
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Identifier("System")))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.DotToken)))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Identifier("Diagnostics")))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.DotToken)))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Identifier("Trace")))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.DotToken)))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Identifier("Assert")))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.OpenParenToken)))),
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.NullKeyword)))),
                            SyntaxFactory.Space,
                            SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.ExclamationEqualsToken)))),
                            SyntaxFactory.Space,
                    }),
                    SyntaxKind.IdentifierToken,
                    SyntaxFactory.TriviaList())))
        .WithParameterList(
            SyntaxFactory.BracketedParameterList()
            .WithOpenBracketToken(
                SyntaxFactory.MissingToken(SyntaxKind.OpenBracketToken))
            .WithCloseBracketToken(
                SyntaxFactory.MissingToken(SyntaxKind.CloseBracketToken)))
        .WithAccessorList(
            SyntaxFactory.AccessorList(
                SyntaxFactory.SingletonList<AccessorDeclarationSyntax>(
                    SyntaxFactory.AccessorDeclaration(
                        SyntaxKind.UnknownAccessorDeclaration)
                    .WithSemicolonToken(
                        SyntaxFactory.Token(
                            SyntaxFactory.TriviaList(),
                            SyntaxKind.SemicolonToken,
                            SyntaxFactory.TriviaList(
                                SyntaxFactory.LineFeed)))))
            .WithOpenBraceToken(
                SyntaxFactory.MissingToken(
                    SyntaxFactory.TriviaList(),
                    SyntaxKind.OpenBraceToken,
                    SyntaxFactory.TriviaList(
                        SyntaxFactory.Trivia(SyntaxFactory.SkippedTokensTrivia().WithTokens(SyntaxFactory.TokenList(SyntaxFactory.Token(SyntaxKind.CloseParenToken)))))))
            .WithCloseBraceToken(
                SyntaxFactory.MissingToken(SyntaxKind.CloseBraceToken)))));
        }
    }
}
