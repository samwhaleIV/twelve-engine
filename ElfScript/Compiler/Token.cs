namespace ElfScript.Compiler {
    internal sealed class Token {
        public string Value { get; init; } = string.Empty;
        public int Line { get; init; } = 0;
        public int Column { get; init; } = 0;
        public TokenType Type { get; init; } = 0;
    }
}
