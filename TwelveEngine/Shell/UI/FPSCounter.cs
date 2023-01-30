using System;

namespace TwelveEngine.Shell.UI {
    public sealed class FPSCounter {

        private readonly TimeSpan frequency;

        public FPSCounter(TimeSpan? frequency = null) {
            this.frequency = frequency ?? Constants.FrameTimeFrequency;
        }

        private int frameCount;
        private TimeSpan startTime;

        public void Update(TimeSpan currentTime) {
            frameCount += 1;
            double t = (currentTime - startTime) / frequency;
            if(t < 1) {
                return;
            }
            TimeSpan duration = t * frequency;
            FPS = frameCount / duration.TotalSeconds;
            frameCount = 0;
            startTime = currentTime;
        }

        public double FPS { get; private set; }
    }
}
