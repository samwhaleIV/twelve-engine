using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Elves.Battle.Scripting {
    public sealed partial class ScriptThreader {
        public async Task Tag(string tag1,string tag2,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Tag(string tag1,string tag2,string tag3,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2,tag3));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2,tag3,tag4));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5,tag6));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,string tag7,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5,tag6,tag7));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Tag(string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,string tag7,string tag8,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5,tag6,tag7,tag8));
            if(threadValue == null) {
                return;
            }
            await Script.Tag(threadValue);
        }

        public async Task Speech(string speech1,string speech2,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }

        public async Task Speech(string speech1,string speech2,string speech3,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2,speech3));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2,speech3,speech4));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5,speech6));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,string speech7,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5,speech6,speech7));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }

        public async Task Speech(string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,string speech7,string speech8,ThreadMode threadMode = DefaultThreadMode,[CallerLineNumber] int threadID = 0) {
            var threadValue = GetThreadValue(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5,speech6,speech7,speech8));
            if(threadValue == null) {
                return;
            }
            await Script.Speech(threadValue);
        }
    }
}
