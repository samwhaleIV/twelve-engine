using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    public static class Constants {
        public const double MaxFrameDelta = 1000 / 15d;
        public const double SimFrameTime = 1000 / 60d;

        public const int EmptyTextureSize = 64;

        public const int ShiftFrameSkip = 30;
        public const int ScreenEdgePadding = 8;

        public const float DefaultLineSpacing = 1.25f;

        public const string ContentDirectory = "Content";

        public const string LogFile = "telog.txt";
        public const string SaveFile = "te.save";
        public const string ConfigFile = "te.config";
        public const string KeyBindsFile = "keybinds.config";

        public const string ConsoleWindowTitle = "Twelve Engine";

        public const string TimeSpanFormat = "{0:hh\\:mm\\:ss\\.f}";

        public const long LogResetLimit = (long)(0.25 * 1048576); // megabytes * bytesPerMegabyte

        public const string DebugFont = "Font/debug-font";

        public const string DefaultPlaybackFile = "default.teinp";
        public const string PlaybackFolder = "playback";

        public const string PlaybackFileExt = "teinp";

        public const string MusicVCAName = "MusicVCA";
        public const string SoundVCAName = "SoundVCA";

        public const string DefaultFMODGameBank = "Content/FMOD/Audio.bank";
        public const string DefaultFMODStringBank = "Content/FMOD/Master.string.bank";
        public const string DefaultFMODMasterBank = "Content/FMOD/Master.bank";

        public const char ConfigValueOperand = '=';
        public const char ConfigArrayDelimiter = ',';

        public const Keys CycleTimeDisplay = Keys.F2;

        public const Keys RecordingKey = Keys.F3;
        public const Keys PlaybackKey = Keys.F4;
        public const Keys PauseGameKey = Keys.F5;
        public const Keys AdvanceFrameKey = Keys.F6;
        public const Keys FullscreenKey = Keys.F11;

        /// <summary>
        /// Smoothing rate limit for the debug presentation of frame times and FPS.
        /// </summary>
        public static readonly TimeSpan FrameTimeFrequency = TimeSpan.FromMilliseconds(250);

        public static class Flags {
            public const string NoFailSafe = "nofailsafe";
            public const string HardwareFullscreen = "hwfullscreen";
            public const string NoVsync = "novsync";
            public const string Fullscreen = "fullscreen";
            public const string DrawDebug = "drawdebug";
            public const string Console = "console";
            public const string FMODProfile = "fmodprofile";
            public const string InteractionAgentDiagnostics = "intagentdiag";
        }

        public static class UI {
            public static readonly TimeSpan DefaultAnimationDuration = TimeSpan.FromMilliseconds(150);
            public static readonly TimeSpan DefaultTransitionDuration = TimeSpan.FromMilliseconds(300);
            public const float DefaultTransitionInputThreshold = 0.1f;
        }

    }
}
