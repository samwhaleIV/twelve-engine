using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Input.Glyphs {
    internal sealed class GamePadMap:GlyphMap<Buttons> {
        public GamePadMap() {
            BlockColumns = 4;
            GlyphSize = 16;
        }
        protected override Buttons[] GetList() => new Buttons[] {
            Buttons.A,
            Buttons.B,
            Buttons.X,
            Buttons.Y,
            Buttons.DPadUp,
            Buttons.DPadDown,
            Buttons.DPadLeft,
            Buttons.DPadRight,
            Buttons.LeftShoulder,
            Buttons.RightShoulder,
            Buttons.LeftTrigger,
            Buttons.RightTrigger,
            Buttons.LeftStick,
            Buttons.RightStick,
            Buttons.Back,
            Buttons.Start
        };
    }
}
