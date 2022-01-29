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
            // 保存先
            settings.Load();
            var destDir = GetOutputPathFromDialog(settings.outputAssetPath);
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
            // 保存先
            settings.Load();
            var destDir = GetOutputPathFromDialog(settings.outputAssetPath);
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
                .Cast<TextAsset>().ToArray();
            return scripts is { Length: > 0 };
        }

        static string GetOutputPathFromDialog(string defaultOutputPath) {
            return EditorUtility.SaveFolderPanel("select destination", defaultOutputPath, "");
        }

        static bool TryGetTextContentFromSelectionObjects(out string textContent) {
            var obj = Selection.GetFiltered(typeof(TextAsset), SelectionMode.TopLevel).FirstOrDefault();
            if (obj == null) {
                textContent = "";
                return false;
            }

            var path = AssetDatabase.GetAssetPath(obj);
            var extension = Path.GetExtension(path);
            if (extension != ".cs") {
                textContent = "";
                return false;
            }

            textContent = (obj as TextAsset)?.text ?? "";
            return !string.IsNullOrEmpty(textContent);
        }

        static void GenerateScriptAsset(string content, string saveTo, string fileName) {
            var saveFullPath = Path.Combine(string.IsNullOrEmpty(saveTo) ? "Assets" : saveTo, fileName);
            File.WriteAllText(saveFullPath, content);
        }
    }
}
