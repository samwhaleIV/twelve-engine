using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Elves.Battle {
    public sealed class ScriptThreader {

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

        private string GetThreadValue(int threadID,ThreadMode threadMode,string[] values) {
            int index = GetThreadIndex(threadID);

            LastThreadIndex = index;
            LastThreadAtEnd = index >= values.Length - 1;
            LastThreadSize = values.Length;

            if(threadMode.HasFlag(ThreadMode.NoRepeat) && index >= values.Length) {
                return null;
            }

            string value = values[index];
            if(threadMode.HasFlag(ThreadMode.Random)) {
                index = Script.Random.Next(0,values.Length);
                SetThreadIndex(threadID,index);
                return value;
            }

            index += 1;
            if(index < values.Length) {
                SetThreadIndex(threadID,index);
                return value;
            }

            if(threadMode.HasFlag(ThreadMode.NoRepeat)) {
                index = values.Length;
            } else if(threadMode.HasFlag(ThreadMode.RepeatLast) && values.Length >= 1) {
                index = values.Length - 1;
            } else if(threadMode.HasFlag(ThreadMode.SkipFirstOnRepeat) && values.Length > 1) {
                index = 1;
            } else {
                index = 0;
            }

            SetThreadIndex(threadID,index);
            return value;
        }

        public async Task TagThread(ThreadMode threadMode,string[] tags,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,tags);
            if(threadValue == null) {
                return;
            }
            Script.SetTag(threadValue);
            await Script.Continue();
            Script.HideTag();
        }

        public async Task SpeechThread(ThreadMode threadMode,string[] speeches,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,speeches);
            if(threadValue == null) {
                return;
            }
            Script.ShowSpeech(threadValue);
            await Script.Continue();
            Script.HideSpeech();
        }
    }
}
