using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public readonly struct ScreenSpace {

        public ScreenSpace(Vector2 position,Vector2 size,int tileSize) {
            Location = position;
            Size = size;

            TileSize = tileSize;
        }

        public readonly Vector2 Location;
        public readonly Vector2 Size;

        public readonly int TileSize;

        public Vector2 GetCenter() {
            return Location + Size * 0.5f;
        }

        public float X => Location.X;
        public float Y => Location.Y;

        public float Width => Size.X;
        public float Height => Size.Y;
    }
}
