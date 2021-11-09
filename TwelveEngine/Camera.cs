using Microsoft.Xna.Framework;
using Newtonsoft.Json;

namespace TwelveEngine {
    public class Camera:ISerializable {
        public Vector2 position;
        public float X {
            get {
                return position.X;
            }
            set {
                position.X = value;
            }
        }
        public float Y {
            get {
                return position.Y;
            }
            set {
                position.Y = value;
            }
        }
        public float Scale { get; set; } = 1;

        private bool horizontalEdgePadding = true;
        private bool verticalEdgePadding = true;
        public bool HorizontalEdgePadding {
            get {
                return horizontalEdgePadding;
            }
            set {
                horizontalEdgePadding = value;
            }
        }
        public bool VerticalEdgePadding {
            get {
                return verticalEdgePadding;
            }
            set {
                horizontalEdgePadding = value;
            }
        }
        public bool EdgePadding {
            get {
                return horizontalEdgePadding || verticalEdgePadding;
            }
            set {
                horizontalEdgePadding = value;
                verticalEdgePadding = value;
            }
        }

        public void Export(SerialFrame frame) {
            frame.Set("X",X);
            frame.Set("Y",Y);
            frame.Set("Scale",Scale);
            frame.Set("horizontalEdgePadding",horizontalEdgePadding);
            frame.Set("verticalEdgePadding",verticalEdgePadding);
        }

        public void Import(SerialFrame frame) {
            X = frame.GetFloat("X");
            Y = frame.GetFloat("Y");
            Scale = frame.GetFloat("Scale");
            horizontalEdgePadding = frame.GetBool("horizontalEdgePadding");
            verticalEdgePadding = frame.GetBool("verticalEdgePadding");
        }
    }
}
