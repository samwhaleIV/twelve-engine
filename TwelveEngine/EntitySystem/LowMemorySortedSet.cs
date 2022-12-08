using System;
using System.Collections.Generic;

namespace TwelveEngine.EntitySystem {

    internal sealed class LowMemorySortedSet<T> {

        private readonly IComparer<T> comparer;

        //todo dynamically resizing list
        public LowMemorySortedSet(int maxItems,IComparer<T> comparer) {
            list = new T[maxItems];
            idList = new int[maxItems];
            for(int i = 0;i<idList.Length;i++) {
                idList[i] = -1;
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

        private bool IsEmptyID(int ID) => ID < 0;

        private int GetInsertionIndex(T value) {
            //todo: convert this to a binary search, you lazy fuck!

            int index = 0, comparison;
            do {
                T item = list[index];
                if(IsEmptyID(idList[index])) { /* assumes id counter starts at 1, and 0 is virtual null */
                    return index;
                }
                index += 1;
                comparison = comparer.Compare(value,item);
            } while(comparison == 1 && !(index > Count)); /* Sorted in ascending order */
            index -= 1;

            return index;
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

        /* ID must start at 1 and not 0, so we can detect an empty spot in the buffer */
        public void Add(int ID,T value) {
            if(IsEmptyID(ID)) {
                throw new ArgumentException("ID cannot be less than 0!","ID");
            }

            int index = GetInsertionIndex(value);

            int remainingItems = Count - index;
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
