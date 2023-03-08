using ElfScript.Errors;

namespace ElfScript.Compiler {
    internal sealed class BlockDecoder {

        private TokenBlock _block;

        private int _blockIDCounter = 0;

        private readonly Dictionary<int,TokenBlock> _tokenBlocks = new();

        public BlockDecoder() {
            var root = new TokenBlock() { ID = _blockIDCounter++ };
            _block = root;
            _tokenBlocks.Add(root.ID,root);
        }

        private void OpenBlock(Token sourceToken) {
            TokenBlock childBlock = new() {
                ParentBlock = _block.ID,
                ID = _blockIDCounter += 1
            };
            _block.Tokens.Add(new() {
                Type = TokenType.BlockReference,
                Value = string.Empty,
                BlockReference = childBlock.ID,
                Line = sourceToken.Line,
                Column = sourceToken.Column,
            });
            _tokenBlocks[childBlock.ID] = childBlock;
            _block = childBlock;
        }

        private void CloseBlock(Token sourceToken) {
            if(!_block.HasParent) {
                /* Cannot escape the root block. */
                throw CompilerErrorFactory.UnexpectedBlockEnd(sourceToken.Line,sourceToken.Column);
            }
            _block = _tokenBlocks[_block.ParentBlock];
        }

        public void AddToken(Token token) {
            if(token.Type == TokenType.OpenBlock) {
                OpenBlock(token);
                return;
            }
            if(token.Type == TokenType.CloseBlock) {
                CloseBlock(token);
                return;
            }
            _block.Tokens.Add(token);
        }


        public IEnumerable<TokenBlock> GetBlocks() {
            return _tokenBlocks.Values;
        }

        public IEnumerable<TokenBlock> GetBlocks(out string disassembly) {
            disassembly = BlockTreeDisassembler.GetString(_tokenBlocks);
            return _tokenBlocks.Values;
        }
    }
}
