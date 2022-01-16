using Microsoft.Xna.Framework;
using TwelveEngine.Serial;

namespace TwelveEngine.Game3D.Camera {
    public class TargetCamera:Camera3D {

        public Vector3 Target { get; set; } = Vector3.Zero;

        private Vector3 lastPosition, lastTarget;

        protected override bool IsViewMatrixStale() {
            return lastPosition != Position || lastTarget != Target;
        }

        protected override Matrix GetViewMatrix() {
            var viewMatrix = Matrix.CreateLookAt(Position,Target,Vector3.Up);

            lastPosition = Position;
            lastTarget = Target;

            return viewMatrix;
        }

        public override void Export(SerialFrame frame) {
            base.Export(frame);
            frame.Set(Target);
        }

        public override void Import(SerialFrame frame) {
            base.Import(frame);
            Target = frame.GetVector3();
        }
    }
}
