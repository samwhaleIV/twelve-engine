using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TwelveEngine;

namespace Elves.Battle {
    public sealed partial class ScriptThreader {

        private BattleScript Script { get; init; }
        public ScriptThreader(BattleScript script) => Script = script;

        private readonly Dictionary<int,int> _threadIndicies = new();

        public int LastThreadIndex { get; private set; }
        public int LastThreadSize { get; private set; }
        public bool LastThreadAtEnd { get; private set; }

        public int GetThreadIndex(int threadID) {
            _threadIndicies.TryGetValue(threadID,out int index);
            return index;
        }

        private void SetThreadIndex(int threadID,int index) => _threadIndicies[threadID] = index;

        private string GetThreadValue(int threadID,ThreadMode threadMode,LowMemoryList<string> values) {
            int index = GetThreadIndex(threadID);

            LastThreadIndex = index;
            LastThreadAtEnd = index >= values.Size - 1;
            LastThreadSize = values.Size;

            if(!threadMode.HasFlag(ThreadMode.Repeat) && index >= values.Size) {
                return null;
            }

            string value = values[index];
            if(threadMode.HasFlag(ThreadMode.Random)) {
                index = Script.Random.Next(0,values.Size);
                SetThreadIndex(threadID,index);
                return value;
            }

            index += 1;
            if(index < values.Size) {
                SetThreadIndex(threadID,index);
                return value;
            }

            if(!threadMode.HasFlag(ThreadMode.Repeat)) {
                index = values.Size;
            } else if(threadMode.HasFlag(ThreadMode.HoldLast) && values.Size >= 1) {
                index = values.Size - 1;
            } else if(threadMode.HasFlag(ThreadMode.SkipFirst) && values.Size > 1) {
                index = 1;
            } else {
                index = 0;
            }

            SetThreadIndex(threadID,index);
            return value;
        }

        public async Task Tag(LowMemoryList<string> tags,ThreadMode threadMode = ThreadMode.NoRepeat,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,tags);
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Speech(LowMemoryList<string> speeches,ThreadMode threadMode = ThreadMode.NoRepeat,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,speeches);
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }
    }
}
