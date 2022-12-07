using System;

namespace TwelveEngine.Shell {
    public sealed class FPSCounter {

        public static TimeSpan DefaultFrequency => TimeSpan.FromMilliseconds(250);

        private readonly TimeSpan frequency;

        public FPSCounter(TimeSpan? frequency = null) {
            this.frequency = frequency ?? DefaultFrequency;
        }

        private int frameCounter;
        private double fps;
        private TimeSpan startTime;

        public void Update(TimeSpan currentTime) {
            frameCounter += 1;
            double t = (currentTime - startTime) / frequency;
            if(t < 1) {
                return;
            }
            TimeSpan duration = t * frequency;
            fps = frameCounter / duration.TotalSeconds;
            frameCounter = 0;
            startTime = currentTime;
        }

        public double Value => fps;
    }
}
