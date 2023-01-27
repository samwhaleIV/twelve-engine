using System;

namespace TwelveEngine.Shell {
    public struct StateData {
        public StateFlags Flags { get; set; }
        public TimeSpan TransitionDuration { get; set; }

        public string[] Args { get; set; }

        public static readonly StateData Empty = new() { 
            Args = null,
            Flags = StateFlags.None
        };

        public static StateData FadeIn(TimeSpan duration) => new() {
            Args = null,
            TransitionDuration = duration,
            Flags = StateFlags.FadeIn
        };
    }
}
