using System;
using TwelveEngine.Serial;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public abstract class Camera3D:ISerializable {

        private Vector3 position = Vector3.Zero;
        private float fieldOfView = 75f, nearPlane = 1f, farPlane = 1000f;

        public Vector3 Position {
            get => position;
            set {
                position = value;
                InvalidateViewMatrix();
            }
        }

        public float FieldOfView {
            get => fieldOfView;
            set {
                fieldOfView = value;
                InvalidateProjectionMatrix();
            }
        }

        public float NearPlane {
            get => nearPlane;
            set {
                nearPlane = value;
                InvalidateProjectionMatrix();
            }
        }

        public float FarPlane {
            get => farPlane;
            set {
                farPlane = value;
                InvalidateProjectionMatrix();
            }
        }

        public bool IsProjectionMatrixValid { get; private set; } = false;
        public bool IsViewMatrixValid { get; private set; } = false;

        private void InvalidateProjectionMatrix() {
            IsProjectionMatrixValid = false;
        }
        protected void InvalidateViewMatrix() {
            IsViewMatrixValid = false;
        }
        private void ValidateProjectionMatrix() {
            IsProjectionMatrixValid = true;
        }
        private void ValidateViewMatrix() {
            IsViewMatrixValid = true;
        }

        protected abstract Matrix GetViewMatrix();

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

        public event Action<Matrix> OnViewMatrixChanged, OnProjectionMatrixChanged;

        private float GetFieldOfView() => MathHelper.ToRadians(FieldOfView);

        private Matrix GetProjectionMatrix(float aspectRatio) {
            return Matrix.CreatePerspectiveFieldOfView(GetFieldOfView(),aspectRatio,NearPlane,FarPlane);
        }

        private float lastAspectRatio = 0f;

        public void Update(float aspectRatio) {
            bool viewMatrixChanged = false, projectionMatrixChanged = false;
            if(aspectRatio != lastAspectRatio || !IsProjectionMatrixValid) {
                ProjectionMatrix = GetProjectionMatrix(aspectRatio);
                lastAspectRatio = aspectRatio;
                ValidateProjectionMatrix();
                projectionMatrixChanged = true;
            }
            if(!IsViewMatrixValid) {
                ViewMatrix = GetViewMatrix();
                ValidateViewMatrix();
                viewMatrixChanged = true;
            }
            if(projectionMatrixChanged) {
                OnProjectionMatrixChanged?.Invoke(ProjectionMatrix);
            }
            if(viewMatrixChanged) {
                OnViewMatrixChanged?.Invoke(ViewMatrix);
            }
        }

        public virtual void Export(SerialFrame frame) {
            frame.Set(Position);
            frame.Set(FieldOfView);
            frame.Set(NearPlane);
            frame.Set(FarPlane);
        }

        public virtual void Import(SerialFrame frame) {
            Position = frame.GetVector3();
            FieldOfView = frame.GetFloat();
            NearPlane = frame.GetFloat();
            FarPlane = frame.GetFloat();
        }
    }
}
