using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine.Shell;

namespace Elves.Battle {
    public abstract class MiniGame {
        public abstract void Render(SpriteBatch spriteBatch,int width,int height);

        public InputGameState GameState { get; private set; }
        protected TimeSpan StartTime { get; private set; } = TimeSpan.Zero;

        public TimeSpan Now => (GameState?.Now ?? TimeSpan.Zero) - StartTime;

        public void UpdateState(InputGameState gameState) {
            GameState = gameState;
            StartTime = gameState?.Now ?? TimeSpan.Zero;
        }

        public abstract void Activate();
        public abstract void Deactivate();
    }
}
