using System;

namespace Elves.Scenes.Battle {
    [Flags]
    public enum ThreadMode {
        NoRepeat, SkipFirstOnRepeat, RepeatLast, Random
    }
}
