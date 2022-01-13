namespace TwelveEngine.Serial.Map {
    public readonly struct Map {

        public readonly int Width;
        public readonly int Height;

        public readonly int[][] Layers;

        public Map(int width,int height,int[][] layers) {
            Width = width;
            Height = height;
            Layers = layers;
        }

        public int[][,] Layers2D => getLayers2D();

        private int[,] getLayer2D(int index) {
            var layer2D = new int[Width,Height];
            var layer = Layers[index];
            for(var x = 0; x < Width; x++) {
                for(var y = 0; y < Height; y++) {
                    layer2D[x,y] = layer[x + y * Width];
                }
            }
            return layer2D;
        }

        private int[][,] getLayers2D() {
            var layers2D = new int[Layers.Length][,];

            for(var i = 0;i<layers2D.Length;i++) {
                layers2D[i] = getLayer2D(i);
            }

            return layers2D;
        }
    }
}
