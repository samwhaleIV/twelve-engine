using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Shell {
    public class TransitionRenderer {

        public static readonly TransitionRenderer Default = new TransitionRenderer();

        private static void DrawDefault(Texture2D texture,Rectangle bounds,SpriteBatch spriteBatch,float t) {
            /* Linear fade in/out to black */
            spriteBatch.Begin(SpriteSortMode.Immediate,BlendState.AlphaBlend,SamplerState.PointClamp);
            spriteBatch.Draw(texture,bounds,Color.FromNonPremultiplied(new Vector4(0,0,0,t)));
            spriteBatch.End();
        }

        public virtual void DrawIn(GameManager game,float t) {
            DrawDefault(game.EmptyTexture,game.Viewport.Bounds,game.SpriteBatch,1-t);
        }

        public virtual void DrawOut(GameManager game,float t) {
            DrawDefault(game.EmptyTexture,game.Viewport.Bounds,game.SpriteBatch,t);
        }
    }
}
