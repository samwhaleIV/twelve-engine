using System;

namespace Elves.UI {
    [Flags]
    public enum ElementFlags {
        None = 0b00,
        CanUpdate = 0b01,
        Interactable = 0b10,
        UpdateAndInteract = 0b11
    }
}
