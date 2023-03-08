using ElfScript.Errors;
using ElfScript.VirtualMachine.Operations;
using System.Text;
using static ElfScript.Symbols;

namespace ElfScript.Compiler {
    internal sealed class TokenDecoder {

        private readonly Queue<Token> _tokenQueue = new();
        private readonly StringBuilder _tokenBuilder = new();

        private int _line = -1, _column = -1, _tokenColumnStart = -1;
        private bool _writingString = false, _writingEscapeSequence = false;

        private readonly HashSet<char> _operators = new() {
            AddOperator, SubtractOperator, DivideOperator, MultiplyOperator, DotOperator, CommaOperator, EqualsOperator
        };

        private readonly HashSet<char> _digits = new() { '0','1','2','3','4','5','6','7','8','9' };
        private readonly HashSet<char> _whitespace = new() { ' ', '\t', '\v' };

        private readonly Dictionary<char,TokenType> _symbolTokens = new() {
            { OpenBlock, TokenType.OpenBlock },
            { CloseBlock, TokenType.CloseBlock },
            { OpenTuple, TokenType.OpenTuple },
            { CloseTuple, TokenType.CloseTuple },
            { OpenArray, TokenType.OpenArray },
            { CloseArray, TokenType.CloseArray },
            { StatementEnd, TokenType.StatementEnd }
        };

        private void EnqueueToken(string value,TokenType type) => _tokenQueue.Enqueue(new() {
            Column = _tokenColumnStart,Line = _line,Type = type,Value = value
        });

        private void EnqueueToken(char value,TokenType type) => _tokenQueue.Enqueue(new() {
            Column = _column,Line = _line,Type = type,Value = value.ToString(),
        });

        private bool IsNumber(string token) {
            if(token.Length <= 0) {
                return false;
            }
            bool isNumber = true;
            foreach(var character in token) {
                if(!_digits.Contains(character)) {
                    isNumber = false;
                    break;
                }
            }
            return isNumber;
        }

        private TokenType GetTokenType(string tokenValue) {
            if(_writingString) {
                return TokenType.String;
            }
            if(IsNumber(tokenValue)) {
                return TokenType.Number;
            }
            return TokenType.Generic;
        }

        private void FlushTokenBuilder() {
            if(_tokenBuilder.Length < 1) {
                return;
            }
            string tokenValue = _tokenBuilder.ToString();
            EnqueueToken(tokenValue,GetTokenType(tokenValue));
            _tokenBuilder.Clear();
            _tokenColumnStart = -1;
        }

        private void AppendTokenBuilder(char character) {
            if(_tokenBuilder.Length < 1) {
                _tokenColumnStart = _column;
            }
            _tokenBuilder.Append(character);
        }

        private void AddCharacterDefault(char character) {
            if(_symbolTokens.TryGetValue(character,out var tokenType)) {
                FlushTokenBuilder();
                EnqueueToken(character,tokenType);
                return;
            }
            if(_operators.Contains(character)) {
                FlushTokenBuilder();
                EnqueueToken(character,TokenType.Operator);
                return;
            }
            if(character == StringEscape) {
                throw CompilerErrorFactory.UnexpectedSymbol(_line,_column,character);
            }
            if(character == StringLimit) {
                FlushTokenBuilder();
                _writingString = true;
                return;
            }
            AppendTokenBuilder(character);
        }

        private void AddCharacterString(char character) {
            if(character == StringEscape) {
                _writingEscapeSequence = true;
                return;
            }
            if(character == StringLimit) {
                FlushTokenBuilder();
                _writingString = false;
                return;
            }
            AppendTokenBuilder(character);
        }

        private void AddCharacterEscapeSequence(char character) {
            if(character != StringEscape && character != StringLimit) {
                throw CompilerErrorFactory.InvalidStringEscapeCharacter(_line,_column,character);
            }
            AppendTokenBuilder(character);
            _writingEscapeSequence = false;
        }

        private void AddCharacter(char character,int line,int column) {
            _line = line;
            _column = column;
            if(!_writingString) {
                if(_whitespace.Contains(character)) {
                    FlushTokenBuilder();
                    return;
                }
                AddCharacterDefault(character);
                return;
            }
            if(!_writingEscapeSequence) {
                AddCharacterString(character);
                return;
            }
            AddCharacterEscapeSequence(character);
        }

        private void AddLineBreak(int line) {
            _line = line;
            _column = -1;
            if(_writingString) {
                return;
            }
            FlushTokenBuilder();
        }

        public IEnumerable<TokenBlock> Export() {
            var blockReader = new BlockDecoder();
            foreach(var token in _tokenQueue) {
                blockReader.AddToken(token);
            }
            return blockReader.GetBlocks();
        }

        public IEnumerable<TokenBlock> Export(out string disassembly) {
            var blockReader = new BlockDecoder();
            foreach(var token in _tokenQueue) {
                blockReader.AddToken(token);
            }
            return blockReader.GetBlocks(out disassembly);
        }

        public void Import(string[] lines) {
            for(int lineNumber = 0;lineNumber<lines.Length;lineNumber++) {
                var line = lines[lineNumber];
                if(line.StartsWith(LineComment)) {
                    continue;
                }
                for(int column = 0;column<line.Length;column++) {
                    var character = line[column];
                    AddCharacter(character,lineNumber,column);
                }
                AddLineBreak(lineNumber);
            }
        }
    }
}
