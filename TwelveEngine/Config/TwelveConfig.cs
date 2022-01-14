using Microsoft.Xna.Framework;

namespace TwelveEngine.Config {
    public sealed class TwelveConfig {

        /* This exists to serve as a readonly interface for Constants.Config */

        private static PlayerIndex gamePadIndex;
        public PlayerIndex GamePadIndex => gamePadIndex;
        private void setGamePadIndex(int value) {
            var minValue = (int)PlayerIndex.One;
            var maxValue = (int)PlayerIndex.Four;
            if(value < minValue) {
                value = minValue;
            } else if(value > maxValue) {
                value = maxValue;
            }
            gamePadIndex = (PlayerIndex)value;
        }

        private int renderScale;
        public int RenderScale => renderScale;

        private float interactSize;
        public float InteractSize => interactSize;

        private string contentDirectory;
        public string ContentDirectory => contentDirectory;

        private int tileSize;
        public int TileSize => tileSize;

        private bool showCollision;
        public bool ShowCollision => showCollision;

        private string tileset;
        public string Tileset => tileset;

        private string playerImage;
        public string PlayerImage => playerImage;

        private string playbackFolder;
        public string PlaybackFolder => playbackFolder;

        private string defaultPlaybackFile;
        public string DefaultPlaybackFile => defaultPlaybackFile;

        private float playerAccel;
        public float PlayerAccel => playerAccel;

        private float playerDeaccel;
        public float PlayerDeaccel => playerDeaccel;

        private float playerSpeed;
        public float PlayerSpeed => playerSpeed;

        private string keyBindsFile;
        public string KeyBindsFile => keyBindsFile;

        private string[] cpuTextures;
        public string[] CPUTextures {
            get {
                var copy = new string[cpuTextures.Length];
                cpuTextures.CopyTo(copy,0);
                return copy;
            }
        }

        public void Save(string path = null) => ConfigWriter.SaveEngineConfig(path);

        internal void Import(TwelveConfigSet set) {
            renderScale = set.RenderScale;
            tileSize = set.TileSize;
            setGamePadIndex(set.GamePadIndex);

            playerSpeed = set.PlayerSpeed;
            playerAccel = set.PlayerAccel;
            playerDeaccel = set.PlayerDeaccel;
            interactSize = set.InteractSize;

            playbackFolder = set.PlaybackFolder;
            defaultPlaybackFile = set.DefaultPlaybackFile;
            contentDirectory = set.ContentDirectory;
            playerImage = set.PlayerImage;
            tileset = set.Tileset;

            cpuTextures = set.CPUTextures != null ? set.CPUTextures : new string[0];
            showCollision = set.ShowCollision;

            keyBindsFile = set.KeyBindsFile;
        }

        internal TwelveConfigSet Export() {
            var set = new TwelveConfigSet();

            set.RenderScale = renderScale;
            set.TileSize = tileSize;
            set.GamePadIndex = (int)gamePadIndex;

            set.PlayerSpeed = playerSpeed;
            set.PlayerAccel = playerAccel;
            set.PlayerDeaccel = playerDeaccel;
            set.InteractSize = interactSize;

            set.PlaybackFolder = playbackFolder;
            set.DefaultPlaybackFile = defaultPlaybackFile;
            set.ContentDirectory = contentDirectory;
            set.PlayerImage = playerImage;
            set.Tileset = tileset;

            set.CPUTextures = cpuTextures;
            set.ShowCollision = showCollision;

            set.KeyBindsFile = keyBindsFile;

            return set;
        }
    }
}
