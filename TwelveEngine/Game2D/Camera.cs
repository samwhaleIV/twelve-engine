namespace TwelveEngine.Game2D {
    public class Camera:ISerializable {
        public float X { get; set; } = 0;
        public float Y { get; set; } = 0;

        public float XOffset { get; set; } = 0.5f;
        public float YOffset { get; set; } = 0.5f;

        public float Scale { get; set; } = 1;

        public bool HorizontalEdgePadding { get; set; } = false;
        public bool VerticalEdgePadding { get; set; } = false;

        public bool EdgePadding {
            get {
                return HorizontalEdgePadding || VerticalEdgePadding;
            }
            set {
                HorizontalEdgePadding = value;
                VerticalEdgePadding = value;
            }
        }

        public void Export(SerialFrame frame) {
            frame.Set(X);
            frame.Set(Y);
            frame.Set(XOffset);
            frame.Set(YOffset);
            frame.Set(Scale);
            frame.Set(HorizontalEdgePadding);
            frame.Set(VerticalEdgePadding);
        }

        public void Import(SerialFrame frame) {
            X = frame.GetFloat();
            Y = frame.GetFloat();
            XOffset = frame.GetFloat();
            YOffset = frame.GetFloat();
            Scale = frame.GetFloat();
            HorizontalEdgePadding = frame.GetBool();
            VerticalEdgePadding = frame.GetBool();
        }
    }
}
