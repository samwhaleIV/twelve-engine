﻿using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class KeyBindSet {
        public Keys Up = Keys.W;
        public Keys Down = Keys.S;
        public Keys Left = Keys.A;
        public Keys Right = Keys.D;

        public Keys Focus = Keys.Tab;

        public Keys Accept = Keys.Enter;
        public Keys Cancel = Keys.Escape;

        public Keys Ascend = Keys.Space;
        public Keys Descend = Keys.LeftControl;
    }
}
