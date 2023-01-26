﻿namespace TwelveEngine.Shell {
    public struct StateData {
        public StateFlags Flags { get; set; }
        public string[] Args { get; set; }

        public static readonly StateData Empty = new() { 
            Args = null,
            Flags = StateFlags.None
        };

        public static readonly StateData FadeIn = new() {
            Args = null,
            Flags = StateFlags.FadeIn
        };
    }
}
