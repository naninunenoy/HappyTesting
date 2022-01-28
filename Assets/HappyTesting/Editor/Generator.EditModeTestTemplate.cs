using System.Collections.Generic;

namespace HappyTesting.Editor {
    internal static partial class Generator {
        const string editModeTemplateUsingTag = "<#=HERE_IS_USING#>";
        const string editModeTemplateNamespaceTag = "<#=HERE_IS_NAMESPACE#>";
        const string editModeTemplateTestClassNameTag = "<#=HERE_IS_TESTCLASSNAME#>";
        const string editModeTemplateClassNameTag = "<#=HERE_IS_CLASSNAME#>";
        readonly static HashSet<string> editModeTemplateUsingSet = new() {
            "using System.Collections;",
            "using System.Collections.Generic;",
            "using NUnit.Framework;",
            "using UnityEngine;",
            "using UnityEngine.TestTools;",
        };

        static string GetEditModeTestFullText(EditModeTestGenerateParam param) {
            return editModeTestTemplateText
                .Replace(editModeTemplateUsingTag, string.Join("\n", param.usingList))
                .Replace(editModeTemplateNamespaceTag, param.namespaceName)
                .Replace(editModeTemplateClassNameTag, param.className)
                .Replace(editModeTemplateTestClassNameTag, param.testClassName);
        }

        const string editModeTestTemplateText = @"<#=HERE_IS_USING#>

namespace <#=HERE_IS_NAMESPACE#>
{
    public class <#=HERE_IS_TESTCLASSNAME#>
    {
        <#=HERE_IS_CLASSNAME#> x = default;
        // A Test behaves as an ordinary method
        [Test]
        public void <#=HERE_IS_TESTCLASSNAME#>SimplePasses()
        {
            // Use the Assert class to test conditions
        }

        // A UnityTest behaves like a coroutine in Play Mode. In Edit Mode you can use
        // `yield return null;` to skip a frame.
        [UnityTest]
        public IEnumerator <#=HERE_IS_TESTCLASSNAME#>WithEnumeratorPasses()
        {
            // Use the Assert class to test conditions.
            // Use yield to skip a frame.
            yield return null;
        }
    }
}";
    }
}
