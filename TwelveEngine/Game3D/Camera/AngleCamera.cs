using TwelveEngine.Serial;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public class AngleCamera:Camera3D {

        public AngleCamera() => SetRotation(0,0);

        public Vector3 Up { get; private set; }
        public Vector3 Forward { get; private set; }

        public float Yaw { get; private set; }
        public float Pitch { get; private set; }

        public Vector2 Angle {
            get => new Vector2(Yaw,Pitch);
            set => SetRotation(value.X,value.Y);
        }

        private void SetYaw(float yaw,float pitch) {
            Vector3 forward = Vector3.Transform(Orientation.CameraForward,Matrix.CreateFromAxisAngle(Orientation.CameraUp,MathHelper.ToRadians(yaw)));
            forward.Normalize();

            Forward = forward;
            Yaw = yaw;

            SetPitch(pitch);
        }

        private void SetPitch(float value) {
            Vector3 cross = Vector3.Cross(Orientation.CameraUp,Forward);
            cross.Normalize();

            Forward = Vector3.Transform(Forward,Matrix.CreateFromAxisAngle(cross,MathHelper.ToRadians(value)));
            Up = Vector3.Transform(Orientation.CameraUp,Matrix.CreateFromAxisAngle(cross,MathHelper.ToRadians(value)));

            Pitch = value;
        }

        public void UpdateFreeCam(Vector3 delta,float velocity) {
            if(delta.X != 0) {
                var cross = Vector3.Cross(Up,Forward);
                cross.Normalize();
                Position -= cross * velocity * delta.X;
            }
            if(delta.Y != 0) {
                var forward = Forward;
                forward.Normalize();
                Position -= forward * velocity * delta.Y;
            }
            if(delta.Z != 0) {
                var position = Position;
                position.Z -= velocity * delta.Z;
                Position = position;
            }
        }

        public void AddRotation(float yaw,float pitch) => SetYaw(Yaw+yaw,Pitch+pitch);
        public void SetRotation(float yaw,float pitch) => SetYaw(yaw,pitch);

        private Vector3 lastPosition, lastForward, lastUp;

        protected override bool IsViewMatrixStale() {
            return lastPosition != Position || lastForward != Forward || lastUp != Up;
        }

        protected override Matrix GetViewMatrix() {
            var viewMatrix = Matrix.CreateLookAt(Position,Forward+Position,Up);

            lastPosition = Position;
            lastForward = Forward;
            lastUp = Up;

            return viewMatrix;
        }

        public override void Export(SerialFrame frame) {
            base.Export(frame);
            frame.Set(Yaw);
            frame.Set(Pitch);
        }

        public override void Import(SerialFrame frame) {
            base.Import(frame);
            Yaw = frame.GetFloat();
            Pitch = frame.GetFloat();
        }
    }
}
