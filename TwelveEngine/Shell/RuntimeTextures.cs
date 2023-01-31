namespace TwelveEngine.Shell {
    public static class RuntimeTextures {

        private static void LoadEmpty(GraphicsDevice graphicsDevice) {
            int size = Constants.EmptyTextureSize;
            int pixelCount = size * size;
            Texture2D emptyTexture = new(graphicsDevice,size,size);
            Color[] pixels = new Color[pixelCount];
            for(int i = 0;i<pixelCount;i++) {
                pixels[i] = Color.White;
            }
            emptyTexture.SetData(pixels);
            Empty = emptyTexture;
        }

        internal static void Load(GraphicsDevice graphicsDevice) {
            LoadEmpty(graphicsDevice);
        }

        internal static void Unload() {
            Empty?.Dispose();
            Empty = null;
        }

        public static Texture2D Empty { get; private set; }
    }
}
