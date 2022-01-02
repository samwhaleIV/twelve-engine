namespace TwelveEngine.Game2D {
    public class Camera:ISerializable {

        private float x = 0f;
        private float y = 0f;

        private float xOffset = 0f;
        private float yOffset = 0f;

        private float scale = Constants.Config.RenderScale;

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

        public void SetPadding(bool horizontal,bool vertical) {
            horizontalPadding = horizontal;
            verticalPadding = vertical;
        }

        public void SetPadding(bool all) {
            horizontalPadding = all;
            verticalPadding = all;
        }

        public CameraPadding Padding {
            get {
                if(horizontalPadding && verticalPadding) {
                    return CameraPadding.All;
                } else if(horizontalPadding) {
                    return CameraPadding.Horizontal;
                } else if(verticalPadding) {
                    return CameraPadding.Vertical;
                } else {
                    return CameraPadding.None;
                }
            }
            set {
                switch(value) {
                    case CameraPadding.All:
                        horizontalPadding = true;
                        verticalPadding = true;
                        break;
                    case CameraPadding.Horizontal:
                        horizontalPadding = true;
                        verticalPadding = false;
                        break;
                    case CameraPadding.Vertical:
                        horizontalPadding = false;
                        verticalPadding = true;
                        break;
                    default:
                    case CameraPadding.None:
                        horizontalPadding = false;
                        verticalPadding = false;
                        break;
                }
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
