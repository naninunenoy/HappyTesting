using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;

namespace HappyTesting.Editor {
    internal static partial class HappyTesting {
        static readonly HappyTestingSettingsPrefs settings = new();
        static TextField outputAssetPathTextField;

        [SettingsProvider]
        public static SettingsProvider Create() {
            // 入力欄作成
            settings.Load();
            outputAssetPathTextField = new TextField("output path (MUST begin from \"Assets/\")");
            outputAssetPathTextField.value = settings.outputAssetPath;
            // 既存の設定値
            var provider = new SettingsProvider("HappyTesting/", SettingsScope.User) {
                // タイトル
                label = "HappyTesting",
                // 初期化
                activateHandler = (searchContext, rootElement) => {
                    rootElement.Add(outputAssetPathTextField);
                },
                // 終了
                deactivateHandler = () => {
                    settings.outputAssetPath = outputAssetPathTextField.value;
                    settings.Save();
                },
                // 検索時のキーワード
                keywords = new HashSet<string>(new[] {"Test"})
            };
            return provider;
        }
    }

    internal class HappyTestingSettingsPrefs {
        const string keyPrefix = "n5y_HappyTestingSettings";
        public string outputAssetPath = "";

        public void Save() {
            EditorPrefs.SetString($"{keyPrefix}_outputAssetPath", outputAssetPath);
        }

        public void Load() {
            outputAssetPath = EditorPrefs.GetString($"{keyPrefix}_outputAssetPath", "Assets");
        }
    }
}
