using ElfScript.Errors;
using System.Reflection.PortableExecutable;
using System.Text;
using static ElfScript.Symbols;

namespace ElfScript.Compiler {
    internal sealed class TokenGenerator {

        private readonly Queue<Token> _tokenQueue = new();
        private readonly StringBuilder _tokenBuilder = new();

        private int _line = -1, _column = -1, _tokenColumnStart = -1;
        private bool _writingString = false, _writingEscapeSequence = false;

        private readonly HashSet<char> _operators = new() {
            AddOperator, SubtractOperator, DivideOperator, MultiplyOperator, DotOperator, CommaOperator
        };

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
            Column = _tokenColumnStart,Line = _line,Type = type,Value = value.ToString(),
        });

        private void FlushTokenBuilder() {
            if(_tokenBuilder.Length < 1) {
                return;
            }
            EnqueueToken(_tokenBuilder.ToString(),_writingString ? TokenType.String : TokenType.Generic);
            _tokenBuilder.Clear();
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

        public void AddCharacter(char character,int line,int column) {
            _line = line;
            _column = column;
            if(!_writingString) {
                if(character == Whitespace) {
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

        public void AddLineBreak() {
            if(_writingString) {
                return;
            }
            FlushTokenBuilder();
        }

        public IEnumerable<Token> Export() {
            FlushTokenBuilder();
            return _tokenQueue;
        }
    }
}
