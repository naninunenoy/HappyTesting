using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using UnityEngine;

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

        static (string namespaceName, string interfaceName, InterfaceDeclarationSyntax interfaceSyntax)
            GetFirstInterfaceMemberNames(CompilationUnitSyntax root) {
            var nameSpaceName = "";
            var firstInterfaceName = "";
            InterfaceDeclarationSyntax firstInterfaceDeclaration;
            var firstNamespaceDeclaration = GetNamespaceDeclarationSyntaxFromRoot(root);
            if (firstNamespaceDeclaration is null) {
                // namespaceなし
                firstInterfaceDeclaration = GetFirstInterfaceDeclarationSyntax(root);
            } else {
                nameSpaceName = firstNamespaceDeclaration.Name.ToString();
                firstInterfaceDeclaration = firstNamespaceDeclaration.Members
                    .FirstOrDefault(x => x.Kind() is SyntaxKind.InterfaceDeclaration) as InterfaceDeclarationSyntax;
            }

            firstInterfaceName = firstInterfaceDeclaration?.Identifier.ToString() ?? "";
            return (nameSpaceName, firstInterfaceName, firstInterfaceDeclaration);
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

        static InterfaceMockGenerateParam LoadInterfaceMockGenerateParam(string code) {
            var tree = CSharpSyntaxTree.ParseText(code);
            var root = (CompilationUnitSyntax)tree.GetRoot();
            // interfaceのusing一覧
            var usingArr = root.Usings.Select(x => x.ToString()).ToArray();
            // namespaceとクラス名取得
            var (orgNamespace, interfaceName, syntax) = GetFirstInterfaceMemberNames(root);
            var namespaceStr = string.IsNullOrEmpty(orgNamespace) ? "Tests" : $"{orgNamespace}.Tests";
            var className = $"{interfaceName.TrimStart('I')}TestMock";
            StringBuilder stringBuilder = new();
            foreach (var mem in syntax.Members) {
                var generate = "";
                if (mem is MethodDeclarationSyntax method) {
                    var retTypeName = method.ReturnType.ToString();
                    if (retTypeName == "void") {
                        // setter
                        generate = GetSetterPair(method.Identifier.ToString(),
                            method.ParameterList.Parameters.Select(x => (x.Type.ToString(), x.Identifier.ToString())).ToArray());
                    } else if (retTypeName == "UniTask") {
                        // async setter
                        generate = GetUniTaskSetterPair(method.Identifier.ToString(),
                            method.ParameterList.Parameters.Select(x => (x.Type.ToString(), x.Identifier.ToString())).ToArray());
                    } else if (retTypeName.Contains("<")) {
                        // async getter
                        var typeName = retTypeName.Replace("UniTask<", "")
                            .Replace(">", "");
                        generate = GetUniTaskGetterPair(method.Identifier.ToString(), typeName);
                    }
                } else if (mem is PropertyDeclarationSyntax prop) {
                    if (prop.Type.ToString().Contains("IReadOnlyReactiveProperty")) {
                        var typeName = prop.Type.ToString().Replace("IReadOnlyReactiveProperty<", "")
                            .Replace(">", "");
                        generate = GetIReadonlyReactivePropertyPair(typeName, prop.Identifier.ToString());
                    } else if (prop.Type.ToString().Contains("IObservable")) {
                        var typeName = prop.Type.ToString().Replace("IObservable<", "")
                            .Replace(">", "");
                        generate = GetObservablePair(typeName, prop.Identifier.ToString());
                    } else {
                        generate = GetPropertyAddSetter(prop.Identifier.ToString(), prop.Type.ToString());
                    }
                }
                stringBuilder.Append(generate);
            }

            return new InterfaceMockGenerateParam(usingArr, namespaceStr, className, interfaceName,
                stringBuilder.ToString());
        }
    }
}
