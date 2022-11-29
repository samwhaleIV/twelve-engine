using Elves.BattleSequencer;
using System.Threading.Tasks;

namespace Elves.ElfScript.BattleSequencer {
    public sealed class Option:Command {

        private readonly string function;
        private readonly string[] parameters;

        public Option(string function,string[] parameters) {
            this.function = function;
            this.parameters = parameters;
        }

        public override async Task Execute(UVSequencer context,Script script) {
            script.OptionQueue.Clear();
            await script.Execute(this.function,context,parameters);
            string[] functions = script.OptionQueue.ToArray();
            string[] displayNames = new string[functions.Length];
            for(int i = 0;i<functions.Length;i++) {
                displayNames[i] = script.GetFunctionDisplayName(functions[i]);
            }
            int selection = await context.SelectOption(displayNames);
            if(selection < 0 || selection >= functions.Length) {
                throw new ElfScriptException("Option selection out of bounds.");
            }
            string function = functions[selection];
            script.SetValue("lastOptionName",function);
            await script.Execute(function,context,parameters);
        }
    }

    public sealed class AddOption:Command {
        private readonly string function;
        public AddOption(string function) {
            this.function = function;
        }
        public override async Task Execute(UVSequencer context,Script script) {
            script.OptionQueue.Enqueue(function);
        }
    }
}
