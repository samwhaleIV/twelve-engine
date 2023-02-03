namespace TwelveEngine {

    internal sealed class LowMemorySortedSet<T> {

        /* Value == int ID */
        private T[] list;
        private int[] idList;

        /* Key == int ID */
        private readonly Dictionary<int,IndexWrapper> lookupTable = new();

        public LowMemorySortedSet(int capacity,IComparer<T> comparer) {
            list = new T[capacity];
            idList = new int[capacity];
            for(int i = 0;i<idList.Length;i++) {
                idList[i] = NO_ID;
            }
            this.comparer = comparer;
        }

        private readonly IComparer<T> comparer;

        private const int NO_ID = -1;
        private static bool IsEmptyID(int ID) => ID < 0;

        private struct IndexWrapper {

            public int Index;
            public T Value;

            public IndexWrapper(int index,T value) {
                Index = index;
                Value = value;
            }
        }

        public T[] List => list;
        public int Count => lookupTable.Count;

        private int GetInsertionIndex(T value) {
            int index = Array.BinarySearch(list,0,Count,value,comparer);
            if(index >= 0) {
                return index;
            }
            return -index - 1;
        }

        private void UpdateIndexWrapper(int oldIndex,int newIndex) {
            int id = idList[oldIndex];
            if(IsEmptyID(id)) {
                return;
            }
            IndexWrapper wrapper = lookupTable[id];
            wrapper.Index = newIndex;
            lookupTable[id] = wrapper;
        }

        private void ResizeList() {
            int newSize = (int)MathF.Pow(2,MathF.Ceiling(MathF.Log(Count + 1)/MathF.Log(2)));

            T[] newList = new T[newSize];
            int[] newIDList = new int[newSize];

            for(int i = 0;i<Count;i++) {
                newList[i] = list[i];
                newIDList[i] = idList[i];
            }
            for(int i = Count;i<newSize;i++) {
                newIDList[i] = NO_ID;
            }

            list = newList;
            idList = newIDList;
        }

        /* ID must start at 1 and not 0, so we can detect an empty spot in the buffer */
        public void Add(int ID,T value) {
            if(IsEmptyID(ID)) {
                throw new ArgumentException("ID cannot be less than 0!",nameof(ID));
            }
            if(Count == list.Length) {
                ResizeList();
            }

            int index = GetInsertionIndex(value);

            int remainingItems = Count - index - 1;
            for(int i = index + remainingItems;i >= index;i--) {
                UpdateIndexWrapper(i,i+1);
                list[i+1] = list[i];
                idList[i+1] = idList[i];
            }

            list[index] = value;
            idList[index] = ID;

            lookupTable[ID] = new IndexWrapper(index,value);
        }

        public bool Contains(int ID) {
            return lookupTable.ContainsKey(ID);
        }

        public void Remove(int ID) {
            if(!lookupTable.ContainsKey(ID)) {
                throw new ArgumentOutOfRangeException($"ID '{ID}' does not exist in sorted set.");
            }
            int removalIndex = lookupTable[ID].Index;

            int remainingItems = Count - removalIndex;
            int endIndex = removalIndex + remainingItems;

            for(int i = removalIndex;i<endIndex;i++) {
                UpdateIndexWrapper(i+1,i);
                list[i] = list[i+1];
                idList[i] = idList[i+1];
            }
            lookupTable.Remove(ID);
        }

        /// <summary>
        /// Remove all entries from the sorted set.
        /// </summary>
        public void Clear() {
            lookupTable.Clear();
            for(int i = 0;i<list.Length;i++) {
                list[i] = default;
                idList[i] = NO_ID;
            }
        }

        private readonly Queue<(int ID,T Value)> refreshBuffer = new();

        /// <summary>
        /// Empty the sorted set then reorder them.
        /// </summary>
        public void Refresh() {
            /* Bleeeeeeeh. I think I just threw up in my mouth */
            int itemCount = Count;

            for(int i = 0;i < itemCount;i++) {
                int ID = idList[i];
                list[i] = default;
                idList[i] = NO_ID;
                if(ID == NO_ID) {
                    continue;
                }
                lookupTable.Remove(ID,out var item);
                refreshBuffer.Enqueue((ID,item.Value));
            }

            while(refreshBuffer.TryDequeue(out var item)) {
                Add(item.ID,item.Value);
            }
        }
    }
}
