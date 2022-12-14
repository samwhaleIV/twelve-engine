using System;

namespace Elves.Battle {
    [Flags]
    public enum ThreadMode {
        NoRepeat, SkipFirstOnRepeat, RepeatLast, Random
    }
}
