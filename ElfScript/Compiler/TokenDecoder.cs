using System.Text;
using ElfScript.Errors;
using static ElfScript.Symbols;
using static ElfScript.Symbols.Operators;
using static ElfScript.Symbols.Keywords;

namespace ElfScript.Compiler {
    internal sealed class TokenDecoder {

        private readonly Queue<Token> _tokenQueue = new();
        private readonly StringBuilder _tokenBuilder = new();

        private int _line = -1, _column = -1, _tokenColumnStart = -1, _decimalSymbolCount = 0;

        private readonly HashSet<string>
            _operators = new() {
                Parameter, Assignment, Add, Subtract, Divide, Multiply,
                Comma, Not, GreaterThan, LessThan, GreaterThanOrEqual, LessThanOrEqual, And, Or,
                AddAssignment, SubtractAssignment, MultiplyAssignment, DivideAssignment, ModulusAssignment, Modulus
            },
            _keyWords = new() {
                Delete, Exec, If
            };

        public TokenDecoder() {
            foreach(string value in _operators) {
                foreach(char character in value) {
                    _operatorSymbols.Add(character);
                }
            }
        }

        private readonly HashSet<char>
            _digits = new() { '0','1','2','3','4','5','6','7','8','9', DecimalSymbol },
            _whitespace = new() { ' ', '\t', '\v' },
            _escapeSequenceOperands = new() { '\\', NewLineCharacter, '"' },
            _operatorSymbols = new();

        private readonly Dictionary<char,TokenType> _symbolTokens = new() {
            { OpenBlock, TokenType.OpenBlock }, { CloseBlock, TokenType.CloseBlock },
            { OpenTuple, TokenType.OpenTuple }, { CloseTuple, TokenType.CloseTuple },
            { OpenArray, TokenType.OpenArray }, { CloseArray, TokenType.CloseArray },
            { StatementEnd, TokenType.StatementEnd }
        };

        private enum TokenBuildMode { String, StringEscape, Number, Generic, Operator }
        private TokenBuildMode _buildMode = TokenBuildMode.Generic;

        private void EnqueueToken(string value,TokenType type) => _tokenQueue.Enqueue(new() {
            Column = _tokenColumnStart,Line = _line,Type = type,Value = value
        });

        private void EnqueueToken(char value,TokenType type) => _tokenQueue.Enqueue(new() {
            Column = _column,Line = _line,Type = type,Value = value.ToString(),
        });

        private TokenType GetNumberType() {
            return _decimalSymbolCount == 0 ? TokenType.Integer : TokenType.Decimal;
        }

        private TokenType GetTokenType() {
            switch(_buildMode) {
                case TokenBuildMode.String: return TokenType.String;
                case TokenBuildMode.Number: return GetNumberType();
                case TokenBuildMode.Operator: return TokenType.Operator;

                /* Internal exception. If this happens, the token decoder has bug with escape sequences. */
                case TokenBuildMode.StringEscape: throw new InvalidOperationException(); 

                default: return TokenType.Generic;
            }
        }

        private void FlushTokenBuilder() {
            if(_tokenBuilder.Length < 1) {
                return;
            }
            TokenType tokenType = GetTokenType();
            if(tokenType == TokenType.Decimal && _tokenBuilder[0] == DecimalSymbol) {
                _tokenBuilder.Insert(0,'0');
            }
            string tokenValue = _tokenBuilder.ToString();
            if(tokenType == TokenType.Operator && !_operators.Contains(tokenValue)) {
                throw CompilerErrorFactory.UnknownOperator(_line,_column,tokenValue);
            }
            if(tokenType == TokenType.Generic && _keyWords.Contains(tokenValue)) {
                tokenType = TokenType.Keyword;
            }
            EnqueueToken(tokenValue,tokenType);
            _tokenBuilder.Clear();
            _tokenColumnStart = -1;
        }

        private void AppendTokenBuilder(char character) {
            if(_tokenBuilder.Length < 1) {
                _tokenColumnStart = _column;
            }
            _tokenBuilder.Append(character);
        }

        private bool TryFlushSymbolOrOperator(char character) {
            if(_whitespace.Contains(character)) {
                FlushTokenBuilder();
                return true;
            }
            if(_symbolTokens.TryGetValue(character,out var tokenType)) {
                FlushTokenBuilder();
                EnqueueToken(character,tokenType);
                return true;
            }
            if(_operatorSymbols.Contains(character)) {
                FlushTokenBuilder();
                _buildMode = TokenBuildMode.Operator;
                ApppendOperatorCharacter(character);
                return true;
            }
            return false;
        }

        private void ApppendOperatorCharacter(char character) {
            if(_symbolTokens.TryGetValue(character,out var tokenType)) {
                FlushTokenBuilder();
                EnqueueToken(character,tokenType);
                _buildMode = TokenBuildMode.Generic;
                return;
            }
            if(!_operatorSymbols.Contains(character)) {
                FlushTokenBuilder();
                _buildMode = TokenBuildMode.Generic;
                AppendGenericCharacter(character);
                return;
            }
            AppendTokenBuilder(character);
        }

        private void AppendGenericCharacter(char character) {
            if(_tokenBuilder.Length <= 0 && _digits.Contains(character)) {
                _buildMode = TokenBuildMode.Number;
                _decimalSymbolCount = character == DecimalSymbol ? 1 : 0;
                AppendTokenBuilder(character);
                return;
            }
            if(TryFlushSymbolOrOperator(character)) {
                return;
            }
            if(character == StringEscape) {
                throw CompilerErrorFactory.UnexpectedSymbol(_line,_column,character);
            }
            if(character == StringLimit) {
                FlushTokenBuilder();
                _buildMode = TokenBuildMode.String;
                return;
            }
            AppendTokenBuilder(character);
        }

        private void AppendStringCharacter(char character) {
            if(character == StringEscape) {
                _buildMode = TokenBuildMode.StringEscape;
                return;
            }
            if(character == StringLimit) {
                FlushTokenBuilder();
                _buildMode = TokenBuildMode.Generic;
                return;
            }
            AppendTokenBuilder(character);
        }

        private void AppendEscapeSequenceCharacter(char character) {
            if(!_escapeSequenceOperands.Contains(character)) {
                throw CompilerErrorFactory.InvalidStringEscapeCharacter(_line,_column,character);
            }
            if(character == NewLineCharacter) {
                character = '\n';
            }
            AppendTokenBuilder(character);
            _buildMode = TokenBuildMode.String;
        }


        private void AppendNumberCharacter(char character) {
            if(TryFlushSymbolOrOperator(character)) {
                _buildMode = TokenBuildMode.Generic;
                return;
            }
            if(!_digits.Contains(character)) {
                FlushTokenBuilder();
                _buildMode = TokenBuildMode.Generic;
                AppendGenericCharacter(character);
                return;
            }
            if(character == DecimalSymbol && ++_decimalSymbolCount > 1) {
                throw CompilerErrorFactory.UnexpectedSymbolInNumber(_line,_column,character);
            }
            AppendTokenBuilder(character);
        }

        private void AppendCharacter(char character,int line,int column) {
            _line = line;
            _column = column;
            switch(_buildMode) {
                case TokenBuildMode.Generic: AppendGenericCharacter(character); break;
                case TokenBuildMode.String: AppendStringCharacter(character); break;
                case TokenBuildMode.StringEscape: AppendEscapeSequenceCharacter(character); break;
                case TokenBuildMode.Number: AppendNumberCharacter(character); break;
                case TokenBuildMode.Operator: ApppendOperatorCharacter(character); break;
                default: throw new NotImplementedException();
            }
        }

        private void AddLineBreak() {
            if(_buildMode == TokenBuildMode.StringEscape) {
                throw CompilerErrorFactory.UnexpectedNewLineInEscapeSequence(_line,_column + 1);
            }
            /* Strings can span multiple lines. Only flush the token builder if the builder is not building a string */
            if(_buildMode == TokenBuildMode.String) {
                return;
            }
            FlushTokenBuilder();
            _buildMode = TokenBuildMode.Generic;
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

        private void ImportLine(string line,int lineNumber) {
            if(line.StartsWith(LineComment)) {
                return;
            }
            for(int column = 0;column<line.Length;column++) {
                char character = line[column];
                AppendCharacter(character,lineNumber,column);
            }
            AddLineBreak();
        }

        public void Import(string[] lines) {
            for(int lineNumber = 0;lineNumber<lines.Length;lineNumber++) {
                string line = lines[lineNumber];
                ImportLine(line,lineNumber);
            }
        }
    }
}
