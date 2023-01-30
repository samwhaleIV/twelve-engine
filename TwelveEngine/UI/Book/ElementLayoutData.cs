using Microsoft.Xna.Framework;

namespace TwelveEngine.UI.Book {
    public struct ElementLayoutData {

        public Vector2 Position, Size, Offset;

        public float Scale, Rotation;

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

        public static ElementLayoutData SmoothStep(ElementLayoutData a,ElementLayoutData b,float amount) {
            return new ElementLayoutData() {
                Position = Vector2.SmoothStep(a.Position,b.Position,amount),
                Size = Vector2.SmoothStep(a.Size,b.Size,amount),
                Scale = MathHelper.SmoothStep(a.Scale,b.Scale,amount),
                Rotation = MathHelper.SmoothStep(a.Rotation,b.Rotation,amount),
                Offset = Vector2.SmoothStep(a.Offset,b.Offset,amount)
            };
        }

        public static ElementLayoutData Interpolate(ElementLayoutData a,ElementLayoutData b,float amount,bool smoothStep) {
            return smoothStep ? SmoothStep(a,b,amount) : Lerp(a,b,amount);
        }
    }
}
