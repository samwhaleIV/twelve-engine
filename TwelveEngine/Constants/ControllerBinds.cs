using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TwelveEngine.Input {
    using static ControllerBinds;

    internal static class ControllerBinds {
        public const Buttons Up = Buttons.DPadUp;
        public const Buttons Down = Buttons.DPadDown;
        public const Buttons Left = Buttons.DPadLeft;
        public const Buttons Right = Buttons.DPadRight;

        public const Buttons Accept = Buttons.A;
        public const Buttons Cancel = Buttons.B;

        public const Buttons Ascend = Buttons.LeftShoulder;
        public const Buttons Descend = Buttons.RightShoulder;

        public const Buttons Toggle = Buttons.Back;
    }

    public sealed partial class ImpulseHandler {
        private static Dictionary<Impulse,Buttons> GetControllerBinds() => new Dictionary<Impulse,Buttons>() {
            { Impulse.Up, Up },
            { Impulse.Down, Down },
            { Impulse.Left, Left },
            { Impulse.Right, Right },
            { Impulse.Accept, Accept },
            { Impulse.Cancel, Cancel },
            { Impulse.Ascend, Ascend },
            { Impulse.Descend, Descend },
            { Impulse.Toggle, Toggle }
        };
    }
}
