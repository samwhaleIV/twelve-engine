namespace TwelveEngine.Game2D {
    public class Camera:ISerializable {

        private float x = 0f;
        private float y = 0f;

        private float xOffset = 0f;
        private float yOffset = 0f;

        private float scale = 1f;

        private bool horizontalPadding = false;
        private bool verticalPadding = false;

        public float X {
            get => x;
            set => x = value;
        }
        public float Y {
            get => y;
            set => y = value;
        }

        public float XOffset {
            get => xOffset;
            set => xOffset = value;
        }

        public float YOffset {
            get => yOffset;
            set => yOffset = value;
        }

        public float AbsoluteX => x + xOffset;
        public float AbsoluteY => y + yOffset;

        public float Scale {
            get => scale;
            set => scale = value;
        }

        public bool HorizontalPadding {
            get => horizontalPadding;
            set => horizontalPadding = value;
        }
        public bool VerticalPadding {
            get => verticalPadding;
            set => verticalPadding = value;
        }

        public bool EdgePadding {
            set {
                horizontalPadding = value;
                verticalPadding = value;
            }
        }

        public void Export(SerialFrame frame) {
            frame.Set(X);
            frame.Set(Y);
            frame.Set(XOffset);
            frame.Set(YOffset);
            frame.Set(Scale);
            frame.Set(HorizontalPadding);
            frame.Set(VerticalPadding);
        }

        public void Import(SerialFrame frame) {
            X = frame.GetFloat();
            Y = frame.GetFloat();
            XOffset = frame.GetFloat();
            YOffset = frame.GetFloat();
            Scale = frame.GetFloat();
            HorizontalPadding = frame.GetBool();
            VerticalPadding = frame.GetBool();
        }
    }
}
