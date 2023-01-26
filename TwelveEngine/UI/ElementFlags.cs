using System;

namespace TwelveEngine.UI {
    [Flags]
    public enum ElementFlags {
        None = 0b00,
        Update = 0b01,
        Interact = 0b10,
        UpdateAndInteract = 0b11
    }
}
