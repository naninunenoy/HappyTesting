﻿using System;
using System.Linq;
using System.Text;

namespace HappyTesting.Editor {
    internal static partial class Generator {
        internal static string GetObservablePair(string typeName, string observableName) {
            var subjectName = $"{observableName.ToPascalCase()}Subject";
            return $"public IObservable<{typeName}> {observableName} => {subjectName};\n" +
                   $"public readonly Subject<{typeName}> {subjectName} = new();";
        }

        internal static string GetIReadonlyReactivePropertyPair(string typeName, string iReadOnlyReactivePropertyName) {
            var subjectName = $"{iReadOnlyReactivePropertyName.ToPascalCase()}Subject";
            return $"public IReadonlyReactiveProperty<{typeName}> {iReadOnlyReactivePropertyName} => {subjectName}.ToReadonlyReactiveProperty();\n" +
                   $"public readonly Subject<{typeName}> {subjectName} = new();";
        }

        internal static string GetResultName(string setterName, string argName) {
            return $"{setterName}{argName.ToPascalCase()}Result";
        }

        internal static string GetSetterArgs(string setterName, (string typeName, string argName)[] paramList) {
            return $"public void {setterName}(" +
                   $"{paramList.Select(x => $"{x.typeName} {x.argName}").Aggregate((prev, total) => $"{prev}, {total}")}" +
                   $")";
        }

        internal static string GetSetterBody(string setterName, (string typeName, string argName)[] paramList) {
            return paramList.Select(x => new { result = GetResultName(setterName, x.argName), x.argName })
                .Select(x => $"  {x.result} = {x.argName};")
                .Aggregate((prev, total) => $"{prev}\n{total}");
        }

        internal static string GetSetterResultList(string setterName, (string typeName, string argName)[] paramList) {
            return paramList
                .Select(x =>
                    $"public {x.typeName} {GetResultName(setterName, x.argName)} {{ private set; get; }}")
                .Aggregate((prev, total) => $"{prev}\n{total}");
        }

        internal static string GetSetterPair(string setterName, (string typeName, string argName)[] paramList) {
            StringBuilder stringBuilder = new();
            stringBuilder.Append($"{GetSetterResultList(setterName, paramList)}\n");
            stringBuilder.Append($"{GetSetterArgs(setterName, paramList)} {{\n");
            stringBuilder.Append($"{GetSetterBody(setterName, paramList)}\n");
            stringBuilder.Append($"}}");
            return stringBuilder.ToString();
        }

        internal static string ToPascalCase(this string x) {
            return char.ToUpperInvariant(x[0]) + x[1..];
        }

        internal static string ToCamelCase(this string x) {
            return char.ToLowerInvariant(x[0]) + x[1..];

        }
    }
}
