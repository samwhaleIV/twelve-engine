using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public readonly struct ScreenSpace {

        public ScreenSpace(Vector2 position,Vector2 size,int tileSize) {
            Position = position;
            Size = size;

            TileSize = tileSize;
        }

        public readonly Vector2 Position;
        public readonly Vector2 Size;

        public readonly int TileSize;

        public Vector2 GetCenter() {
            return Position + Size / 2f;
        }

        public float X => Position.X;
        public float Y => Position.Y;

        public float Width => Size.X;
        public float Height => Size.Y;
    }
}
