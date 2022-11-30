using System;

namespace Elves.Battle.Script {
    [Flags]
    public enum ThreadMode {
        NoRepeat, SkipFirstOnRepeat, RepeatLast, Random
    }
}
