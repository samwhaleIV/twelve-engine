using TwelveEngine.Serial;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D.Camera {
    public abstract class Camera3D:ISerializable {

        public Vector3 Position { get; set; } = Vector3.Zero;

        public float FieldOfView { get; set; } = 75f;
        public float NearPlane { get; set; } = 1f;
        public float FarPlane { get; set; } = 1000f;

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        private float GetFieldOfView() => MathHelper.ToRadians(FieldOfView);

        private float lastAspectRatio, lastFieldOfView, lastNearPlane, lastFarPlane;
        private bool hasProjectionMatrix = false, hasViewMatrix = false;

        private Matrix GetProjectionMatrix(float aspectRatio) {
            return Matrix.CreatePerspectiveFieldOfView(GetFieldOfView(),aspectRatio,NearPlane,FarPlane);
        }

        protected abstract Matrix GetViewMatrix();
        protected abstract bool IsViewMatrixStale();

        private bool IsProjectionMatrixStale(float aspectRatio) {
            return !(aspectRatio == lastAspectRatio &&
                FieldOfView == lastFieldOfView &&
                NearPlane == lastNearPlane &&
                FarPlane == lastFarPlane);
        }

        private void UpdateProjectionData(float aspectRatio) {
            lastAspectRatio = aspectRatio;
            lastFieldOfView = FieldOfView;
            lastNearPlane = NearPlane;
            lastFarPlane = FarPlane;
        }

        /* Warning: The matrices are not updated automatically after Import(SerialFrame) */
        public void UpdateMatrices(float aspectRatio) {
            if(!hasProjectionMatrix || IsProjectionMatrixStale(aspectRatio)) {
                ProjectionMatrix = GetProjectionMatrix(aspectRatio);
                UpdateProjectionData(aspectRatio);
                hasProjectionMatrix = true;
            }

            if(!hasViewMatrix || IsViewMatrixStale()) {
                ViewMatrix = GetViewMatrix();
                hasViewMatrix = true;
            }
        }

        public virtual void Export(SerialFrame frame) => frame.Set(Position);

        public virtual void Import(SerialFrame frame) => Position = frame.GetVector3();
    }
}
