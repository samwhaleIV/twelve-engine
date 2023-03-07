namespace ElfScript.Compiler {
    internal static class PreProcessor {
        public static IEnumerable<Token> GetTokens(string[] lines) {
            var generator = new TokenGenerator();
            for(int lineNumber = 0;lineNumber<lines.Length;lineNumber++) {
                var line = lines[lineNumber];
                if(line.StartsWith(Symbols.LineComment)) {
                    continue;
                }
                for(int column = 0;column<line.Length;column++) {
                    var character = line[column];
                    generator.AddCharacter(character,lineNumber,column);
                }
                generator.AddLineBreak(lineNumber);
            }
            return generator.Export();
        }
    }
}
