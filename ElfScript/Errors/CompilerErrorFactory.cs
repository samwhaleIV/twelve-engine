namespace ElfScript.Errors {
    internal static class CompilerErrorFactory {
        private static ElfScriptCompilerException Create(string type,int line,int column,string message) {
            return new($"{type}: {message} {{ Line: {line + 1}, Column: {column} }}");
        }
        public static ElfScriptCompilerException UnexpectedSymbol(int line,int column,char symbol) {
            return Create(nameof(UnexpectedSymbol),line,column,$"Unexpected symbol '{symbol}'.");
        }
        public static ElfScriptCompilerException InvalidStringEscapeCharacter(int line,int column,char symbol) {
            return Create(nameof(InvalidStringEscapeCharacter),line,column,$"Invalid string escape character '{symbol}'.");
        }
        public static ElfScriptCompilerException StatementIsEmpty(int line,int column) {
            return Create(nameof(StatementIsEmpty),line,column,"Statement is empty. A statement must contain at least one token.");
        }
        public static ElfScriptCompilerException ExpectedStatementEnd(int line,int column) {
            return Create(nameof(ExpectedStatementEnd),line,column,$"Expected statement end. Must use the terminating symbol '{Symbols.StatementEnd}'.");
        }
        public static ElfScriptCompilerException UnexpectedBlockEnd(int line,int column) {
            return Create(nameof(UnexpectedBlockEnd),line,column,$"Unxpected end of block. Are you missing an opening block symbol '{Symbols.OpenBlock}'?");
        }
        public static ElfScriptCompilerException BadNumberToken(int line,int column,string value) {
            return Create(nameof(BadNumberToken),line,column,$"Bad number token. Value '{value}' is an invalid number.");
        }
        public static ElfScriptCompilerException UnexpectedSymbolInNumber(int line,int column,char symbol) {
            return Create(nameof(UnexpectedSymbolInNumber),line,column,$"Unexpected symbol in number token '{symbol}'.");
        }
        public static ElfScriptCompilerException UnexpectedNewLineInEscapeSequence(int line,int column) {
            return Create(nameof(UnexpectedNewLineInEscapeSequence),line,column,"Unexpected new line in escape sequence.");
        }
        public static ElfScriptCompilerException IllegalTrailingDecimalSymbol(int line,int column) {
            return Create(nameof(IllegalTrailingDecimalSymbol),line,column,"A numerical constant cannot contain a trailing decimal symbol.");
        }
        public static ElfScriptCompilerException UnknownOperator(int line,int column,string value) {
            return Create(nameof(UnknownOperator),line,column,$"Unknown operator '{value}'.");
        }
    }
}
