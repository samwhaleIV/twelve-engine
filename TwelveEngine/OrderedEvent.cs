namespace TwelveEngine {

    public enum EventPriority {
        First = 0,
        Second = 1,
        Third = 2,
        Fourth = 3,
        Normal = 4,
        FourthToLast = 5,
        ThirdToLast = 6,
        SecondToLast = 7,
        Last = 8
    };

    internal class OrderedEventSorter<TAction>:IComparer<PriorityAction<TAction>> {
        public int Compare(PriorityAction<TAction> a,PriorityAction<TAction> b) {
            if(a.Priority == b.Priority) {
                return a.ID.CompareTo(b.ID);
            } else {
                return a.Priority.CompareTo(b.Priority);
            }
        }
    }

    internal readonly struct PriorityAction<TAction> {
        public readonly EventPriority Priority { get; init; }
        public readonly TAction Action { get; init; }
        public readonly int ID { get; init; }
    }

    public abstract class OrderedEvent<TAction> where TAction:Delegate {

        private const int DEFAULT_SORTED_SET_SIZE = 16;
        private readonly LowMemorySortedSet<PriorityAction<TAction>> sortedSet = new(DEFAULT_SORTED_SET_SIZE,GetSorter());

        private static OrderedEventSorter<TAction> GetSorter() => new();

        private int _IDCounter = 0;
        private bool _bufferNeedsRefresh = true;
        private readonly Queue<PriorityAction<TAction>> _actionBuffer = new();

        public int Add(TAction action,EventPriority priority = EventPriority.Normal) {
            if(action is null) {
                throw new ArgumentNullException(nameof(action));
            }
            int ID = _IDCounter;
            _IDCounter += 1;
            PriorityAction<TAction> priorityAction = new() {
                ID = ID,
                Action = action,
                Priority = priority
            };
            sortedSet.Add(priorityAction.ID,priorityAction);
            _bufferNeedsRefresh = true;
            return priorityAction.ID;
        }

        public void Remove(int ID) {
            sortedSet.Remove(ID);
            _bufferNeedsRefresh = true;
        }

        private void TryUpdateBuffer() {
            if(!_bufferNeedsRefresh) {
                return;
            }
            _actionBuffer.Clear();
            for(int i = 0;i< sortedSet.Count;i++) {
                _actionBuffer.Enqueue(sortedSet.List[i]);
            }
            _bufferNeedsRefresh = false;
        }

        public abstract void OnInvoke(TAction action);

        public void Invoke() {
            TryUpdateBuffer();
            foreach(var action in _actionBuffer) OnInvoke(action.Action);
        }
    }

    public sealed class OrderedEvent:OrderedEvent<Action> {
        public override void OnInvoke(Action action) => action.Invoke();
    }
}
