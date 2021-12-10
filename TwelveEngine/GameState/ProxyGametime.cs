using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    internal sealed class ProxyGameTime:GameTime {

        private readonly TimeSpan MAX_ELAPSED_TIME = TimeSpan.FromMilliseconds(Constants.MaximumFrameDelta);

        private TimeSpan pauseTimeOffset = TimeSpan.Zero;

        private bool frozen = false;
        private bool totalTimeLocked = false;
        private bool shouldResetTime = true;

        internal void Update(GameTime gameTime) {
            if(shouldResetTime) {
                AddPauseTime(gameTime.TotalGameTime - pauseTimeOffset - TotalGameTime);
                shouldResetTime = false;
            }
            if(frozen) {
                return;
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
        internal void Freeze() {
            ElapsedGameTime = TimeSpan.Zero;
            frozen = true;
        }
        internal void Unfreeze() {
            frozen = false;
        }
        internal void AddPauseTime(TimeSpan pauseTime) {
            pauseTimeOffset += pauseTime;
        }

        internal void LockTime() {
            totalTimeLocked = true;
        }
        internal void UnlockTime() {
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
