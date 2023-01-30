using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input.Binding {
    public sealed class KeyBindSet {
        public MultiBindKey Up = (Keys.W, Keys.Up);
        public MultiBindKey Down = (Keys.S, Keys.Down);
        public MultiBindKey Left = (Keys.A, Keys.Left);
        public MultiBindKey Right = (Keys.D, Keys.Right);

        public MultiBindKey Focus = Keys.Tab;

        public MultiBindKey Accept = (Keys.Enter, Keys.E);
        public MultiBindKey Cancel = Keys.Escape;

        public MultiBindKey Ascend = Keys.Space;
        public MultiBindKey Descend = Keys.LeftControl;
    }
}
