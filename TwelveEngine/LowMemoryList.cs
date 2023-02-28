using System.Collections;

namespace TwelveEngine {

    /* Zero allocation scripting support. Please don't take this too seriously. I'm not that proud of it. */

    public readonly struct LowMemoryList<T>:IEnumerable<T> {

        private const int MAX_SIZE = 8;

        public readonly T Item1, Item2, Item3, Item4, Item5, Item6, Item7, Item8;
        private static readonly BufferPool _indexingBuffers = new(MAX_SIZE);

        public readonly int Size { get; private init; }

        private sealed class BufferPool:PoolSet<T[]> {
            public int Size { get; private init; }
            public BufferPool(int size) => Size = size;
            protected override void Reset(T[] item) { }
            protected internal override T[] CreateNew() => new T[Size];
        }

        private void WriteToBuffer(T[] buffer) {
            buffer[0] = Item1; buffer[1] = Item2;
            buffer[2] = Item3; buffer[3] = Item4;
            buffer[4] = Item5; buffer[5] = Item6;
            buffer[6] = Item7; buffer[7] = Item8;
        }

        /* Not intended for sequential read access. For the love of God, use the enumerator. */
        public bool TryGet(int index,out T value) {
            if(index >= Size || index < 0) {
                value = default;
                return false;
            }
            var lease = _indexingBuffers.Lease(out T[] buffer);
            WriteToBuffer(buffer);
            value = buffer[index];
            _indexingBuffers.Return(lease);
            return true;
        }

        public T this[int i] {
            get {
                if(!TryGet(i,out T value)) {
                    throw new IndexOutOfRangeException($"Index exceeds list size ({Size}).");
                }
                return value;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetItemEnumerator();
        }

        public IEnumerator<T> GetEnumerator() {
            return GetItemEnumerator();
        }

        public LowMemoryList(T item) {
            Item1 = item; Item2 = default; Item3 = default; Item4 = default;
            Item5 = default; Item6 = default; Item7 = default; Item8 = default;
            Size = 1;
        }

        public LowMemoryList(T item1,T item2) {
            Item1 = item1; Item2 = item2; Item3 = default; Item4 = default;
            Item5 = default; Item6 = default; Item7 = default; Item8 = default;
            Size = 2;
        }

        public LowMemoryList(T item1,T item2,T item3) {
            Item1 = item1; Item2 = item2; Item3 = item3;
            Item4 = default; Item5 = default; Item6 = default; Item7 = default; Item8 = default;
            Size = 3;
        }

        public LowMemoryList(T item1,T item2,T item3,T item4) {
            Item1 = item1; Item2 = item2; Item3 = item3; Item4 = item4;
            Item5 = default; Item6 = default; Item7 = default; Item8 = default;
            Size = 4;
        }

        public LowMemoryList(T item1,T item2,T item3,T item4,T item5) {
            Item1 = item1; Item2 = item2; Item3 = item3; Item4 = item4;
            Item5 = item5; Item6 = default; Item7 = default; Item8 = default;
            Size = 5;
        }

        public LowMemoryList(T item1,T item2,T item3,T item4,T item5,T item6) {
            Item1 = item1; Item2 = item2; Item3 = item3; Item4 = item4;
            Item5 = item5; Item6 = item6; Item7 = default; Item8 = default;
            Size = 6;
        }

        public LowMemoryList(T item1,T item2,T item3,T item4,T item5,T item6,T item7) {
            Item1 = item1; Item2 = item2; Item3 = item3; Item4 = item4;
            Item5 = item5; Item6 = item6; Item7 = item7; Item8 = default;
            Size = 7;
        }

        public LowMemoryList(T item1,T item2,T item3,T item4,T item5,T item6,T item7,T item8) {
            Item1 = item1; Item2 = item2; Item3 = item3; Item4 = item4;
            Item5 = item5; Item6 = item6; Item7 = item7; Item8 = item8;
            Size = 8;
        }

        private IEnumerator<T> GetItemEnumerator() {
            if(Size >= 1) {
                yield return Item1;
            } else {
                yield break;
            }
            if(Size >= 2) {
                yield return Item2;
            } else {
                yield break;
            }
            if(Size >= 3) {
                yield return Item3;
            } else {
                yield break;
            }
            if(Size >= 4) {
                yield return Item4;
            } else {
                yield break;
            }
            if(Size >= 5) {
                yield return Item5;
            } else {
                yield break;
            }
            if(Size >= 6) {
                yield return Item6;
            } else {
                yield break;
            }
            if(Size >= 7) {
                yield return Item7;
            } else {
                yield break;
            }
            if(Size >= 8) {
                yield return Item8;
            } else {
                yield break;
            }
        }
    }
}
