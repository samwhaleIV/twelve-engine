using Elves.ElfScript;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elves.BattleSequencer {

    public sealed class IfStatement:Command {

        private bool Evaluate(UVSequencer context,Script script) {
            throw new NotImplementedException();
        }

        public override async Task Execute(UVSequencer context,Script script) {
            if(script.ConditionalBranch != ConditionalBranch.None) {
                throw new ElfScriptException("Don't write your own scripting language, nested if statements are illegal.");
            }
            var conditionalPolarity = Evaluate(context,script);
            script.ConditionalPolarity = conditionalPolarity;
            script.ConditionalBranch = ConditionalBranch.If;
        }
    }
    public sealed class ElseStatement:Command {
        public override async Task Execute(UVSequencer context,Script script) {
            script.ConditionalBranch = ConditionalBranch.Else;
        }
    }
    public sealed class EndIfStatement:Command {
        public override async Task Execute(UVSequencer context,Script script) {
            script.ConditionalBranch = ConditionalBranch.None;
            script.ConditionalPolarity = true;
        }
    }
    public sealed class ReturnStatement:Command {
        public override async Task Execute(UVSequencer context,Script script) { }
    }

    public sealed class SwitchStatement:Command {
        public override async Task Execute(UVSequencer context,Script script) {
            if(script.InSwitchStatement) {
                throw new ElfScriptException("Cannot create a switch statement inside of an executing" +
                    " switch statement block.");
            }
            script.InSwitchStatement = true;
        }
    }

    public sealed class DefaultStatement:Command {
        public override async Task Execute(UVSequencer context,Script script) {

        }
    }
    public sealed class SwitchCase:Command {
        public override async Task Execute(UVSequencer context,Script script) {
            //todo compare switch evaluation
        }
    }
    public sealed class EndSwitchStatement:Command {
        public override async Task Execute(UVSequencer context,Script script) {
            if(!script.InSwitchStatement) {
                throw new ElfScriptException("Cannot terminate switch with no active switch block!");
            }
            script.InSwitchStatement = false;
            if(!script.SwitchCase.HasValue) {
                return;
            }
            var switchCase = script.SwitchCase.Value;
            var function = switchCase.Function;
            script.SwitchCase = null;
            script.InSwitchStatement = false;
            await script.Execute(function,context,switchCase.Parameters);
        }
    }
}
