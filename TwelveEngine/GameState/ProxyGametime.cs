using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    internal sealed class ProxyGameTime:GameTime {

        private readonly TimeSpan MAX_ELAPSED_TIME = TimeSpan.FromMilliseconds(Constants.MaximumFrameDelta);

        private TimeSpan pauseTimeOffset = TimeSpan.Zero;

        private bool totalTimeLocked = false, shouldResetTime = false;

        internal void Update(GameTime gameTime) {
            if(shouldResetTime) {
                AddPauseTime(gameTime.TotalGameTime - pauseTimeOffset - TotalGameTime);
                shouldResetTime = false;
            }
            var elapsedTime = gameTime.ElapsedGameTime;
            if(elapsedTime > MAX_ELAPSED_TIME) {
                elapsedTime = MAX_ELAPSED_TIME;
            }
            ElapsedGameTime = elapsedTime;
            if(!totalTimeLocked) {
                TotalGameTime = gameTime.TotalGameTime - pauseTimeOffset;
            }
        }
        internal void AddPauseTime(TimeSpan pauseTime) {
            pauseTimeOffset += pauseTime;
        }

        internal void Freeze() {
            totalTimeLocked = true;
        }
        internal void Unfreeze() {
            if(!totalTimeLocked) {
                return;
            }
            totalTimeLocked = false;
            shouldResetTime = true;
        }
        internal void SetPlaybackTime(TimeSpan elapsedTime) {
            ElapsedGameTime = elapsedTime;
            TotalGameTime += elapsedTime;
        }
        internal void AddSimframe(TimeSpan simTime) {
            ElapsedGameTime = simTime;
            TotalGameTime += simTime;
        }
    }
}
