using System.Text;

namespace ElfScript.Compiler
{
    internal sealed class TokenTranslator {

        private readonly Queue<Token> _tokens = new();

        public void AddToken(Token token) {
            _tokens.Enqueue(token);
        }

        public Expression[] GenerateExpressions() {
            //todo
            return Array.Empty<Expression>();
        }

        public string GetTokenDisassembly() {
            var stringBuilder = new StringBuilder();
            foreach(var token in _tokens) {
                token.AppendToStringBuilder(stringBuilder);
            }
            return stringBuilder.ToString();
        }

        public static Expression[] GenerateExpressions(string[] lines,Action<string>? writeDisassembly = null) {
            var compiler = new TokenTranslator();
            var tokens = PreProcessor.GetTokens(lines);
            foreach(var token in tokens) {
                compiler.AddToken(token);
            }
            writeDisassembly?.Invoke(compiler.GetTokenDisassembly());
            return compiler.GenerateExpressions();
        }
    }
}
