using System;
using System.Collections.Generic;
using System.Text;

namespace Elves.BattleSequencer {
    public abstract class Event {
        public abstract void Invoke(Sequencer sequencer);
    }
}
