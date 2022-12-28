using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Input {
    public sealed class KeyBindSet {
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
