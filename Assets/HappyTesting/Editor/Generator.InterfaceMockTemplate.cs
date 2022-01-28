using System.Linq;
using System.Text;

namespace HappyTesting.Editor {
    internal static partial class Generator {
        const string interfaceMockGenericsTypeName = "<#=TYPE_NAME#>";
        const string interfaceMockTemplateIReadonlyReactivePropertyTag = "<#=IREADONLYREACTIVEPROPERTY_NAME#>";
        const string interfaceMockTemplateSetterTag = "<#=SETTER_NAME#>";
        const string interfaceMockTemplateUniTaskTag = "<#=UNITASK_NAME#>";
        const string interfaceMockTemplateTaskTag = "<#=TASK_NAME#>";

        internal static string GetObservablePair(string typeName, string observableName) {
            var subjectName = $"{observableName}Subject";
            return $"public IObservable<{typeName}> {observableName} => {subjectName};\n" +
                   $"public readonly Subject<{typeName}> {subjectName} = new();";
        }

        internal static string GetSetterPair(string setterName, (string typeName, string argName)[] paramList) {
            StringBuilder stringBuilder = new();
            var args = paramList.Select(x => $"{x.typeName} {x.argName}").Aggregate((a, b) => $"{a}, {b}");
            foreach (var (typeName, argName) in paramList) {
                var resultName = $"{setterName}{argName}Result";
                var setter = $"public void {setterName}({args}) {{" +
                             $"}}";
            }

            string getResultStr(string typeName, string argName) {
                return $"public {typeName} {setterName}{argName}Result;";
            }

            return stringBuilder.ToString();
        }
    }
}
