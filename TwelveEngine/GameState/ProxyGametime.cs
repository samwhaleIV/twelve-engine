using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    internal sealed class ProxyGameTime:GameTime {

        private readonly TimeSpan MAX_ELAPSED_TIME = TimeSpan.FromMilliseconds(Constants.MaximumFrameDelta);

        TimeSpan pauseTimeOffset = TimeSpan.Zero;

        private bool frozen = false;
        private bool totalTimeLocked = false;
        private bool shouldResetTime = false;

        internal void Update(GameTime gameTime) {
            if(shouldResetTime) {
                AddPauseTime(gameTime.TotalGameTime - pauseTimeOffset - this.TotalGameTime);
                shouldResetTime = false;
            }
            if(frozen) {
                return;
            }
            var elapsedTime = gameTime.ElapsedGameTime;
            if(elapsedTime > MAX_ELAPSED_TIME) {
                elapsedTime = MAX_ELAPSED_TIME;
            }
            this.ElapsedGameTime = elapsedTime;
            if(!totalTimeLocked) {
                this.TotalGameTime = gameTime.TotalGameTime - pauseTimeOffset;
            }
        }
        internal void Freeze() {
            this.ElapsedGameTime = TimeSpan.Zero;
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
        internal void SetPlaybackTime(TimeSpan elapsedTime,TimeSpan totalTime) {
            this.ElapsedGameTime = elapsedTime;
            if(Constants.RecompileAutomationTiming) {
                this.TotalGameTime += elapsedTime;
            } else {
#pragma warning disable CS0162
                this.TotalGameTime = totalTime;
#pragma warning restore CS0162
            }
        }
        internal void AddSimframe(TimeSpan simTime) {
            this.ElapsedGameTime = simTime;
            this.TotalGameTime += simTime;
        }
    }
}
