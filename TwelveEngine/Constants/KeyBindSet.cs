using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using TwelveEngine.Shell.Config;

namespace TwelveEngine.Shell.Config {
    public sealed class KeyBindSet {
        public Keys Recording = Keys.F3;
        public Keys Playback = Keys.F4;

        public Keys PauseGame = Keys.F5;
        public Keys AdvanceFrame = Keys.F6;

        public Keys Up = Keys.W;
        public Keys Down = Keys.S;
        public Keys Left = Keys.A;
        public Keys Right = Keys.D;

        public Keys Accept = Keys.E;
        public Keys Cancel = Keys.Escape;

        public Keys Ascend = Keys.Space;
        public Keys Descend = Keys.LeftControl;

        public Keys Toggle = Keys.Tab;
    }
}
namespace TwelveEngine.Shell.Input {
    public sealed partial class KeyBinds {
        private static Dictionary<Impulse,Keys> GetBinds(KeyBindSet keyBindSet) => new() {
            { Impulse.Up, keyBindSet.Up },
            { Impulse.Down, keyBindSet.Down },
            { Impulse.Left, keyBindSet.Left },
            { Impulse.Right, keyBindSet.Right },

            { Impulse.Accept, keyBindSet.Accept },
            { Impulse.Cancel, keyBindSet.Cancel },

            { Impulse.Ascend, keyBindSet.Ascend },
            { Impulse.Descend, keyBindSet.Descend },

            { Impulse.Toggle, keyBindSet.Toggle }
        };
    }
}
