using System;
using System.Collections.Generic;

namespace TwelveEngine.Shell {

    public sealed class TimeoutManager {

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

        private enum QueueOperation { Add, Remove } 

        private readonly struct PendingAction {
            public readonly TimeoutSet Set;
            public readonly QueueOperation Operation;
            public PendingAction(TimeoutSet set,QueueOperation operation) {
                Set = set;
                Operation = operation;
            }
        }

        private int idCounter = 0;
        private bool updating = false;

        private readonly Dictionary<int,TimeoutSet> timeoutSets = new();
        private readonly Queue<PendingAction> actionQueue = new();

        public int Add(Action action,TimeSpan timeout,TimeSpan currentTime) {
            int ID = idCounter;
            idCounter += 1;
            TimeoutSet set = new(ID,action,currentTime + timeout);
            if(updating) {
                actionQueue.Enqueue(new PendingAction(set,QueueOperation.Add));
            } else {
                timeoutSets[ID] = set;
            }
            return ID;
        }

        public bool Remove(int ID) {
            if(!timeoutSets.ContainsKey(ID)) {
                return false;
            }
            if(updating) {
                actionQueue.Enqueue(new PendingAction(timeoutSets[ID],QueueOperation.Remove));
            } else {
                timeoutSets.Remove(ID);
            }
            return true;
        }

        public void Update(TimeSpan currentTime) {
            updating = true;
            foreach(var item in timeoutSets) {
                TimeoutSet set = item.Value;
                if(currentTime < set.EndTime) {
                    continue;
                }
                Remove(set.ID);
                set.Action.Invoke();
            }
            foreach(var item in actionQueue) {
                switch(item.Operation) {
                    case QueueOperation.Add:
                        timeoutSets.Add(item.Set.ID,item.Set);
                        break;
                    case QueueOperation.Remove:
                        timeoutSets.Remove(item.Set.ID);
                        break;
                }
            }
            updating = false;
        }
    }
}
