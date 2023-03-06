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
    }
}
