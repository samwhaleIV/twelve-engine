namespace TwelveEngine.UI.Book {
    public struct ElementLayoutData {

        public Vector2 Position { get; set; }
        public Vector2 Size { get; set; }
        public Vector2 Offset { get; set; }

        public float Scale { get; set; }
        public float Rotation { get; set; }

        public Color Color { get; set; }

        public static ElementLayoutData Default => new() {
            Position = Vector2.Zero,
            Size = Vector2.Zero,
            Offset = Vector2.Zero,
            Scale = 0f,
            Rotation = 0f,
            Color = Color.White
        };

        public static ElementLayoutData Lerp(ElementLayoutData a,ElementLayoutData b,float amount) {
            return new ElementLayoutData() {
                Position = Vector2.Lerp(a.Position,b.Position,amount),
                Size = Vector2.Lerp(a.Size,b.Size,amount),
                Scale = MathHelper.Lerp(a.Scale,b.Scale,amount),
                Rotation = MathHelper.Lerp(a.Rotation,b.Rotation,amount),
                Offset = Vector2.Lerp(a.Offset,b.Offset,amount),
                Color = Color.Lerp(a.Color,b.Color,amount)
            };
        }

        private static Color SmoothStepColor(Color a,Color b,float amount) {
            amount = (int)MathHelper.Clamp(amount,0,1);
            int red = (int)MathHelper.SmoothStep(a.R,b.R,amount);
            int green = (int)MathHelper.SmoothStep(a.G,b.G,amount);
            int blue = (int)MathHelper.SmoothStep(a.B,b.B,amount);
            int alpha = (int)MathHelper.SmoothStep(a.A,b.A,amount);
            return new Color(red,green,blue,alpha);
        }

        public static ElementLayoutData SmoothStep(ElementLayoutData a,ElementLayoutData b,float amount) {
            return new ElementLayoutData() {
                Position = Vector2.SmoothStep(a.Position,b.Position,amount),
                Size = Vector2.SmoothStep(a.Size,b.Size,amount),
                Scale = MathHelper.SmoothStep(a.Scale,b.Scale,amount),
                Rotation = MathHelper.SmoothStep(a.Rotation,b.Rotation,amount),
                Offset = Vector2.SmoothStep(a.Offset,b.Offset,amount),
                Color = SmoothStepColor(a.Color,b.Color,amount)
            };
        }

        public static ElementLayoutData Interpolate(ElementLayoutData a,ElementLayoutData b,float amount,bool smoothStep) {
            return smoothStep ? SmoothStep(a,b,amount) : Lerp(a,b,amount);
        }
    }
}
