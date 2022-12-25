using System;

namespace TwelveEngine.Shell.Config {
    public class TwelveConfigSet {

        public int RenderScale = 8;
        public int TileSize = 16;
        public int GamePadIndex = 0;
        public float InteractSize = 0.25f;

        public int? HWFullScreenWidth = null;
        public int? HWFullScreenHeight = null;

        public string PlaybackFolder = "playback";
        public string DefaultPlaybackFile = "default" + '.' + Constants.PlaybackFileExt;
        public string PlayerImage = "player";
        public string Tileset = Constants.DefaultTileset;

        public string KeyBindsFile = Constants.DefaultKeyBindsFile;

        public string[] CPUTextures = Array.Empty<string>();

        public bool ShowCollision = false;
    }
}
