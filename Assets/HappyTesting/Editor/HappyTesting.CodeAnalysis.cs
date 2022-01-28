using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HappyTesting.Editor {
    internal static partial class HappyTesting {
        static EditModeTestGenerateParam LoadEditModeTestGenerateParam(string code) {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            // テスト対象のusing一覧
            var usingArr = new HashSet<string>(
                root.Usings.Select(x => x.ToString()).Concat(editModeTemplateUsingSet)
            ).ToArray();
            Array.Sort(usingArr);

            var namespaceStr = "";
            MemberDeclarationSyntax firstClassOrStructDeclaration;
            var firstNamespaceDeclaration = root.Members
                .FirstOrDefault(x => x.Kind() is SyntaxKind.NamespaceDeclaration);
            if (firstNamespaceDeclaration is NamespaceDeclarationSyntax namespaceDeclarationSyntax) {
                namespaceStr = $"{namespaceDeclarationSyntax.Name}.Tests";
                firstClassOrStructDeclaration = namespaceDeclarationSyntax.Members
                    .FirstOrDefault(x => x.Kind() is SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration);
            } else {
                // namespaceなし
                namespaceStr = "Tests";
                firstClassOrStructDeclaration = root.Members
                    .FirstOrDefault(x => x.Kind() is SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration);
            }

            // テスト対象のクラス名
            var className = "";
            if (firstClassOrStructDeclaration is null) {
                className = "//UnknownClass";
            } else {
                className = firstClassOrStructDeclaration switch {
                    StructDeclarationSyntax x => x.Identifier.Text,
                    ClassDeclarationSyntax x => x.Identifier.Text
                };
            }

            return new EditModeTestGenerateParam(usingArr, namespaceStr, className, $"{className}Test");
        }
    }
}
