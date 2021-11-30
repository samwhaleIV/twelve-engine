using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    public static class Constants {
        public const int MinimumLoadTime = 500;
        public const double MaximumFrameDelta = 1000 / 15d;
        public const double SimFrameTime = 1000 / 60d;
        public const int RenderScale = 8;

        public const string ContentRootDirectory = "Content";
        public const int DefaultTileSize = 16;

        public const bool ShowCollisionLayer = false;

        public const string MapDatabase = ContentRootDirectory + "/" + "map-database.json";
        public const string Tileset = "Tileset";
        public const string PlayerImage = "Player";

        public const int ObjectLayerIndex = 1;
        public const int CollisionLayerIndex = 2;

        public const Keys RecordingKey = Keys.F3;
        public const Keys PlaybackKey = Keys.F4;

        public const Keys PauseGame = Keys.F5;
        public const Keys AdvanceFrame = Keys.F6;

        public const Keys SaveState = Keys.F1;
        public const Keys LoadState = Keys.F2;

        public const string PlaybackFileExtension = "input";
        public const string PlaybackFolder = "playback";

        public const string DefaultPlaybackFile = "default." + PlaybackFileExtension;

        public const string PreferredPlaybackFile = null;

        public const int ShiftFastForwardFrames = 30;

        public const int PlayerAccel = 200;
        public const int PlayerDeaccel = 100;
        public const float DefaultPlayerSpeed = 2.5f;
    }
}
