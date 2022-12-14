using System;
using System.Collections.Generic;

namespace Elves.Battle {
    public sealed class ThreadPicker {

        private readonly Random _random = new Random();

        public ThreadPicker(Random random) {
            _random = random;
        }

        private readonly Dictionary<int,int> threadIndicies = new Dictionary<int,int>();

        private int lastThreadIndex = 0;
        private int lastThreadSize = 0;
        private bool lastThreadAtEnd = false;

        public int LastThreadIndex => lastThreadIndex;
        public int LastThreadSize => lastThreadSize;
        public bool LastThreadAtEnd => lastThreadAtEnd;

        public int GetThreadIndex(int threadID) {
            int index;
            threadIndicies.TryGetValue(threadID,out index);
            return index;
        }

        public void SetThreadIndex(int threadID,int index) {
            threadIndicies[threadID] = index;
        }

        public string GetThreadValue(int threadID,ThreadMode threadMode,string[] values) {
            int index = GetThreadIndex(threadID);

            lastThreadIndex = index;
            lastThreadAtEnd = index >= values.Length - 1;
            lastThreadSize = values.Length;

            if(threadMode.HasFlag(ThreadMode.NoRepeat) && index >= values.Length) {
                return null;
            }

            string value = values[index];
            if(threadMode.HasFlag(ThreadMode.Random)) {
                index = _random.Next(0,values.Length);
                SetThreadIndex(threadID,index);
                return value;
            }

            index += 1;
            if(index >= values.Length) {
                if(threadMode.HasFlag(ThreadMode.NoRepeat)) {
                    index = values.Length;
                } else if(threadMode.HasFlag(ThreadMode.RepeatLast) && values.Length >= 1) {
                    index = values.Length - 1;
                } else if(threadMode.HasFlag(ThreadMode.SkipFirstOnRepeat) && values.Length > 1) {
                    index = 1;
                } else {
                    index = 0;
                }
            }
            SetThreadIndex(threadID,index);
            return value;
        }
    }
}
