namespace TwelveEngine.Game3D {
    public sealed class TargetCamera:Camera3D {

        private Vector3 target = Vector3.Zero;

        public Vector3 Target {
            get => target;
            set {
                if(target == value) {
                    return;
                }
                target = value;
                InvalidateViewMatrix();
            }
        }

        protected override Matrix GetViewMatrix() {
            return Matrix.CreateLookAt(Position,Target,Vector3.Up);
        }
    }
}
