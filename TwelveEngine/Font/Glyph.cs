using Microsoft.Xna.Framework;

namespace TwelveEngine.Font {
    public readonly struct Glyph {
        public readonly Rectangle Source;

        public readonly int YOffset;

        public Glyph(int x,int y,int width,int height,int yOffset) {
            Source = new Rectangle(x,y,width,height);
            YOffset = yOffset;
        }

        public Glyph() {
            Source = Rectangle.Empty;
            YOffset = 0;
        }
    }
}
