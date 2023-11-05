namespace ElfScript.Compiler {
    internal sealed class TokenBlock {
        public int ID { get; init; }
        public int ParentBlock { get; init; } = -1;
        public List<Token> Tokens { get; private init; } = new();
        public bool HasParent => ParentBlock >= 0;
    }
}
