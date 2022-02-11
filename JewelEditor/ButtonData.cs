using Microsoft.Xna.Framework;

namespace JewelEditor {
    internal readonly struct ButtonData {

        public ButtonData(Rectangle textureSource,InputMode inputMode,TileType? tileType = null) {
            TextureSource = textureSource;
            InputMode = inputMode;
            TileType = tileType;
        }

        public readonly Rectangle TextureSource;
        public readonly InputMode InputMode;
        public readonly TileType? TileType;

        internal void Deconstruct(out Rectangle TextureSource,out InputMode InputMode,out TileType? TileType) {
            TextureSource = this.TextureSource;
            InputMode = this.InputMode;
            TileType = this.TileType;
        }
    }
}
