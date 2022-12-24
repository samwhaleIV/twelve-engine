using System;

namespace TwelveEngine.Shell {
    public sealed class FrameTimeSmoother {

        public static TimeSpan DefaultFrequency => TimeSpan.FromMilliseconds(250);

        private readonly TimeSpan frequency;

        public FrameTimeSmoother(TimeSpan? frequency = null) {
            this.frequency = frequency ?? DefaultFrequency;
        }

        private int frameCount;
        private TimeSpan startTime;

        private TimeSpan accumulator;

        public void Update(TimeSpan currentTime,TimeSpan elapsedTime) {
            frameCount += 1;
            accumulator += elapsedTime;
            double t = (currentTime - startTime) / frequency;
            if(t < 1) {
                return;
            }
            Average = accumulator / frameCount;
            accumulator = TimeSpan.Zero;
            frameCount = 0;
            startTime = currentTime;
        }

        public TimeSpan Average { get; private set; } = TimeSpan.Zero;
    }
}
