using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public abstract class Camera3D {

        private Vector3 position = Vector3.Zero;
        private float fieldOfView = 75f, nearPlane = 1f, farPlane = 100f;

        private bool orthographic = false;

        public Vector3 Position {
            get => position;
            set {
                if(position == value) {
                    return;
                }
                position = value;
                InvalidateViewMatrix();
            }
        }

        public float FieldOfView {
            get => fieldOfView;
            set {
                if(fieldOfView == value) {
                    return;
                }
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
                if(nearPlane == value) {
                    return;
                }
                nearPlane = value;
                InvalidateProjectionMatrix();
            }
        }

        public float FarPlane {
            get => farPlane;
            set {
                if(farPlane == value) {
                    return;
                }
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

        internal event Action OnViewMatrixChanged, OnProjectionMatrixChanged;

        private float GetFieldOfView() {
            return MathHelper.ToRadians(FieldOfView);
        }

        public FloatRectangle OrthographicArea { get; private set; } = FloatRectangle.Zero;

        private Matrix GetProjectionMatrix() {
            if(orthographic) {
                return Matrix.CreateOrthographic(OrthographicArea.Width,OrthographicArea.Height,nearPlane,farPlane);
            } else {
                return Matrix.CreatePerspectiveFieldOfView(GetFieldOfView(),aspectRatio,NearPlane,FarPlane);
            }
        }

        private float aspectRatio = 0f;

        public void UpdateScreenSize(float aspectRatio) {
            if(aspectRatio == this.aspectRatio) {
                return;
            }
            this.aspectRatio = aspectRatio;
            IsProjectionMatrixValid = false;
            float width, height;
            if(aspectRatio > 1) {
                width = 1f;
                height = 1f / aspectRatio;
            } else {
                width = aspectRatio / 1f;
                height = 1f;
            }
            float x = width * -0.5f, y = height * -0.5f;
            OrthographicArea = new FloatRectangle(x,y,width,height);
        }

        public void Update() {
            bool viewMatrixChanged = false, projectionMatrixChanged = false;
            if(!IsProjectionMatrixValid) {
                ProjectionMatrix = GetProjectionMatrix();
                ValidateProjectionMatrix();
                projectionMatrixChanged = true;
            }
            if(!IsViewMatrixValid) {
                ViewMatrix = GetViewMatrix();
                ValidateViewMatrix();
                viewMatrixChanged = true;
            }
            if(projectionMatrixChanged) {
                OnProjectionMatrixChanged?.Invoke();
            }
            if(viewMatrixChanged) {
                OnViewMatrixChanged?.Invoke();
            }
        }
    }
}
