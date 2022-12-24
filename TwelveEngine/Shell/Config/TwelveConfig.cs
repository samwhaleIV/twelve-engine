using System;
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
        public string KeyBindsFile { get; private set; }

        private PlayerIndex _gamePadIndex;
        public PlayerIndex GamePadIndex => _gamePadIndex;

        private void SetGamePadIndex(int value) {
            var minValue = (int)PlayerIndex.One;
            var maxValue = (int)PlayerIndex.Four;
            if(value < minValue) {
                value = minValue;
            } else if(value > maxValue) {
                value = maxValue;
            }
            _gamePadIndex = (PlayerIndex)value;
        }

        private static T[] GetArrayCopy<T>(T[] array) {
            var copy = new T[array.Length];
            array.CopyTo(copy,0);
            return copy;
        }

        private string[] _cpuTextures = Array.Empty<string>();
        public string[] CPUTextures => GetArrayCopy(_cpuTextures);

        private void SetCPUTextures(string[] textures) {
            if(textures != null) _cpuTextures = textures;
        }

        public static void Save(string path = null) => ConfigWriter.SaveEngineConfig(path);

        internal void Import(TwelveConfigSet set) {
            RenderScale = set.RenderScale;
            TileSize = set.TileSize;
            SetGamePadIndex(set.GamePadIndex);

            InteractSize = set.InteractSize;

            PlaybackFolder = set.PlaybackFolder;
            DefaultPlaybackFile = set.DefaultPlaybackFile;
            PlayerImage = set.PlayerImage;
            Tileset = set.Tileset;

            SetCPUTextures(set.CPUTextures);
            ShowCollision = set.ShowCollision;

            KeyBindsFile = set.KeyBindsFile;
        }

        internal TwelveConfigSet Export() => new() {
            RenderScale = RenderScale,
            TileSize = TileSize,
            GamePadIndex = (int)GamePadIndex,
            InteractSize = InteractSize,

            PlaybackFolder = PlaybackFolder,
            DefaultPlaybackFile = DefaultPlaybackFile,
            PlayerImage = PlayerImage,
            Tileset = Tileset,

            CPUTextures = CPUTextures,
            ShowCollision = ShowCollision,

            KeyBindsFile = KeyBindsFile
        };
    }
}
