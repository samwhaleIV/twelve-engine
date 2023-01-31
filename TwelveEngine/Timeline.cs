using TwelveEngine.Shell.UI;

namespace TwelveEngine {
    public sealed class Timeline {

        public TimeSpan Duration { get; private set; }
        public TimeSpan StartTimeOffset { get; set; }

        public void WriteDebug(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(GlobalT,nameof(GlobalT));
            writer.Write(LocalT,nameof(LocalT));
            writer.Write(Stage,nameof(Stage));
            writer.Write(Duration,nameof(Duration));
        }

        public void Update(TimeSpan now) {
            double t = (now + StartTimeOffset) / Duration;

            if(t >= 1) {
                Stage = timeline[^1].Stage;
                GlobalT = 1;
                LocalT = 1;
                return;
            } else if(t <= 0) {
                Stage = 0;
                GlobalT = 0;
                LocalT = 0;
                return;
            } else {
                Stage = 0;
                GlobalT = (float)t;
                LocalT = 0;
            }

            for(int i = 1;i<timeline.Count;i++) {
                (int Stage, double End) item = timeline[i];
                double start = timeline[i-1].End;
                double localT = (t - start) / (item.End - start);
                if(localT <= 1) {
                    Stage = item.Stage;
                    LocalT = (float)localT;
                    return;
                }
            }
        }

        private readonly List<(int Stage, double End)> timeline = new() { (0, 0) };

        public int Stage { get; private set; } = 0;

        public float GlobalT { get; private set; } = 0;
        public float LocalT { get; private set; } = 0;

        private void ResetTimeline() {
            timeline.Clear();
            timeline.Add((0, 0));
        }

        private void AddStage(int stage,TimeSpan length) {
            timeline.Add((stage, length / Duration + timeline[^1].End));
        }

        public void CreateFixedDuration(TimeSpan duration,params (int Stage, TimeSpan Length)[] stages) {
            ResetTimeline();
            Duration = duration;
            foreach(var stage in stages) {
                AddStage(stage.Stage,stage.Length);
            }
        }

        public void CreateAutoDuration(params (int Stage, TimeSpan Length)[] stages) {
            ResetTimeline();
            TimeSpan duration = TimeSpan.Zero;
            foreach(var stage in stages) {
                duration += stage.Length;
            }
            Duration = duration;
            foreach(var stage in stages) {
                AddStage(stage.Stage,stage.Length);
            }
        }
    }
}
