namespace TwelveEngine.Shell {
    public class TransitionRenderer {

        public static readonly TransitionRenderer Default = new();

        private static void DrawDefault(SpriteBatch spriteBatch,Texture2D texture,Rectangle bounds,float t) {
            /* Linear fade in/out to black */
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend,SamplerState.PointClamp);
            spriteBatch.Draw(texture,bounds,Color.FromNonPremultiplied(new Vector4(0,0,0,t)));
            spriteBatch.End();
        }

        public virtual void DrawIn(SpriteBatch spriteBatch,Rectangle bounds,float t) {
            DrawDefault(spriteBatch,RuntimeTextures.Empty,bounds,1-t);
        }

        public virtual void DrawOut(SpriteBatch spriteBatch,Rectangle bounds,float t) {
            DrawDefault(spriteBatch,RuntimeTextures.Empty,bounds,t);
        }
    }
}
