using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed partial class ImpulseHandler {
        private static Dictionary<Impulse,Buttons> GetControllerBinds() => new Dictionary<Impulse,Buttons>() {
            { Impulse.Accept, Buttons.A },
            { Impulse.Exit, Buttons.B },
            { Impulse.Up, Buttons.DPadUp },
            { Impulse.Down, Buttons.DPadDown },
            { Impulse.Left, Buttons.DPadLeft },
            { Impulse.Right, Buttons.DPadRight }
        };
    }
}
