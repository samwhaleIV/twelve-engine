using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Elves.BattleSequencer;

namespace Elves.ElfScript {
    public sealed class Script {
        
        private readonly Dictionary<string,ExecutableFunction> executableFunctions = new Dictionary<string,ExecutableFunction>();

        private Script(Function[] functions) {
            Queue<Command> queue = new Queue<Command>();
            foreach(var function in functions) {
                foreach(string[] line in function.Lines) {
                    Command command = CommandFactory.Route(line);
                    queue.Enqueue(command);
                }
                executableFunctions[function.Name] = new ExecutableFunction(queue.ToArray(),function.Parameters,function.DisplayName);
                queue.Clear();
            }
        }

        private readonly Random random = new Random();

        public int GetRandom(int min,int max) {
            return random.Next(min,max);
        }

        public string GetFunctionDisplayName(string function) {
            if(!executableFunctions.ContainsKey(function)) {
                return null;
            } else {
                return executableFunctions[function].DisplayName;
            }
        }

        internal readonly Queue<string> OptionQueue = new Queue<string>();

        internal ConditionalBranch ConditionalBranch = ConditionalBranch.None;
        internal bool ConditionalPolarity = true;

        internal bool InSwitchStatement = false;
        internal (string Function,string[] Parameters)? SwitchCase = null;

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

        public Task Execute(string function,UVSequencer context,string[] parameters) {
            if(!executableFunctions.ContainsKey(function)) {
                throw new ElfScriptException($"Function '{function}' does not exist!");
            }
            return executableFunctions[function].Execute(context,this,parameters);
        }
    }
}
