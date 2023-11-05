namespace ElfScript.Compiler.ILGen {
    internal readonly struct ExpressionPatternItem {

        public readonly bool ExpressionWildcard { get; init; }
        public readonly string MatchValue { get; init; }
        public readonly TokenType[] MatchTypes { get; init; }

        public ExpressionPatternItem(TokenType matchType,string? matchValue = null) {
            MatchValue = matchValue ?? string.Empty;
            MatchTypes = new TokenType[] { matchType };
            ExpressionWildcard = false;
        }

        public ExpressionPatternItem(TokenType[] matchTypes,string? matchValue = null) {
            MatchValue = matchValue ?? string.Empty;
            MatchTypes = matchTypes;
            ExpressionWildcard = false;
        }

        public static readonly ExpressionPatternItem ExpressionWildCard = new() {
            ExpressionWildcard = true,
            MatchValue = string.Empty,
            MatchTypes = Array.Empty<TokenType>()
        };
    }
}
