using Elves.BattleSequencer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Elves.ElfScript {
    public sealed class Function {
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string[] Parameters { get; set; }
        public string[][] Lines { get; set; }
    }
}
