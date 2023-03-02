using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TwelveEngine;

namespace Elves.Battle.Scripting {
    public sealed partial class ScriptThreader {

        private const ThreadMode DefaultThreadMode = ThreadMode.Repeat;

        private BattleScript Script { get; init; }
        public ScriptThreader(BattleScript script) => Script = script;

        private readonly Dictionary<int,int> _threadIndicies = new();

        public int LastThreadIndex { get; private set; }
        public int LastThreadSize { get; private set; }
        public bool LastThreadAtEnd { get; private set; }

        public int GetThreadIndex(int threadID) {
            if(!_threadIndicies.TryGetValue(threadID,out int index)) {
                return -1;
            }
            return index;
        }

        private void SetThreadIndex(int threadID,int index) => _threadIndicies[threadID] = index;

        private static bool TryGetNewThreadIndex(ref int index,ThreadMode threadMode,int size,Random random) {
            if(threadMode.HasFlag(ThreadMode.Random)) {
                index = random.Next(0,size);
                return true;
            }
            index = index < 0 ? 0 : index + 1;
            if(index < size) {
                return true;
            }
            if(threadMode.HasFlag(ThreadMode.HoldLast)) {
                index = size - 1;
                return true;
            }
            if(threadMode.HasFlag(ThreadMode.SkipFirst)) {
                index = size >= 2 ? 1 : 0;
                return true;
            }
            if(threadMode.HasFlag(ThreadMode.Repeat)) {
                index = 0;
                return true;
            }
            return false;
        }

        private bool TryGetThreadValue<T>(int threadID,ThreadMode threadMode,LowMemoryList<T> values,out T value) {
            int index = GetThreadIndex(threadID);
            if(!TryGetNewThreadIndex(ref index,threadMode,values.Size,Script.Random)) {
                value = default;
                return false;
            }
            SetThreadIndex(threadID,index);
            LastThreadIndex = index;
            LastThreadSize = values.Size;
            LastThreadAtEnd = index + 1 >= values.Size;   
            value = values[index];
            return true;
        }

        public async Task Task(LowMemoryList<Func<Task>> functions,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            if(!TryGetThreadValue(threadID,threadMode,functions,out var func)) {
                return;
            }
            await func.Invoke();
        }

        public async Task Speech(LowMemoryList<PolyThreadSelection<string>> selections,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            if(!TryGetThreadValue(threadID,threadMode,selections,out var selection)) {
                return;
            }
            await (selection.IsItem ? Script.Speech(selection.ItemList) : selection.Task.Invoke());
        }

        public async Task Tag(LowMemoryList<PolyThreadSelection<string>> selections,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            if(!TryGetThreadValue(threadID,threadMode,selections,out var selection)) {
                return;
            }
            await (selection.IsItem ? Script.Tag(selection.ItemList) : selection.Task.Invoke());
        }
    }
}
