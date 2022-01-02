namespace TwelveEngine.Config {
    public class TwelveConfigSet {

        public int RenderScale = 8;
        public int TileSize = 16;
        public int GamePadIndex = 0;

        public double MinLoadTime = 500;

        public float PlayerSpeed = 2.5f;
        public float PlayerAccel = 200;
        public float PlayerDeaccel = 100;
        public float InteractSize = 2.5f;

        public string PlaybackFolder = "playback";
        public string DefaultPlaybackFile = "default" + '.' + Constants.PlaybackFileExt;
        public string ContentDirectory = "Content";
        public string PlayerImage = "player";
        public string Tileset = "tileset";

        public string[] CPUTextures = new string[] { "tileset" };

        public bool ShowCollision = false;
    }
}
