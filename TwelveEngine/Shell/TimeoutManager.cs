using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine.Shell {

    public class TimeoutManager {

        private readonly struct TimeoutSet {

            public readonly int ID;
            public readonly Action Action;
            public readonly TimeSpan EndTime;

            public TimeoutSet(int ID,Action action,TimeSpan endTime) {
                this.ID = ID;
                Action = action;
                EndTime = endTime;
            }
        }

        private readonly Dictionary<int,TimeoutSet> sets = new Dictionary<int,TimeoutSet>();

        private int idCounter = 1;
        private TimeoutSet[] list = new TimeoutSet[0];

        private bool listFrozen = false;
        private void UpdateList() {
            if(listFrozen) {
                return;
            }
            list = sets.Values.ToArray();
        }

        private void PauseListUpdates() {
            listFrozen = true;
        }
        private void ResumeListUpdates() {
            listFrozen = false;
        }

        public int Add(Action action,TimeSpan timeout,TimeSpan currentTime) {
            var ID = idCounter;
            idCounter += 1;
            sets[ID] = new TimeoutSet(ID,action,currentTime + timeout);
            UpdateList();
            return ID;
        }

        public bool Remove(int ID) {
            if(!sets.ContainsKey(ID)) {
                return false;
            }
            sets.Remove(ID);
            UpdateList();
            return true;
        }

        public void Update(TimeSpan currentTime) {
            PauseListUpdates();
            bool didChange = false;
            for(var i = 0;i<list.Length;i++) {
                var set = list[i];
                if(currentTime < set.EndTime) {
                    continue;
                }
                Remove(set.ID);
                set.Action.Invoke();
                didChange = true;
            }
            ResumeListUpdates();
            if(didChange) {
                UpdateList();
            }
        }
    }
}
