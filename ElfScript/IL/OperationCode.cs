namespace ElfScript.IL {
    internal readonly struct OperationCode {

        public OperationCode() {
            Type = OperationCodeType.Operation_NoOperation;
            V1 = -1;
            V2 = -1;
        }

        public readonly OperationCodeType Type { get; init; }

        public readonly int V1 { get; init; }
        public readonly int V2 { get; init; }
    }
}
