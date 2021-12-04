using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine {
    internal sealed class TimeoutManager {
        private readonly struct Set {

            public readonly int ID;
            public readonly Action Action;
            public readonly TimeSpan EndTime;

            public Set(int ID,Action action,TimeSpan endTime) {
                this.ID = ID;
                Action = action;
                EndTime = endTime;
            }
        }

        private readonly Dictionary<int,Set> sets = new Dictionary<int,Set>();

        private int idCounter = 0;
        private Set[] list = new Set[0];

        private void updateList() {
            list = sets.Values.ToArray();
        }

        internal int Add(Action action,TimeSpan timeout,TimeSpan currentTime) {
            var ID = idCounter;
            idCounter += 1;
            sets[ID] = new Set(ID,action,currentTime + timeout);
            updateList();
            return ID;
        }

        internal bool Remove(int ID) {
            if(!sets.ContainsKey(ID)) {
                return false;
            }
            sets.Remove(ID);
            updateList();
            return true;
        }

        internal void Update(TimeSpan currentTime) {
            for(var i = 0;i<list.Length;i++) {
                var set = list[i];
                if(currentTime < set.EndTime) {
                    continue;
                }
                Remove(set.ID);
                set.Action.Invoke();
            }
        }
    }
}
