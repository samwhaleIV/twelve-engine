using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    [Flags]
    public enum UserModifiers {
        None, NoDamage, NoHeal, Disabled
    }
}
