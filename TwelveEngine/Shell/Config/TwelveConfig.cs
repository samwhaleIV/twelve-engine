using Microsoft.Xna.Framework;

namespace TwelveEngine.Shell.Config {
    public sealed class TwelveConfig {

        /* This exists to serve as a readonly interface for Constants.Config.
         * In addition, it allows conversions from binary types to engine types */

        public int RenderScale { get; private set; }
        public float InteractSize { get; private set; }
        public int TileSize { get; private set; }
        public bool ShowCollision { get; private set; }
        public string Tileset { get; private set; }
        public string PlayerImage { get; private set; }
        public string PlaybackFolder { get; private set; }
        public string DefaultPlaybackFile { get; private set; }
        public float PlayerAccel { get; private set; }
        public float PlayerDeaccel { get; private set; }
        public float PlayerSpeed { get; private set; }
        public string KeyBindsFile { get; private set; }

        private static PlayerIndex _gamePadIndex;
        public PlayerIndex GamePadIndex => _gamePadIndex;

        private void setGamePadIndex(int value) {
            var minValue = (int)PlayerIndex.One;
            var maxValue = (int)PlayerIndex.Four;
            if(value < minValue) {
                value = minValue;
            } else if(value > maxValue) {
                value = maxValue;
            }
            _gamePadIndex = (PlayerIndex)value;
        }

        private static T[] getArrayCopy<T>(T[] array) {
            var copy = new T[array.Length];
            array.CopyTo(copy,0);
            return copy;
        }

        private string[] _cpuTextures = new string[0];
        public string[] CPUTextures => getArrayCopy(_cpuTextures);

        private void setCPUTextures(string[] textures) {
            if(textures != null) _cpuTextures = textures;
        }

        public void Save(string path = null) => ConfigWriter.SaveEngineConfig(path);

        internal void Import(TwelveConfigSet set) {
            RenderScale = set.RenderScale;
            TileSize = set.TileSize;
            setGamePadIndex(set.GamePadIndex);

            PlayerSpeed = set.PlayerSpeed;
            PlayerAccel = set.PlayerAccel;
            PlayerDeaccel = set.PlayerDeaccel;
            InteractSize = set.InteractSize;

            PlaybackFolder = set.PlaybackFolder;
            DefaultPlaybackFile = set.DefaultPlaybackFile;
            PlayerImage = set.PlayerImage;
            Tileset = set.Tileset;

            setCPUTextures(set.CPUTextures);
            ShowCollision = set.ShowCollision;

            KeyBindsFile = set.KeyBindsFile;
        }

        internal TwelveConfigSet Export() {
            var set = new TwelveConfigSet();

            set.RenderScale = RenderScale;
            set.TileSize = TileSize;
            set.GamePadIndex = (int)GamePadIndex;

            set.PlayerSpeed = PlayerSpeed;
            set.PlayerAccel = PlayerAccel;
            set.PlayerDeaccel = PlayerDeaccel;
            set.InteractSize = InteractSize;

            set.PlaybackFolder = PlaybackFolder;
            set.DefaultPlaybackFile = DefaultPlaybackFile;
            set.PlayerImage = PlayerImage;
            set.Tileset = Tileset;

            set.CPUTextures = CPUTextures;
            set.ShowCollision = ShowCollision;

            set.KeyBindsFile = KeyBindsFile;

            return set;
        }
    }
}
