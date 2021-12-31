using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    public static partial class Constants {
        public const int SerialSubframeSize = 1024;

        public const double MaximumFrameDelta = 1000 / 15d;
        public const double SimFrameTime = 1000 / 60d;
        public const int RenderScale = 8;

        public const float InteractionBoxSize = 0.25f;

        public const bool DoFakeLoadingTime = true;
        public static readonly TimeSpan FakeLoadingTime = TimeSpan.FromMilliseconds(500);

        public const string ContentRootDirectory = "Content";
        public const int DefaultTileSize = 16;

        public const bool ShowCollisionLayer = false;

        public const PlayerIndex GamePadIndex = PlayerIndex.One;

        public const string Tileset = "Tileset";
        public const string PlayerImage = "Player";

        public const int ObjectLayerIndex = 1;
        public const int CollisionLayerIndex = 2;

        public const string PlaybackFileExtension = "teinp"; /* Twelve Engine (Playback) Input (File) */
        public const string PlaybackFolder = "playback";
        public const string DefaultPlaybackFile = "default." + PlaybackFileExtension;

        public const string PreferredPlaybackFile = null;

        public const int ShiftFastForwardFrames = 30;

        public const int PlayerAccel = 200;
        public const int PlayerDeaccel = 100;
        public const float DefaultPlayerSpeed = 2.5f;

        public const bool DoLoadCPUTextures = true;
        public static readonly string[] CPUTextures = new string[] {Tileset};
    }
}
