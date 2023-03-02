using System;

namespace Elves.Battle.Scripting {
    [Flags]
    public enum ThreadMode {
        /// <summary>
        /// Do not repeat the list. Fall through the thread when the final index has already been activated.
        /// </summary>
        StopAtEnd        = 0b_0000_0000,
        /// <summary>
        /// Repeat, from first to last. May be modified by <see cref="SkipFirst"/>.
        /// </summary>
        Repeat          = 0b_0000_0001,
        /// <summary>
        /// During a repeat, skip the first element. The first element is always activated on the first loop.
        /// </summary>
        SkipFirst       = 0b_0000_0011,
        /// <summary>
        /// Repeats the last element once it has been reached. Incompatiable with <see cref="Repeat"/>.
        /// </summary>
        HoldLast        = 0b_0000_0100,
        /// <summary>
        /// Selects a random element. Incompatiable with all other flags.
        /// </summary>
        Random          = 0b_0000_1000
    }
}
