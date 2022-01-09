using TwelveEngine.Serial;

namespace TwelveEngine.Game2D {
    public struct LayerMode:ISerializable {
        public bool Background;
        public int BackgroundStart;
        public int BackgroundLength;

        public bool Foreground;
        public int ForegroundStart;
        public int ForegroundLength;

        public void Export(SerialFrame frame) {
            frame.Set(Background);
            frame.Set(BackgroundStart);
            frame.Set(BackgroundLength);
            frame.Set(Foreground);
            frame.Set(ForegroundStart);
            frame.Set(ForegroundLength);
        }

        public void Import(SerialFrame frame) {
            Background = frame.GetBool();
            BackgroundStart = frame.GetInt();
            BackgroundLength = frame.GetInt();
            Foreground = frame.GetBool();
            ForegroundStart = frame.GetInt();
            ForegroundLength = frame.GetInt();
        }
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
            ForegroundStart = 3,
            ForegroundLength = 1,
            Background = true,
            Foreground = true
        };
        public static LayerMode GetAutomatic(int layerCount) {
            switch(layerCount) {
                default:
                case 2:
                    return SingleLayerBackground;
                case 3:
                    return DoubleLayerBackground;
                case 4:
                    return BackgroundForegroundAlt;
            }
        }
    }
}
