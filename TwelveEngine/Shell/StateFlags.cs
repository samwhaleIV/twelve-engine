using System;

namespace TwelveEngine.Shell {
    [Flags]
    public enum StateFlags {
        None,
        CarryKeyboardInput,
        CarryMouseInput,
        CarryInput = CarryKeyboardInput | CarryMouseInput,
        ForceGC,
    }
}
