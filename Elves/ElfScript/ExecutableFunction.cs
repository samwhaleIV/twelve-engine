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
                var isReturnStatement = command is ReturnStatement;
                if(isReturnStatement || command is EndIfStatement) {
                    script.ConditionalBranch = ConditionalBranch.None;
                    script.ConditionalPolarity = true;
                    if(isReturnStatement) {
                        script.InSwitchStatement = false;
                        script.SwitchCase = null;
                    }
                    await command.Execute(context,script);
                } else if(script.SwitchCase.HasValue) {
                    if(!(command is EndSwitchStatement)) {

                        continue;
                    }
                } else {
                    switch(script.ConditionalBranch) {
                        case ConditionalBranch.If:
                            if(!script.ConditionalPolarity) {
                                continue;
                            }
                            break;
                        case ConditionalBranch.Else:
                            if(script.ConditionalPolarity) {
                                continue;
                            }
                            break;
                    }
                    await command.Execute(context,script);
                }
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
