using System;

namespace TwelveEngine.Shell {
    [Flags]
    public enum StateFlags {
        None = 0x00000000,

        CarryKeyboardInput  = 0x00000001,
        CarryMouseInput     = 0x00000010,
        CarryInput          = 0x00000011,

        /* Works independently of Config.Keys.StateCleanUpGC. This is for when your state leaves a huge fucking mess. */
        ForceGC = 0x00000100,

        /* It's up to the receiver whether or not they want to respect StateFlags.FadeIn.
         * It's a hint that we faded out to get the the new state. */
        FadeIn = 0x00001000
    }
}
