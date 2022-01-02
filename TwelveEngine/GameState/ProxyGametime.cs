using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    internal sealed class ProxyGameTime:GameTime {

        private readonly TimeSpan MAX_ELAPSED_TIME = TimeSpan.FromMilliseconds(Constants.MaxFrameDelta);

        private TimeSpan pauseTimeOffset = TimeSpan.Zero;

        private bool shouldResetTime = false;

        internal void Update(GameTime gameTime) {
            if(shouldResetTime) {
                AddPauseTime(gameTime.TotalGameTime - pauseTimeOffset - TotalGameTime);
                shouldResetTime = false;
            }

            if(freezeCount == 0) {
                var elapsedTime = gameTime.ElapsedGameTime;
                if(elapsedTime > MAX_ELAPSED_TIME) {
                    elapsedTime = MAX_ELAPSED_TIME;
                }
                ElapsedGameTime = elapsedTime;
                TotalGameTime = gameTime.TotalGameTime - pauseTimeOffset;
            }
        }
        internal void AddPauseTime(TimeSpan pauseTime) {
            pauseTimeOffset += pauseTime;
        }

        private int freezeCount = 0;

        internal void Freeze() {
            freezeCount += 1;
        }
        internal void Unfreeze() {
            if(freezeCount == 0) {
                return;
            }
            freezeCount -= 1;
            if(freezeCount != 0) {
                return;
            }
            shouldResetTime = true;
        }

        internal void AddSimTime(TimeSpan elapsedTime) {
            ElapsedGameTime = elapsedTime;
            TotalGameTime += elapsedTime;
        }
    }
}
