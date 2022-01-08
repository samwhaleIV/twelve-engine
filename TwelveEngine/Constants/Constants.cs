using System;
using TwelveEngine.Config;

namespace TwelveEngine {
    public static partial class Constants {
        public const int SerialSubframeSize = 1024;

        public const double MaxFrameDelta = 1000 / 15d;
        public const double SimFrameTime = 1000 / 60d;

        public const int ObjectLayerIndex = 1;
        public const int CollisionLayerIndex = 2;

        public const int ShiftFrameSkip = 30;

        public const string PlaybackFileExt = "teinp"; /* Twelve Engine (Playback) Input (File) */

        public const string EngineConfigFile = "twelve.config";

        public const string DefaultFont = "default-font";

        public const string PatternsImage = "patterns";

        public const string DefaultTileset = "tileset";

        private static bool loadedConfigFile = false;

        private static TwelveConfig config = null;
        public static TwelveConfig Config {
            get => config;
            internal set {
                if(loadedConfigFile) {
                    throw new Exception("A config file intended for constant use has already been loaded!");
                }
                loadedConfigFile = true;
                config = value;
            }
        }
    }
}
