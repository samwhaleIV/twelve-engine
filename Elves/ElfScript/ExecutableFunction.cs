using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Elves.BattleSequencer;

namespace Elves.ElfScript {
    public sealed class ExecutableFunction {

        private readonly Command[] commands;
        private readonly string[] parameterNames;

        public string DisplayName { get; private set; }

        public ExecutableFunction(Command[] commands,string[] parameterNames,string displayName) {
            this.commands = commands;
            this.parameterNames = parameterNames;
            DisplayName = displayName;
        }

        public async Task Execute(UVSequencer context,Script script,object[] parameters) {
            if(parameters.Length < parameterNames.Length) {
                throw new ElfScriptException("Function called with insufficient parameters.");
            }
            var oldValues = new Dictionary<string,object>();
            for(int i = 0;i < parameterNames.Length;i++) {
                var key = parameterNames[i];
                if(script.ValueExists(key)) {
                    oldValues[key] = script.GetValue(key);
                }
                script.SetValue(key,parameters[i]);
            }
            foreach(var command in commands) {
                await command.Execute(context,script);
            }
            foreach(var key in parameterNames) {
                script.DeleteValue(key);
            }
            foreach(var item in oldValues) {
                script.SetValue(item.Key,item.Value);
            }
        }
    }
}
