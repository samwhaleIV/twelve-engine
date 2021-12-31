using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if DEBUG
namespace TwelveEngine {
    public sealed partial class GameManager:Game {

        private static readonly Stopwatch watch = new Stopwatch();
        private TimeSpan updateTime = TimeSpan.Zero, renderTime = TimeSpan.Zero;

        private double getFps() {
            var totalSeconds = proxyGameTime.ElapsedGameTime.TotalSeconds;
            if(totalSeconds == 0) {
                return 0;
            }
            return 1 / totalSeconds;
        }

        private void renderGameTime() {
            Vector2 position = new Vector2 {
                X = 4,
                Y = GraphicsDevice.Viewport.Height - 99
            };
            SpriteBatch.Begin();

            updateTime.TotalMilliseconds.ToString();
            spriteBatch.DrawString(spriteFont,proxyGameTime.TotalGameTime.ToString("hh\\:mm\\:ss\\:ff"),position,Color.White);
            position.Y += 25;
            spriteBatch.DrawString(spriteFont,$"FPS: {getFps():F}",position,Color.White);
            position.Y += 25;
            spriteBatch.DrawString(spriteFont,$"Update: {string.Format("{0:0.00}",updateTime.TotalMilliseconds)}ms",position,Color.White);
            position.Y += 25;
            spriteBatch.DrawString(spriteFont,$"Render: {string.Format("{0:0.00}",renderTime.TotalMilliseconds)}ms",position,Color.White);
            spriteBatch.End();
        }
    }
}
#endif