using System.Text;

namespace TwelveEngine {

    public abstract class PoolSet<T> {

        private readonly Dictionary<int,T> _active = new();
        private readonly Stack<T> _inactive = new();

        private int _IDCounter = 0;

        protected abstract void Reset(T item);
        protected internal abstract T CreateNew();

        public int Lease(out T item) {
            if(_IDCounter == int.MaxValue) {
                _IDCounter = int.MinValue;
            }
            int ID = _IDCounter++;
            if(_inactive.Count <= 0) {
                item = CreateNew();
            } else {
                item = _inactive.Pop();
            }
            _active[ID] = item;
            return ID;
        }

        public void Return(int ID) {
            if(!_active.TryGetValue(ID,out T value)) {
                throw new InvalidOperationException($"ID '{ID}' not contained in active pool.");
            }
            Reset(value);
            _active.Remove(ID);
            _inactive.Push(value);
        }

        public (int ID1, int ID2) Lease(out T item1,out T item2) {
            return (Lease(out item1), Lease(out item2));
        }

        public void Return((int ID1, int ID2) IDs) {
            Return(IDs.ID1);
            Return(IDs.ID2);
        }

        public (int ID1, int ID2, int ID3) Lease(out T item1,out T item2,out T item3) {
            return (Lease(out item1), Lease(out item2), Lease(out item3));
        }

        public void Return((int ID1, int ID2,int ID3) IDs) {
            Return(IDs.ID1);
            Return(IDs.ID2);
            Return(IDs.ID3);
        }
    }

    public sealed class StringBuilderPool:PoolSet<StringBuilder> {
        protected override void Reset(StringBuilder item) => item.Clear();
        protected internal override StringBuilder CreateNew() => new();
    }

    public static class Pools {
        public static readonly StringBuilderPool StringBuilder = new();
    }
}
