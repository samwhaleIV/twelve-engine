namespace TwelveEngine {
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
            frame.Set("X",X);
            frame.Set("Y",Y);
            frame.Set("XOffset",XOffset);
            frame.Set("YOffset",YOffset);
            frame.Set("Scale",Scale);
            frame.Set("HorizontalEdgePadding",HorizontalEdgePadding);
            frame.Set("VerticalEdgePadding",VerticalEdgePadding);
        }

        public void Import(SerialFrame frame) {
            X = frame.GetFloat("X");
            Y = frame.GetFloat("Y");
            XOffset = frame.GetFloat("XOffset");
            YOffset = frame.GetFloat("YOffset");
            Scale = frame.GetFloat("Scale");
            HorizontalEdgePadding = frame.GetBool("HorizontalEdgePadding");
            VerticalEdgePadding = frame.GetBool("VerticalEdgePadding");
        }
    }
}
