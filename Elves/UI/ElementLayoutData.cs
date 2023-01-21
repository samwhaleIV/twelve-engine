using Microsoft.Xna.Framework;

namespace Elves.UI {
    public struct ElementLayoutData {

        public Vector2 Position;
        public Vector2 Size;

        public Vector2 Offset;

        public float Scale;
        public float Rotation;

        public ElementLayoutData() {
            Offset = Vector2.Zero;
            Position = Vector2.Zero;
            Size = Vector2.Zero;
            Rotation = 0f;
            Scale = 0f;
        }

        public static ElementLayoutData Lerp(ElementLayoutData a,ElementLayoutData b,float amount) {
            return new ElementLayoutData() {
                Position = Vector2.Lerp(a.Position,b.Position,amount),
                Size = Vector2.Lerp(a.Size,b.Size,amount),
                Scale = MathHelper.Lerp(a.Scale,b.Scale,amount),
                Rotation = MathHelper.Lerp(a.Rotation,b.Rotation,amount),
                Offset = Vector2.Lerp(a.Offset,b.Offset,amount)
            };
        }
    }
}
