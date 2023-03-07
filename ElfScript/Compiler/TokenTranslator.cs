using System.Text;

namespace ElfScript.Compiler {
    internal sealed class TokenTranslator {

        private readonly Queue<Token> _tokens = new();

        public void AddToken(Token token) {
            _tokens.Enqueue(token);
        }

        public static string GetTokenDisassembly(IEnumerable<Token> tokens) {
            var stringBuilder = new StringBuilder();
            foreach(var token in tokens) {
                token.AppendToStringBuilder(stringBuilder);
            }
            return stringBuilder.ToString();
        }

        public static IEnumerable<Token> GenerateTokens(string[] lines) {
            var compiler = new TokenTranslator();
            var tokens = PreProcessor.GetTokens(lines);
            foreach(var token in tokens) {
                compiler.AddToken(token);
            }
            return tokens;
        }
    }
}
