using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Elves.Battle.Scripting {
    public sealed partial class ScriptThreader {
        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,string tag3,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2,tag3),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,string tag3,string tag4,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2,tag3,tag4),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,string tag3,string tag4,string tag5,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5,tag6),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,string tag7,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5,tag6,tag7),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Tag(ThreadMode threadMode,string tag1,string tag2,string tag3,string tag4,string tag5,string tag6,string tag7,string tag8,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(tag1,tag2,tag3,tag4,tag5,tag6,tag7,tag8),out var tag)) {
                return;
            }
            await Script.Tag(tag);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,string speech3,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2,speech3),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,string speech3,string speech4,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2,speech3,speech4),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,string speech3,string speech4,string speech5,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5,speech6),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,string speech7,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5,speech6,speech7),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }

        public async Task Speech(ThreadMode threadMode,string speech1,string speech2,string speech3,string speech4,string speech5,string speech6,string speech7,string speech8,[CallerLineNumber] int threadID = 0) {
            if(TryGetThreadValue<string>(threadID,threadMode,new(speech1,speech2,speech3,speech4,speech5,speech6,speech7,speech8),out var speech)) {
                return;
            }
            await Script.Speech(speech);
        }
    }
}
