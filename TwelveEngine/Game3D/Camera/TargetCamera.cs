using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public sealed class TargetCamera:Camera3D {

        public Vector3 Target { get; set; }

        protected override Matrix GetViewMatrix() {
            return Matrix.CreateLookAt(Position,Target,Vector3.Up);
        }
    }
}
