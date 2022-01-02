using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TwelveEngine {
    internal static class KeyBinds {
        public const Keys Recording = Keys.F3;
        public const Keys Playback = Keys.F4;

        public const Keys PauseGame = Keys.F5;
        public const Keys AdvanceFrame = Keys.F6;

        public const Keys SaveState = Keys.F1;
        public const Keys LoadState = Keys.F2;

        public const Keys Up = Keys.W;
        public const Keys Down = Keys.S;
        public const Keys Left = Keys.A;
        public const Keys Right = Keys.D;

        public const Keys Accept = Keys.E;
        public const Keys Cancel = Keys.Escape;
    }
}

namespace TwelveEngine.Input {
    public sealed partial class KeyBindSet {
        private static Dictionary<Impulse,Keys> GetKeyBinds() => new Dictionary<Impulse,Keys>() {
            { Impulse.Up, KeyBinds.Up },
            { Impulse.Down, KeyBinds.Down },
            { Impulse.Left, KeyBinds.Left },
            { Impulse.Right, KeyBinds.Right },

            { Impulse.Accept, KeyBinds.Accept },
            { Impulse.Cancel, KeyBinds.Cancel }
        };
    }
}
