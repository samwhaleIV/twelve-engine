using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

#if DEBUG
namespace TwelveEngine {
    public sealed partial class GameManager:Game {

        private static readonly Stopwatch watch = new Stopwatch();
        private TimeSpan updateTime = TimeSpan.Zero, renderTime = TimeSpan.Zero;

        private void renderGameTime() {
            var position = new Vector2();
            position.X = 4;
            position.Y = GraphicsDevice.Viewport.Height - 74;
            SpriteBatch.Begin();

            updateTime.TotalMilliseconds.ToString();
            spriteBatch.DrawString(spriteFont,proxyGameTime.TotalGameTime.ToString("hh\\:mm\\:ss\\:ff"),position,Color.White);
            position.Y += 25;
            spriteBatch.DrawString(spriteFont,$"Update: {string.Format("{0:0.00}",updateTime.TotalMilliseconds)}ms",position,Color.White);
            position.Y += 25;
            spriteBatch.DrawString(spriteFont,$"Render: {string.Format("{0:0.00}",renderTime.TotalMilliseconds)}ms",position,Color.White);
            spriteBatch.End();
        }
    }
}
#endif