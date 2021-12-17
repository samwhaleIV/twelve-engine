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
    }
}
