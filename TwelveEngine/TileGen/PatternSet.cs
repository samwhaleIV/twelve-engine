using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.TileGen {
    public sealed class PatternSet {

        internal PatternSet(Pattern[] patterns,int patternSize) {
            this.patterns = patterns;
            this.patternSize = patternSize;
            randomizer = new Randomizer(this);
        }

        private readonly Randomizer randomizer;

        public TileGenSettings Settings {
            get => randomizer.Settings;
            set => randomizer.Settings = value;
        }

        private readonly Pattern[] patterns;
        private readonly int patternSize;

        public int Count => patterns.Length;
        public int PatternSize => patternSize;

        public Pattern GetPattern(int index) => patterns[index];

        private void drawLayer(Color[] data,int dataWidth,Point target) {
            var pattern = randomizer.GetPattern();
            var drawSettings = randomizer.GetDrawSettings();
            pattern.Draw(data,dataWidth,target,drawSettings);
        }

        private void generateTile(Color[] data,int dataWidth,Point target) {
            var layerCount = randomizer.GetLayerCount();
            for(int i = 0;i<layerCount;i++) {
                drawLayer(data,dataWidth,target);
            }
        }

        public Texture2D GenerateTexture(GraphicsDevice graphicsDevice,int tileColumns) {
            var size = tileColumns * patternSize;

            var texture = new Texture2D(graphicsDevice,size,size);
            Color[] data = new Color[size * size];
            for(int i = 0;i<data.Length;i++) {
                data[i] = Settings.BackgroundColor;
            }

            randomizer.Seed(Settings.Seed);

            for(int x = 0;x<tileColumns;x++) {
                for(int y = 0;y<tileColumns;y++) {
                    var textureTarget = new Point(x*patternSize,y*patternSize);
                    generateTile(data,size,textureTarget);
                }
            }

            texture.SetData(data);
            return texture;
        }
    }
}
