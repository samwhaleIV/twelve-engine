using System;
using TwelveEngine.Serial;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public abstract class Camera3D {

        private Vector3 position = Vector3.Zero;
        private float fieldOfView = 75f, nearPlane = 1f, farPlane = 1000f;

        private bool orthographic = false;

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
                if(Orthographic) {
                    return;
                }
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

        public bool Orthographic {
            get => orthographic;
            set {
                if(value == orthographic) {
                    return;
                }
                orthographic = value;
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

        internal event Action<Matrix> OnViewMatrixChanged, OnProjectionMatrixChanged;

        private float GetFieldOfView() => MathHelper.ToRadians(FieldOfView);

        public Vector2 OrthographicCenter { get; private set; } = Vector2.Zero;
        public Vector2 OrthographicSize { get; private set; } = Vector2.Zero;

        private Matrix GetProjectionMatrix(float aspectRatio) {
            if(orthographic) {
                float width, height;
                if(aspectRatio > 1) {
                    width = 1f;
                    height = 1f / aspectRatio;
                } else {
                    width = aspectRatio / 1f;
                    height = 1f;
                }
                OrthographicCenter = new Vector2(width * -0.5f,height* -0.5f);
                OrthographicSize = new Vector2(width,height);
                return Matrix.CreateOrthographic(width,height,nearPlane,farPlane);
            } else {
                return Matrix.CreatePerspectiveFieldOfView(GetFieldOfView(),aspectRatio,NearPlane,FarPlane);
            }
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
    }
}
