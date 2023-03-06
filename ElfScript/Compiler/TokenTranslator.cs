namespace ElfScript.Compiler {
    internal sealed class TokenTranslator {

        private readonly Queue<Token> _tokens = new();

        public void AddToken(Token token) {
            _tokens.Enqueue(token);
        }

        public Expression[] Export() {
            throw new NotImplementedException();
        }

        public static Expression[] GenerateExpressions(string[] lines) {
            var compiler = new TokenTranslator();
            var tokens = PreProcessor.GetTokens(lines);
            foreach(var token in tokens) {
                compiler.AddToken(token);
            }
            return compiler.Export();
        }
    }
}
