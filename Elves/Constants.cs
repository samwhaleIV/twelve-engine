using System;

namespace Elves {
    public static class Constants {
        public const int DefaultHealth = 100;
        public const string NoName = "<No Name>";

        public const string SaveFolder = "ElvesGame";
        public const string SaveFile = "elves.save";

        public const float Debug3DMovementSpeed = 0.75f;
        public const float Debug3DLookSpeed = 10f;

        public static class Flags {
            public const string Debug3D = "debug-3d";
            public const string Fullscreen = "fullscreen";
            public const string HardwareFullscreen = "hw-fullscreen";
            public const string NoVsync = "no-vsync";
            public const string OSCursor = "os-cursor";
        }

        public static class AnimationTiming {
            public static readonly TimeSpan ScrollingBackgroundDefault = TimeSpan.FromSeconds(60);

            public static readonly TimeSpan ActionButtonMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan SpeechBoxMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan TargetMovementDuration = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan TaglineMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan TaglineTextMovement = TimeSpan.FromMilliseconds(150);

            public static readonly TimeSpan HealthDropDuration = TimeSpan.FromMilliseconds(100);
        }
    }
}
