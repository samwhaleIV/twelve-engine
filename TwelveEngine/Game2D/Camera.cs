using Microsoft.Xna.Framework;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D {
    public class Camera:ISerializable {

        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; }

        public float Scale { get; set; } = Constants.Config.RenderScale;

        public bool HorizontalPadding { get; set; } = false;
        public bool VerticalPadding { get; set; } = false;

        public void SetPadding(bool horizontal,bool vertical) {
            HorizontalPadding = horizontal;
            VerticalPadding = vertical;
        }

        public void SetPadding(bool all) {
            HorizontalPadding = all;
            VerticalPadding = all;
        }

        public CameraPadding Padding {
            get {
                if(HorizontalPadding && VerticalPadding) {
                    return CameraPadding.All;
                } else if(HorizontalPadding) {
                    return CameraPadding.Horizontal;
                } else if(VerticalPadding) {
                    return CameraPadding.Vertical;
                } else {
                    return CameraPadding.None;
                }
            }
            set {
                switch(value) {
                    case CameraPadding.All:
                        HorizontalPadding = true;
                        VerticalPadding = true;
                        break;
                    case CameraPadding.Horizontal:
                        HorizontalPadding = true;
                        VerticalPadding = false;
                        break;
                    case CameraPadding.Vertical:
                        HorizontalPadding = false;
                        VerticalPadding = true;
                        break;
                    default:
                    case CameraPadding.None:
                        HorizontalPadding = false;
                        VerticalPadding = false;
                        break;
                }
            }
        }

        public void Export(SerialFrame frame) {
            frame.Set(Position);
            frame.Set(Offset);
            frame.Set(Scale);
            frame.Set(HorizontalPadding);
            frame.Set(VerticalPadding);
        }

        public void Import(SerialFrame frame) {
            Position = frame.GetVector2();
            Offset = frame.GetVector2();
            HorizontalPadding = frame.GetBool();
            VerticalPadding = frame.GetBool();
        }
    }
}
