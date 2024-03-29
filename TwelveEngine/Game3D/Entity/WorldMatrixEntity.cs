﻿namespace TwelveEngine.Game3D.Entity {
    public abstract class WorldMatrixEntity:Entity3D {

        public WorldMatrixEntity() {
            OnPreRender += PreRender;
        }

        private void FirstPreRender() {
            UpdateWorldMatrix();
            ApplyViewMatrix(ref Owner.ViewMatrix);
            ApplyProjectionMatrix(ref Owner.ProjectionMatrix);
        }

        private bool _firstPreRender = true;

        private void PreRender() {
            if(_firstPreRender) {
                FirstPreRender();
                _firstPreRender = false;
                return;
            }
            UpdateMatrices();
        }

        private void UpdateMatrices() {
            UpdateWorldMatrix();
            if(Owner.ViewMatrixUpdated) {
                ApplyViewMatrix(ref Owner.ViewMatrix);
            }
            if(Owner.ProjectionMatrixUpdated) {
                ApplyProjectionMatrix(ref Owner.ProjectionMatrix);
            }
        }

        protected abstract void ApplyViewMatrix(ref Matrix viewMatrix);
        protected abstract void ApplyProjectionMatrix(ref Matrix projectionMatrix);
        protected abstract void ApplyWorldMatrix(ref Matrix matrix);

        private Vector3? lastCameraPosition = null;

        private Matrix originMatrix, rotationMatrix, scaleMatrix;

        private void ValidateBillboard() {
            var camera = Owner.Camera;
            if(camera == null) {
                return;
            }
            var cameraPosition = camera.Position;
            if(!(Billboard && cameraPosition != lastCameraPosition)) {
                return;
            }
            PositionValid = false;
            WorldMatrixValid = false;
            lastCameraPosition = cameraPosition;
        }

        private void UpdateWorldMatrix() {
            ValidateBillboard();

            if(WorldMatrixValid) {
                return;
            }

            UpdateVectorMatrices();

            Matrix worldMatrix = GetWorldMatrix();
            WorldMatrixValid = true;

            ApplyWorldMatrix(ref worldMatrix);
        }

        private Matrix GetWorldMatrix() {
            if(Billboard) {
                return scaleMatrix * originMatrix;
            } else {
                return scaleMatrix * rotationMatrix * originMatrix;
            }
        }

        private void UpdateVectorMatrices() {
            if(!Billboard) {
                UpdateRotationMatrix();
            }
            UpdateOriginMatrix();
            UpdateScaleMatrix();
        }

        private Matrix GetWorldOriginMatrix() {
            return Matrix.CreateWorld(Position,Vector3.Forward,Vector3.Up);
        }

        private Matrix GetBillboardOriginMatrix() {
            var camera = Owner.Camera;
            return Matrix.CreateConstrainedBillboard(Position,-camera.Position,new Vector3(0,1,0),null,null);
        }

        private Matrix GetOriginMatrix() {
            if(Billboard) {
                return GetBillboardOriginMatrix();
            }
            return GetWorldOriginMatrix();
        }

        private void UpdateOriginMatrix() {
            if(PositionValid) {
                return;
            }
            originMatrix = GetOriginMatrix();
            PositionValid = true;
        }

        private void UpdateScaleMatrix() {
            if(ScaleValid) {
                return;
            }
            scaleMatrix = Matrix.CreateScale(Scale);
            ScaleValid = true;
        }

        private void UpdateRotationMatrix() {
            if(RotationValid) {
                return;
            }
            var rotation = Rotation;
            rotationMatrix =  Matrix.CreateFromYawPitchRoll(
                MathHelper.ToRadians(rotation.X),
                MathHelper.ToRadians(rotation.Y),
                MathHelper.ToRadians(rotation.Z)
            );
            RotationValid = true;
        }
    }
}
