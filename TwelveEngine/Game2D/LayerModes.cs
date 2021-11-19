namespace TwelveEngine.Game2D {
    public struct LayerMode {
        public bool Background;
        public int BackgroundStart;
        public int BackgroundLength;

        public bool Foreground;
        public int ForegroundStart;
        public int ForegroundLength;
    }
    public static class LayerModes {
        public static readonly LayerMode Default = SingleLayerBackground;
        public static readonly LayerMode SingleLayerBackground = new LayerMode() {
            BackgroundStart = 0,
            BackgroundLength = 1,
            Background = true,
            Foreground = false
        };
        public static readonly LayerMode BackgroundForegroundStandard = new LayerMode() {
            BackgroundStart = 0,
            BackgroundLength = 1,
            ForegroundStart = 1,
            ForegroundLength = 1,
            Background = true,
            Foreground = true
        };
        public static readonly LayerMode DoubleLayerBackground = new LayerMode() {
            BackgroundStart = 0,
            BackgroundLength = 2,
            Background = true,
            Foreground = false
        };
        public static readonly LayerMode BackgroundForegroundAlt = new LayerMode() {
            BackgroundStart = 0,
            BackgroundLength = 2,
            ForegroundStart = 2,
            ForegroundLength = 1,
            Background = true,
            Foreground = true
        };
        public static LayerMode GetAutomatic(Map map) {
            switch(map.Layers.Length) {
                default:
                case 1:
                    return SingleLayerBackground;
                case 2:
                    return DoubleLayerBackground;
                case 3:
                    return BackgroundForegroundAlt;
            }
        }
    }
}
