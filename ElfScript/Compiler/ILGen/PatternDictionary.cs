using static ElfScript.Symbols.Operators;

namespace ElfScript.Compiler.ILGen {
    internal sealed class PatternDictionary {
        private readonly List<ExpressionPattern> _patterns = new();

        public PatternDictionary() {
            _patterns.Add(
                new(new(TokenType.Generic),new(TokenType.Operator,Assignment.ToString()),ExpressionPatternItem.ExpressionWildCard)
            );
        }
    }
}
