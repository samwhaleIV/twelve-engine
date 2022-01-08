using Microsoft.Xna.Framework;

namespace TwelveEngine.TileGen {
    internal sealed class Painter {

        public static readonly Color NoFillColor = Color.Transparent;

        private readonly Color color;
        public Painter(Color color) {
            this.color = color;
        }
        public Color Paint(bool value) {
            if(!value) {
                return NoFillColor;
            }
            return color;
        }
    }
}
