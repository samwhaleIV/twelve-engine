using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.TileGen {
    internal sealed class Randomizer {

        private static readonly int MIRROR_TYPE_COUNT = Enum.GetValues(typeof(Mirroring)).Length;
        private static readonly int ROTATION_TYPE_COUNT = Enum.GetValues(typeof(Rotation)).Length;

        private readonly PatternSet patternSet;
        public TileGenSettings Settings { get; set; } = TileGenSettings.Default;

        public Randomizer(PatternSet set) => patternSet = set;

        private Random random;

        private Color[] palette;

        private Color[] generateAutoPalette() {
            var palette = new Color[Settings.AutoPaletteColorCount];
            for(int i = 0;i< palette.Length;i++) palette[i] = getRandomColor();
            return palette;
        }

        private void seedPalette() {
            if(Settings.Palette == null && Settings.GeneratePalette) {
                palette = generateAutoPalette();
            } else {
                palette = Settings.Palette;
            }
        }

        public void Seed(int? seed) {
            random = seed.HasValue ? new Random(seed.Value) : random == null ? new Random() : random;
            seedPalette();
        }

        private Color getRandomColor() {
            var bytes = new byte[3]; random.NextBytes(bytes);
            return new Color(bytes[0],bytes[1],bytes[2]);
        }

        public Pattern GetPattern() {
            int index;
            if(Settings.OverridePattern.HasValue) {
                index = Settings.OverridePattern.Value;
            } else {
                index = random.Next(0,patternSet.Count);
            }
            return patternSet.GetPattern(index);
        }

        public int GetLayerCount() {
            return random.Next(Settings.MinLayers,Settings.MaxLayers+1);
        }

        public Color GetColor() {
            if(Settings.OverrideColor.HasValue) {
                return Settings.OverrideColor.Value;
            } else if(palette == null) {
                return getRandomColor();
            }
            return palette[random.Next(0,palette.Length)];
        }

        public Painter GetPainter() {
            return new Painter(GetColor());
        }
        public Mirroring GetMirroring() {
            if(!Settings.DoMirror) {
                return Mirroring.None;
            }
            return (Mirroring)random.Next(0,MIRROR_TYPE_COUNT);
        }
        public Rotation GetRotation() {
            if(!Settings.DoRotate) {
                return Rotation.Zero;
            }
            return (Rotation)random.Next(0,ROTATION_TYPE_COUNT);
        }

        public bool GetStampPolarity() {
            if(Settings.OverrideStampPolarity.HasValue) {
                return Settings.OverrideStampPolarity.Value;
            }
            return random.Next(0,2) == 0;
        }

        public PatternDrawSettings GetDrawSettings() {

            var painter = GetPainter();

            var settings = new PatternDrawSettings() {
                Mirroring = GetMirroring(),
                Rotation = GetRotation(),
                Polarity = GetStampPolarity(),
                Paint = painter.Paint
            };

            return settings;
        }
    }
}
