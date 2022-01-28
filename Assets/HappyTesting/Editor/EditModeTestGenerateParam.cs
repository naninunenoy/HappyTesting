namespace HappyTesting.Editor {
    internal readonly struct EditModeTestGenerateParam {
        public readonly string[] usingList;
        public readonly string namespaceName;
        public readonly string className;
        public readonly string testClassName;

        public EditModeTestGenerateParam(string[] usingList, string namespaceName, string className, string testClassName) {
            this.usingList = usingList;
            this.namespaceName = namespaceName;
            this.className = className;
            this.testClassName = testClassName;
        }
    }
}
