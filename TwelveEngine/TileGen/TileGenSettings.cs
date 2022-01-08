using Microsoft.Xna.Framework;

namespace TwelveEngine.TileGen {
    public sealed class TileGenSettings {
        public int MinLayers { get; set; }
        public int MaxLayers { get; set; }

        public bool DoRotate { get; set; }
        public bool DoMirror { get; set; }

        public Color BackgroundColor { get; set; }
        public Color[] Palette { get; set; }

        public int? OverridePattern { get; set; }
        public Color? OverrideColor { get; set; }
        public bool? OverrideStampPolarity { get; set; }

        public int? Seed { get; set; }

        public bool GeneratePalette { get; set; }
        public int AutoPaletteColorCount { get; set; }

        public static TileGenSettings Default => new TileGenSettings() {
            MinLayers = 1,
            MaxLayers = 4,
            DoMirror = true,
            DoRotate = true,
            BackgroundColor = Color.White,
            OverridePattern = null,
            OverrideColor = null,
            OverrideStampPolarity = null,
            Seed = null,
            AutoPaletteColorCount = 8,
            GeneratePalette = true,
            Palette = null
        };
    }
}
