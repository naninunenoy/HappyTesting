namespace HappyTesting.Editor {
    internal readonly struct InterfaceMockGenerateParam {
        public readonly string[] usingList;
        public readonly string namespaceName;
        public readonly string className;
        public readonly string interfaceName;
        public readonly string body;

        public InterfaceMockGenerateParam(string[] usingList, string namespaceName, string className,
            string interfaceName, string body) {
            this.usingList = usingList;
            this.namespaceName = namespaceName;
            this.className = className;
            this.interfaceName = interfaceName;
            this.body = body;
        }
    }
}
