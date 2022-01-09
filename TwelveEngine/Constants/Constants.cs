﻿using TwelveEngine.Config;

namespace TwelveEngine {
    public static partial class Constants {
        public const int SerialSubframeSize = 1024;

        public const double MaxFrameDelta = 1000 / 15d;
        public const double SimFrameTime = 1000 / 60d;

        public const int ObjectLayerIndex = 1;
        public const int CollisionLayerIndex = 2;
        public const int ShiftFrameSkip = 30;
        public const int ScreenEdgePadding = 8;

        public const string PlaybackFileExt = "teinp"; /* Twelve Engine (Playback) Input (File) */
        public const string EngineConfigFile = "twelve.config";
        public const string DefaultFont = "default-font";
        public const string PatternsImage = "patterns";
        public const string DefaultTileset = "tileset";
        public const string DefaultKeyBindsFile = "keybinds.config";

        public const char ConfigValueOperand = '=';
        public const char ConfigArrayDelimiter = ',';

        public static TwelveConfig Config { get; internal set; }
    }
}
