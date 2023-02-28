using System;

namespace Elves.Battle {
    [Flags]
    public enum ThreadMode {
        NoRepeat = 0, Repeat, SkipFirst, HoldLast, Random
    }
}
