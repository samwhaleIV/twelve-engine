using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine {
    public static class Graphics2D {
        public static void DrawCentered(SpriteBatch spriteBatch,Texture2D texture,Vector2 origin) {
            Vector2 destination = new Vector2(
                origin.X - texture.Width / 2,
                origin.Y - texture.Height / 2
            );
            spriteBatch.Draw(texture,destination,Color.White);
        }
        public static void DrawCentered(SpriteBatch spriteBatch,Texture2D texture,Game game) {
            DrawCentered(spriteBatch,texture,GetScreenCenter(game));
        }
        public static Viewport GetViewport(Game game) {
            return game.GraphicsDevice.Viewport;
        }
        public static Vector2 GetScreenCenter(Game game) {
            var viewport = GetViewport(game);
            Vector2 screenCenter = new Vector2(viewport.Width / 2,viewport.Height / 2);
            return screenCenter;
        }
        public static Rectangle GetDestination(float x,float y,float tileSize) {
            int size = (int)tileSize;
            return new Rectangle((int)Math.Floor(x),(int)Math.Floor(y),size,size);
        }
    }
}
