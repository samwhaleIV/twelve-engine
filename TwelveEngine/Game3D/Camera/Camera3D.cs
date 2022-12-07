﻿using System;
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

        internal event Action<Matrix> OnViewMatrixChanged, OnProjectionMatrixChanged;

        private float GetFieldOfView() => MathHelper.ToRadians(FieldOfView);

        public VectorRectangle OrthographicArea { get; private set; } = VectorRectangle.Zero;

        private Matrix GetProjectionMatrix(float aspectRatio) {
            float width, height;
            if(aspectRatio > 1) {
                width = 1f;
                height = 1f / aspectRatio;
            } else {
                width = aspectRatio / 1f;
                height = 1f;
            }
            float x = width * -0.5f, y = height * -0.5f;
            OrthographicArea = new VectorRectangle(x,y,width,height);
            if(orthographic) {
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
