using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace HappyTesting.Editor {
    internal static partial class Generator {

        static NamespaceDeclarationSyntax GetNamespaceDeclarationSyntaxFromRoot(CompilationUnitSyntax root) {
            var found = root.Members
                .FirstOrDefault(x => x.Kind() is SyntaxKind.NamespaceDeclaration);
            return found as NamespaceDeclarationSyntax;
        }

        static InterfaceDeclarationSyntax GetFirstInterfaceDeclarationSyntax(CompilationUnitSyntax root) {
            var found = root.Members
                .FirstOrDefault(x => x.Kind() is SyntaxKind.InterfaceDeclaration);
            return found as InterfaceDeclarationSyntax;
        }

        static (string namespaceName, string firstClassOrStructName) GetFirstClassMemberNames(CompilationUnitSyntax root) {
            var nameSpaceName = "";
            var firstClassOrStructName = "";
            MemberDeclarationSyntax firstClassOrStructDeclaration;
            var firstNamespaceDeclaration = GetNamespaceDeclarationSyntaxFromRoot(root);
            if (firstNamespaceDeclaration is null) {
                // namespaceなし
                firstClassOrStructDeclaration = root.Members
                    .FirstOrDefault(x => x.Kind() is SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration);
            } else {
                nameSpaceName = firstNamespaceDeclaration.Name.ToString();
                firstClassOrStructDeclaration = firstNamespaceDeclaration.Members
                    .FirstOrDefault(x => x.Kind() is SyntaxKind.ClassDeclaration or SyntaxKind.StructDeclaration);
            }

            if (firstClassOrStructDeclaration is not null) {
                firstClassOrStructName = firstClassOrStructDeclaration switch {
                    StructDeclarationSyntax x => x.Identifier.Text,
                    ClassDeclarationSyntax x => x.Identifier.Text
                };
            }

            return (nameSpaceName, firstClassOrStructName);
        }
        static EditModeTestGenerateParam LoadEditModeTestGenerateParam(string code) {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            // テスト対象のusing一覧
            var usingArr = new HashSet<string>(
                root.Usings.Select(x => x.ToString()).Concat(editModeTemplateUsingSet)
            ).ToArray();
            Array.Sort(usingArr);
            // namespaceとクラス名取得
            var (orgNamespace, orgClass) = GetFirstClassMemberNames(root);
            var namespaceStr = string.IsNullOrEmpty(orgNamespace) ? "Tests" : $"{orgNamespace}.Tests";
            var className = string.IsNullOrEmpty(orgClass) ? "//UnknownClass" : orgClass;
            return new EditModeTestGenerateParam(usingArr, namespaceStr, className, $"{className}Test");
        }
    }
}
