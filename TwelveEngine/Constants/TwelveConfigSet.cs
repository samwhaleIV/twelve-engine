namespace TwelveEngine.Config {
    public class TwelveConfigSet {

        public int RenderScale = 8;
        public int TileSize = 16;
        public int GamePadIndex = 0;

        public float PlayerSpeed = 2.5f;
        public float PlayerAccel = 200;
        public float PlayerDeaccel = 100;
        public float InteractSize = 2.5f;

        public string PlaybackFolder = "playback";
        public string DefaultPlaybackFile = "default" + '.' + Constants.PlaybackFileExt;
        public string ContentDirectory = "Content";
        public string PlayerImage = "player";
        public string Tileset = Constants.DefaultTileset;

        public string[] CPUTextures = new string[] { Constants.DefaultTileset, Constants.PatternsImage };

        public bool ShowCollision = false;
    }
}
