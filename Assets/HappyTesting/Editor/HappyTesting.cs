using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HappyTesting.Editor {
    internal static partial class HappyTesting {
        [MenuItem("Assets/HappyTesting/Generate TestCode Template")]
        public static void GenerateTestTemplate() {
            var (success, assetPath) = TryGetTextContentFromSelectionObjects(out var code);
            if (!success) {
                Debug.LogWarning("selection is not C# script");
                return;
            }

            var param = LoadEditModeTestGenerateParam(code);
            var fullText = GetEditModeTestFullText(param);
            settings.Load();
            var destDir = settings.outputAssetPath;
            GenerateScriptAsset(fullText, destDir, $"{param.testClassName}.cs");
        }

        /*[MenuItem("Assets/HappyTesting/Generate Interface TestMock")]
        public static void GenerateTestMock() {
            var (success, assetPath) = TryGetTextContentFromSelectionObjects(out var code);
            if (!success) {
                Debug.LogWarning("selection is not C# script");
            }
        }*/

        static (bool succes, string assetPath) TryGetTextContentFromSelectionObjects(out string textContent) {
            var obj = Selection.GetFiltered(typeof(TextAsset), SelectionMode.TopLevel).FirstOrDefault();
            if (obj == null) {
                textContent = "";
                return (false, "");
            }

            var path = AssetDatabase.GetAssetPath(obj);
            var extension = Path.GetExtension(path);
            if (extension != ".cs") {
                textContent = "";
                return (false, "");
            }

            textContent = (obj as TextAsset)?.text ?? "";
            var success = !string.IsNullOrEmpty(textContent);
            return (success, success ? AssetDatabase.GetAssetPath(obj) : "");
        }

        static void GenerateScriptAsset(string content, string saveTo, string fileName) {
            var saveFullPath = Path.Combine(string.IsNullOrEmpty(saveTo) ? "Assets" : saveTo, fileName);
            File.WriteAllText(saveFullPath, content);
            AssetDatabase.Refresh();
        }
    }
}
