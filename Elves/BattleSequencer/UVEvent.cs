using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public abstract class UVEvent {
        public abstract void Invoke(UVSequencer battleInterface);
    }
}
