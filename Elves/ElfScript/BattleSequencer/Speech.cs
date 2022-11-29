using Elves.BattleSequencer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elves.ElfScript.BattleSequencer {
    public sealed class Speech:Command {

        public bool List { get; set; }
        public bool NoRepeat { get; set; }
        public bool Random { get; set; }
        public bool RepeatLast { get; set; }

        public override Command[] Compile(string[][] lines) {
            throw new NotImplementedException();
        }

        public override void Execute(UVSequencer context,Script script) {
            //script.SetValue("lastSpeechAtEnd",null);
            throw new NotImplementedException();
        }
    }
}
