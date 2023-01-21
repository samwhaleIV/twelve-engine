using Microsoft.Xna.Framework;
using System;

namespace Elves {
    public static class Constants {
        public const string SaveFolder = "ElvesGame";
        public const string SaveFile = "elves.save";

        public const float Debug3DMovementSpeed = 0.75f;
        public const float Debug3DLookSpeed = 10f;

        public static class Flags {
            public const string Debug = "debug";
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

        public static class UI {
            public static readonly Color PressedColor = Color.Lerp(Color.White,Color.Black,0.1f);
            public static readonly Color SelectColor = Color.Lerp(Color.White,Color.Black,0.05f);
        }

        public static class Depth {
            public const float DeepBackground = 0 / 8f;
            public const float Background = 1 / 8f;
            public const float MiddleFarthest = 2 / 8f;
            public const float MiddleFar = 3 / 8f;
            public const float Middle = 4 / 8f;
            public const float MiddleClose = 5 / 8f;
            public const float MiddleCloser = 6 / 8f;
            public const float Foreground = 7 / 8f;
            public const float SuperForeground = 8 / 8f;
            public const float Cam = 9 / 8f;
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

            public static readonly TimeSpan IntroStartDelay = TimeSpan.FromSeconds(1);
            public static readonly TimeSpan IntroFadeOutDelay = TimeSpan.FromSeconds(1);
            public static readonly TimeSpan IntroFadeOutDuration = TimeSpan.FromSeconds(9);
            public static readonly TimeSpan IntroSongDurationOffset = -TimeSpan.FromSeconds(0f);

            public const float IntroTextFadeSpeed = 2f;

            public static readonly TimeSpan TransitionDuration = TimeSpan.FromSeconds(1f);
            public static readonly TimeSpan QuickTransition = TimeSpan.FromSeconds(0.1f);
        }

        public static class Songs {
            public const string Intro = "Music/A_hero";
            public const string UV2T3 = "Music/UV2T3";
        }
    }
}
