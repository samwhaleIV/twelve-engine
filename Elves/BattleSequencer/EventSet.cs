using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Elves.BattleSequencer {
    public sealed class EventSet:UVEvent {
        private readonly UVEvent[] uvEvents;
        public EventSet(params UVEvent[] uvEvents) {
            this.uvEvents = uvEvents;
        }
        public EventSet(Queue<UVEvent> uvEvents) {
            this.uvEvents = uvEvents.ToArray();
        }
        public override void Invoke(UVSequencer uvSequencer) {
            foreach(UVEvent uvEvent in uvEvents) {
                uvEvent.Invoke(uvSequencer);
            }
        }
    }
}
