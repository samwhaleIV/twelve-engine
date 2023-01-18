using System;
using System.Collections.Generic;

namespace TwelveEngine {
    public sealed class SmoothTimeline {

        public SmoothTimeline(TimeSpan duration) {
            Duration = duration;
        }

        public TimeSpan Duration { get; private set; }
        public TimeSpan StartTimeOffset { get; set; }

        public void Update(TimeSpan now) {
            double t = (now - StartTimeOffset) / Duration;

            if(t >= 1) {
                Stage = timeline[^1].Stage;
                GlobalT = 1;
                LocalT = 1;
                return;
            } else if(t < 0) {
                Stage = 0;
                GlobalT = 0;
                LocalT = 0;
                return;
            } else {
                GlobalT = (float)t;
                LocalT = 0;
            }

            for(int i = 1;i<timeline.Count;i++) {
                (int Stage, double KeyFrame) item = timeline[i];
                double start = timeline[i-1].KeyFrame;
                double localT = (t - start) / (item.KeyFrame - start);
                if(localT <= 1) {
                    Stage = item.Stage;
                    LocalT = (float)localT;
                }
            }
        }

        private List<(int Stage, double KeyFrame)> timeline = new() { (0, 0) };

        public int Stage { get; private set; } = 0;
        public float GlobalT { get; private set; } = 0;
        public float LocalT { get; private set; } = 0;

        public void AddStage(int stage,TimeSpan length) {
            timeline.Add((stage,length/Duration+timeline[^1].KeyFrame));
        }
    }
}
