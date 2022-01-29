using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace HappyTesting.Editor {
    internal static partial class Generator {
        [MenuItem("Assets/HappyTesting/Generate TestCode Template")]
        public static void GenerateTestTemplate() {
            // Selectionがら対象取得
            if (!ContainsScriptInSelection(out var texts)) {
                Debug.LogWarning("selection is not C# script");
                return;
            }
            var destDir = GetOutputPathFromDialog();
            if (string.IsNullOrEmpty(destDir)) {
                return;
            }
            // コードの生成
            foreach (var text in texts) {
                var param = LoadEditModeTestGenerateParam(text.text);
                var fullText = GetEditModeTestFullText(param);
                GenerateScriptAsset(fullText, destDir, $"{param.testClassName}.cs");
            }
            AssetDatabase.Refresh();
        }

        [MenuItem("Assets/HappyTesting/Generate Interface TestMock")]
        public static void GenerateTestMock() {
            // Selectionがら対象取得
            if (!ContainsScriptInSelection(out var texts)) {
                Debug.LogWarning("selection is not C# script");
            }
            var destDir = GetOutputPathFromDialog();
            if (string.IsNullOrEmpty(destDir)) {
                return;
            }
            // コードの生成
            foreach (var text in texts) {
                var param = LoadInterfaceMockGenerateParam(text.text);
                var fullText = GetInterfaceTestMockFullText(param);
                GenerateScriptAsset(fullText, destDir, $"{param.className}.cs");
            }
            AssetDatabase.Refresh();
        }

        static bool ContainsScriptInSelection(out TextAsset[] scripts) {
            var textAssets = Selection.GetFiltered(typeof(TextAsset), SelectionMode.TopLevel);
            if (textAssets == null || textAssets.Length == 0) {
                scripts = default;
                return false;
            }

            scripts = textAssets
                .Where(x => Path.GetExtension(AssetDatabase.GetAssetPath(x)) == ".cs")
                .Cast<TextAsset>()
                .ToArray();
            return scripts is { Length: > 0 };
        }

        static string GetOutputPathFromDialog() {
            return EditorUtility.SaveFolderPanel("select destination", Application.dataPath, "");
        }

        static void GenerateScriptAsset(string content, string saveTo, string fileName) {
            var saveFullPath = Path.Combine(string.IsNullOrEmpty(saveTo) ? "Assets" : saveTo, fileName);
            File.WriteAllText(saveFullPath, content);
        }
    }
}
