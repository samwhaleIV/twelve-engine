using TwelveEngine.Serial;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D.Camera {
    public class AngleCamera:Camera3D {

        public Vector3 Angle { get; set; } = Vector3.Zero;

        private Vector3 lastPosition, lastAngle;

        protected override bool IsViewMatrixStale() {
            return lastPosition != Position || lastAngle != Angle;
        }

        protected override Matrix GetViewMatrix() {
            var angle = Angle;

            var positionMatrix = Matrix.CreateLookAt(Position,Position,Vector3.Up);
            var viewMatrix = Matrix.CreateFromYawPitchRoll(angle.X,angle.Y,angle.Z) * positionMatrix;

            lastPosition = Position;
            lastAngle = angle;

            return viewMatrix;
        }

        public override void Export(SerialFrame frame) {
            base.Export(frame);
            frame.Set(Angle);
        }

        public override void Import(SerialFrame frame) {
            base.Import(frame);
            Angle = frame.GetVector3();
        }
    }
}
