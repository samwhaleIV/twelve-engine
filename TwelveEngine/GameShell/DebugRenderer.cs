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
            int lineHeight = DebugFont.LineSpacing;
            Vector2 position = new Vector2 {
                X = Constants.ScreenEdgePadding,
                Y = Viewport.Height - lineHeight * 4 - Constants.ScreenEdgePadding * 0.5f
            };
            SpriteBatch.Begin();

            updateTime.TotalMilliseconds.ToString();
            SpriteBatch.DrawString(DebugFont,proxyGameTime.TotalGameTime.ToString("hh\\:mm\\:ss\\:ff"),position,Color.White);
            position.Y += lineHeight;
            SpriteBatch.DrawString(DebugFont,$"FPS: {getFps():F}",position,Color.White);
            position.Y += lineHeight;
            SpriteBatch.DrawString(DebugFont,$"Update: {string.Format("{0:0.00}",updateTime.TotalMilliseconds)}ms",position,Color.White);
            position.Y += lineHeight;
            SpriteBatch.DrawString(DebugFont,$"Render: {string.Format("{0:0.00}",renderTime.TotalMilliseconds)}ms",position,Color.White);
            SpriteBatch.End();
        }
    }
}
#endif