using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace HappyTesting.Editor {
    internal static partial class Generator {
        static readonly object parallellLock = new();
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
            var scripts = texts.Select(x => x.text).ToArray();
            List<(string code, string fileName)> generated = new();
            Parallel.ForEach(scripts, script => {
                    var param = LoadEditModeTestGenerateParam(script);
                    var fullText = GetEditModeTestFullText(param);
                    lock (parallellLock) {
                        generated.Add((fullText, $"{param.testClassName}.cs"));
                    }
                }
            );
            Debug.Log(string.Join(",", generated.Select(x => x.fileName)));
            GenerateScriptAsset(destDir, generated);
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
            var scripts = texts.Select(x => x.text).ToArray();
            List<(string code, string fileName)> generated = new();
            Parallel.ForEach(scripts, script => {
                    var param = LoadInterfaceMockGenerateParam(script);
                    var fullText = GetInterfaceTestMockFullText(param);
                    lock (parallellLock) {
                        generated.Add((fullText, $"{param.className}.cs"));
                    }
                }
            );
            GenerateScriptAsset(destDir, generated);
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

        static void GenerateScriptAsset(string saveTo, List<(string code, string fileName)> generated) {
            foreach (var (code, fileName) in generated) {
                var saveFullPath = Path.Combine(saveTo, fileName);
                File.WriteAllText(saveFullPath, code);
            }
            AssetDatabase.Refresh();
        }
    }
}
