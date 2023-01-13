using Microsoft.Xna.Framework;
using System;

namespace Elves {
    public static class Constants {
        public const string SaveFolder = "ElvesGame";
        public const string SaveFile = "elves.save";

        public const float Debug3DMovementSpeed = 0.75f;
        public const float Debug3DLookSpeed = 10f;

        public static class Flags {
            public const string Debug3D = "debug3d";
            public const string OSCursor = "oscursor";
            public const string FixedBattleRandom = "norandombattle";
        }

        public static class Battle {
            public const int FixedSeed = 27;
            public const int DefaultHealth = 100;
            public const string NoName = "<No Name>";
            public const string PlayerName = "You";
            public const string ContinueText = "Continue";
            public static readonly Color DefaultUserColor = Color.White;
        }

        public static class AnimationTiming {
            public static readonly TimeSpan ScrollingBackgroundDefault = TimeSpan.FromSeconds(60);

            public static readonly TimeSpan ActionButtonMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan SpeechBoxMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan TargetMovementDuration = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan TaglineMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan TaglineTextMovement = TimeSpan.FromMilliseconds(150);

            public static readonly TimeSpan HealthDropDuration = TimeSpan.FromMilliseconds(100);

            public static readonly TimeSpan CarouselRotationDuration = TimeSpan.FromMilliseconds(300);
            public static readonly TimeSpan CarouselBackgroundScroll = TimeSpan.FromSeconds(120);
        }
    }
}
