using System.Text;

namespace ElfScript.Compiler {

    internal abstract class BlockTreeDisassembler {

        protected virtual void Start() { }
        protected virtual void End() { }
        protected abstract void WriteBlockStart(Token token,int depth);
        protected abstract void WriteToken(Token token,int depth);
        protected virtual void WriteNullToken(int depth) { }
        protected abstract void WriteBlockEnd(Token token,int depth);

        private readonly Stack<(TokenBlock Value, int TokenIndex)> _parentStack = new();
        private readonly Dictionary<int,TokenBlock> _blocks;

        public BlockTreeDisassembler(Dictionary<int,TokenBlock> blocks) {
            _blocks = blocks;
            _block = _blocks[0];
        }

        private TokenBlock _block;

        private int _tokenIndex = 0, _depth = 0;

        private void PushStack(int blockReference) {
            _parentStack.Push((_block, _tokenIndex));
            _block = _blocks[blockReference];
            _tokenIndex = 0;
            _depth += 1;
        }

        private void PopStack() {
            var (Value, TokenIndex) = _parentStack.Pop();
            _block = Value;
            _tokenIndex = TokenIndex;
            _depth -= 1;
        }
        
        /// <summary>
        /// Reference cycles are not detected. Do not use circular block trees.
        /// </summary>
        public void Disassemble() {
            Start();
            bool terminated = false;
            while(!terminated) {
                int tokenIndex = _tokenIndex++;
                if(_block.Tokens.Count <= 0) {
                    WriteNullToken(_depth);
                } else {
                    var token = _block.Tokens[tokenIndex];
                    if(token.Type == TokenType.BlockReference) {
                        WriteBlockStart(token,_depth);
                        PushStack(token.BlockReference);
                        continue;
                    }
                    WriteToken(token,_depth);
                }
                while(_tokenIndex >= _block.Tokens.Count) {
                    if(_parentStack.Count < 1) {
                        terminated = true;
                        break;
                    }
                    PopStack();
                    WriteBlockEnd(_block.Tokens[^1],_depth);
                }
            }
            End();
        }

        public static string GetString(Dictionary<int,TokenBlock> blocks) {
            StringDisassembler disassembler = new(blocks);
            disassembler.Disassemble();
            return disassembler.StringBuilder.ToString();
        }

        private sealed class StringDisassembler:BlockTreeDisassembler {

            public StringDisassembler(Dictionary<int,TokenBlock> blocks) : base(blocks) { }

            public StringBuilder StringBuilder { get; private init; } = new StringBuilder();
            public int IndentationSize { get; set; } = 4;

            private void WriteIndentation(int depth) {
                StringBuilder.Append(string.Empty.PadLeft(depth * IndentationSize));
            }

            protected override void WriteBlockEnd(Token token,int depth) {
                WriteIndentation(depth);
                StringBuilder.Append($"}} (Line: {token.Line}, Column: {token.Column})");
                StringBuilder.AppendLine();
            }

            protected override void WriteBlockStart(Token token,int depth) {
                WriteIndentation(depth);
                StringBuilder.Append($"Block (Line: {token.Line}, Column: {token.Column}): {{");
                StringBuilder.AppendLine();
            }

            protected override void WriteToken(Token token,int depth) {
                WriteIndentation(depth);
                StringBuilder.AppendLine(token.ToString());
            }

            protected override void WriteNullToken(int depth) {
                WriteIndentation(depth);
                StringBuilder.AppendLine("< Empty Block >");
            }
        }
    }
}
