using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Elves {
    public sealed class Textures {

        internal static ContentManager ContentManager { get; set; }
        private static Texture2D Load(string file) => ContentManager.Load<Texture2D>(file);

        public readonly Texture2D Panel = Load("panel");
        public readonly Texture2D Nothing = Load("nothing-white");
        public readonly Texture2D CursorDefault = Load("Cursor/default");
        public readonly Texture2D CursorAlt1 = Load("Cursor/alt-1");
        public readonly Texture2D CursorAlt2 = Load("Cursor/alt-2");
        public readonly Texture2D Drowning = Load("drowning");
        public readonly Texture2D Carousel = Load("carousel");
        public readonly Texture2D Missing = Load("missing");
        public readonly Texture2D Mountains = Load("mountains");
        public readonly Texture2D SaveSelect = Load("save-select");
        public readonly Texture2D  GiftPattern = Load("gift-pattern");
        public readonly Texture2D QuadColor = Load("quad-color");
        public readonly Texture2D Circle = Load("circle");
    }
}
