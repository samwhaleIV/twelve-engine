using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

namespace Elves.Battle.Script {
    public abstract class Script {

        private readonly Random random = new Random();

        private readonly IUVSequencer sequencer;

        public Script(IUVSequencer sequencer) {
            this.sequencer = sequencer;
        }

        public abstract Task Main();

        protected Task<int> GetOption(string[] options) {
            return sequencer.GetOption(options);
        }

        protected Task Tag(string tag) {
            return sequencer.Tag(tag);
        }
        protected Task Speech(string speech) {
            return sequencer.Tag(speech);
        }

        protected async Task Tag(params string[] tags) {
            foreach(var tag in tags) await sequencer.Tag(tag);
        }
        protected async Task Speech(params string[] speeches) {
            foreach(var speech in speeches) await sequencer.Speech(speech);
        }

        private readonly Dictionary<int,int> threadIndicies = new Dictionary<int,int>();

        private int lastThreadIndex = 0;
        private int lastThreadSize = 0;
        private bool lastThreadAtEnd = false;

        protected int LastThreadIndex => lastThreadIndex;
        protected int LastThreadSize => lastThreadSize;
        protected bool LastThreadAtEnd => lastThreadAtEnd;

        protected int GetThreadIndex(int threadID) {
            int index;
            threadIndicies.TryGetValue(threadID, out index);
            return index;
        }

        private void SetThreadIndex(int threadID,int index) {
            threadIndicies[threadID] = index;
        }

        private string GetThreadValue(int threadID,ThreadMode threadMode,string[] values) {
            int index = GetThreadIndex(threadID);

            lastThreadIndex = index;
            lastThreadAtEnd = index >= values.Length - 1;
            lastThreadSize = values.Length;

            if(threadMode.HasFlag(ThreadMode.NoRepeat) && index >= values.Length) {
                return null;
            }

            string value = values[index];
            if(threadMode.HasFlag(ThreadMode.Random)) {
                index = random.Next(0,values.Length);
            } else {
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
            }

            SetThreadIndex(threadID,index);
            return value;
        }

        protected async Task TagThread(ThreadMode threadMode,string[] tags,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,tags);
            if(threadValue == null) {
                return;
            }
            await sequencer.Tag(threadValue);
        }

        protected async Task SpeechThread(ThreadMode threadMode,string[] speeches,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,speeches);
            if(threadValue == null) {
                return;
            }
            await sequencer.Speech(threadValue);
        }
    }
}
