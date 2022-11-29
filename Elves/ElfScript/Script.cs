using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Elves.BattleSequencer;

namespace Elves.ElfScript {
    public sealed class Script {
        
        private readonly Dictionary<string,ExecutableFunction> executableFunctions = new Dictionary<string,ExecutableFunction>();

        private Script(Function[] functions) {
            foreach(var function in functions) {
                Queue<Command> queue = new Queue<Command>();
                foreach(string[] line in function.Lines) {
                    var commands = CommandRouter.Route(line);
                    foreach(var command in commands) {
                        queue.Enqueue(command);
                    }
                }
            }
        }

        public static Script Compile(Function[] functions) => new Script(functions);

        private readonly Dictionary<string,object> variables = new Dictionary<string,object>();

        public bool ValueExists(string name) {
            return variables.ContainsKey(name);
        }
        public object GetValue(string name) {
            return variables[name];
        }
        public void SetValue(string name,object value) {
            variables[name] = value;
        }
        public bool DeleteValue(string name) {
            return variables.Remove(name);
        }

        public void Execute(string functionName,UVSequencer context,string[] parameters) {
            if(!executableFunctions.ContainsKey(functionName)) {
                throw new ElfScriptException($"Function '{functionName}' does not exist!");
            }
            executableFunctions[functionName].Execute(context,this,parameters);
        }
    }
}
