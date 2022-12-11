using System;
using System.Collections.Generic;

namespace TwelveEngine.EntitySystem {

    internal sealed class LowMemorySortedSet<T> {

        private readonly IComparer<T> comparer;

        private const int NO_ID = -1;
        private bool IsEmptyID(int ID) => ID < 0;

        public LowMemorySortedSet(int capacity,IComparer<T> comparer) {
            list = new T[capacity];
            idList = new int[capacity];
            for(int i = 0;i<idList.Length;i++) {
                idList[i] = NO_ID;
            }
            this.comparer = comparer;
        }

        /* Value == int ID */
        private T[] list;
        private int[] idList;

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

        /* Key == int ID */
        private Dictionary<int,IndexWrapper> lookupTable = new Dictionary<int,IndexWrapper>();

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
                throw new ArgumentException("ID cannot be less than 0!","ID");
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

        public void Remove(int ID) {
            if(!lookupTable.ContainsKey(ID)) {
                return;
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
    }
}
