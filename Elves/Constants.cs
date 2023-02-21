using Microsoft.Xna.Framework;
using System;

namespace Elves {
    public static class Constants {
        public const string SaveFolder = "ElvesGame";
        public const string SaveFileFormat = "elves{0}.save";
        public const string GlobalSaveFile = "elves-global.save";

        public const float Debug3DMovementSpeed = 0.75f;
        public const float Debug3DLookSpeed = 10f;

        public const float DefaultSoundVolume = 1f;
        public const float DefaultMusicVolume = 1f;

        public static class Flags {
            public const string OrthoDebug = "orthodebug";
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
            public const int MiniGameWidth = 128;
            public const int MiniGameHeight = 96;
        }

        public static class BattleUI {

            public static readonly Rectangle SpeechBoxSource = new(48,16,72,48);
            public static readonly Rectangle HealthBarSource = new(16,0,16,16);

            public static readonly Rectangle ButtonDefaultSource = new(0,16,48,16);
            public static readonly Rectangle ButtonSelectedSoruce = new(0,32,48,16);
            public static readonly Rectangle ButtonPressedSource = new(0,48,48,16);

            public static readonly Rectangle MiniGameScreenInnerArea = new(100,3,128,96);

            public const float TagBackgroundScale = 2.4f;
            public const float HealthImpactScale = 2;
            public const float ButtonScale = 1.25f;

            public const float NameTextScale = 1 / 2f;
            public const float TagTextScale = 1 / 3f;
            public const float ButtonTextScale = 1 / 3f;
            public const float SpeechBoxTextScale = 1 / 3f;
            public const float SpeechBoxMarginScale = 6f;
            public const float SpeechBoxScale = 1.25f;

            public const float MiniGameScale = 0.75f;

            public const float MarginScale = 1;

            public static readonly TimeSpan ButtonMovement = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan SpeechBoxMovement = TimeSpan.FromMilliseconds(275);
            public static readonly TimeSpan TargetMovementDuration = TimeSpan.FromMilliseconds(250);
            public static readonly TimeSpan TagMovement = TimeSpan.FromMilliseconds(250);
            public static readonly TimeSpan TagTextMovement = TimeSpan.FromMilliseconds(350);

            public static readonly TimeSpan HealthImpact = TimeSpan.FromMilliseconds(80);

            public static readonly TimeSpan MiniGameMovement = TimeSpan.FromMilliseconds(300);
        }

        public static class UI {
            public const float CarouselDirectionButtonOffset = 22;
            public const float CarouselElfNameScale = 0.75f;

            public const float MinUIScale = 0.5f;
            public const float MaxUIScale = 2f;

            public const float SplashMenuScaleModifier = 1.5f;
            public const float BattleSceneScaleModifier = 1.25f;
            public const float UIScaleBaseDivisor = 0.008f;

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

            public static readonly TimeSpan CarouselRotationDuration = TimeSpan.FromMilliseconds(300);
            public static readonly TimeSpan CarouselBackgroundScroll = TimeSpan.FromSeconds(120);

            public static readonly TimeSpan IntroStartDelay = TimeSpan.FromSeconds(0.25f);
            public static readonly TimeSpan IntroFadeOutDelay = TimeSpan.FromSeconds(0.5f);
            public static readonly TimeSpan IntroFadeOutDuration = TimeSpan.FromSeconds(4);
            public static readonly TimeSpan IntroSongDurationOffset = -TimeSpan.FromSeconds(3);
            public const float IntroTextFadeSpeed = 2f;

            public static readonly TimeSpan QuickTransition = TimeSpan.FromSeconds(0.25f);

            public static readonly TimeSpan TransitionDuration = TimeSpan.FromSeconds(1f);

            public static readonly TimeSpan BattleEndDelay = TimeSpan.FromSeconds(2f);

            public static readonly TimeSpan CarouselRotationDurationSlow = TimeSpan.FromSeconds(0.75f);
        }

        public static class Songs {

        }
    }
}
