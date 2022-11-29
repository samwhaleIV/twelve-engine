using Elves.BattleSequencer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Elves.ElfScript {
    public static class Tokenizer {

        const char TEXT_BLOCK_SEPERATOR = '|';
        const char COMMAND_BLOCK_SEPERATOR = ':';
        const char EQUALITY_OPERATOR = '=';
        const char PARAMETER_OPEN_BRACKET = '(';
        const char PARAMETER_CLOSE_BRACKET = ')';
        const char PARAMETER_DELIMITER = ',';

        private static readonly HashSet<char> textSeperations = new HashSet<char>() { TEXT_BLOCK_SEPERATOR, COMMAND_BLOCK_SEPERATOR, EQUALITY_OPERATOR };

        private static readonly HashSet<char> equalityModifier = new HashSet<char>() { '+','/','*','-' };

        private static string[] Tokenize(string line) {
            Queue<string> tokens = new Queue<string>();
            string buffer = string.Empty;
            bool inCommandBlock = true;
            void flushBuffer() {
                if(buffer == string.Empty) {
                    return;
                }
                if(inCommandBlock) {
                    foreach(string segment in buffer.Split(' ',StringSplitOptions.RemoveEmptyEntries)) {
                        tokens.Enqueue(segment.Trim());
                    }
                } else {
                    tokens.Enqueue(buffer.Trim());
                }
                buffer = string.Empty;
            }
            char lastCharacter = char.MinValue;
            foreach(char character in line) {
                if(!inCommandBlock ? character == TEXT_BLOCK_SEPERATOR : textSeperations.Contains(character)) {
                    if(character == COMMAND_BLOCK_SEPERATOR) {
                        flushBuffer();
                        inCommandBlock = false;
                        tokens.Enqueue(COMMAND_BLOCK_SEPERATOR.ToString());
                    } else if(character == EQUALITY_OPERATOR && equalityModifier.Contains(lastCharacter)) {
                        buffer = buffer.Remove(buffer.Length - 1, 1);
                        flushBuffer();
                        buffer += lastCharacter;
                        buffer += character;
                        flushBuffer();
                    } else {
                        flushBuffer();
                    }
                } else {
                    buffer += character;
                }
                lastCharacter = character;
            }
            flushBuffer();
            return tokens.ToArray();
        }

        private static string CreateDisplayName(string name) {
            StringBuilder stringBuilder = new StringBuilder();
            foreach(char character in name) {
                if(char.IsUpper(character) && stringBuilder.Length > 0) {
                    stringBuilder.Append(' ');
                }
                stringBuilder.Append(character);
            }
            string displayName = stringBuilder.ToString().Trim();
            return displayName;
        }

        private static void DecorateFunctionHeader(string[] line,Function target) {
            string header = string.Join(null,line,1,line.Length-1);
            string[] split = header.Split(PARAMETER_OPEN_BRACKET,StringSplitOptions.RemoveEmptyEntries);
            if(split.Length != 2) {
                throw new ElfScriptException("Malformed function header.");
            } else if(split[1][split[1].Length-1] != PARAMETER_CLOSE_BRACKET) {
                throw new ElfScriptException("Function header missing ending parenthesis.");
            }
            string name = split[0];
            string parametersString = split[1].TrimEnd(PARAMETER_CLOSE_BRACKET);
            string[] parameters;
            if(parametersString.Length == 0) {
                parameters = new string[0];
            } else {
                parameters = parametersString.Split(PARAMETER_DELIMITER,StringSplitOptions.None);
                foreach(string parameter in parameters) {
                    if(parameter == string.Empty) {
                        throw new ElfScriptException("Empty parameter in function header.");
                    }
                }
            }
            target.Name = name;
            target.Parameters = parameters;
            target.DisplayName = CreateDisplayName(name);
            return;
        }

        private static Function[] MapFunctions(string[][] lines) {
            Dictionary<string,Function> functions = new Dictionary<string,Function>();
            Queue<string[]> functionBuffer = new Queue<string[]>();
            bool inFunction = false;
            Function function = null;
            void flushFunction() {
                function.Lines = functionBuffer.ToArray();
                functionBuffer.Clear();
                if(functions.ContainsKey(function.Name)) {
                    throw new ElfScriptException($"Duplicate function name {function.Name}.");
                }
                functions[function.Name] = function;
                function = null;  
            }
            foreach(var line in lines) {
                switch(line[0]) {
                    case "func":
                        if(inFunction) {
                            throw new ElfScriptException("Cannot start a new 'func' block before terminating the previous one.");
                        }
                        inFunction = true;
                        function = new Function();
                        DecorateFunctionHeader(line,function);
                        break;
                    case "endfunc":
                        if(inFunction) {
                            flushFunction();
                            inFunction = false;
                        } else {
                            throw new ElfScriptException("'endfunc' statement does not follow a 'func' statement.");
                        }
                        break;
                    default:
                        if(!inFunction) {
                            throw new ElfScriptException("Code exists outside of a block - all top level code must execute in the main entry point.");
                        } else {
                            functionBuffer.Enqueue(line);
                        }
                        break;
                }
            }
            if(function != null) {
                throw new ElfScriptException("'func' is missing an 'endfunc' statement");
            }
            return functions.Values.ToArray();
        }

        public static Function[] GetFunctions(string[] lines) {
            Queue<string[]> tokenLines = new Queue<string[]>();
            foreach(var line in lines) {
                if(line == string.Empty) {
                    continue;
                }
                tokenLines.Enqueue(Tokenize(line));
            }
            return MapFunctions(tokenLines.ToArray());
        }
    }
}
