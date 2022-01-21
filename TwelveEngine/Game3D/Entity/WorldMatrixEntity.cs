using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D.Entity {
    public abstract class WorldMatrixEntity:Entity3D {

        private Vector3? lastCameraPosition = null;

        private Matrix originMatrix, rotationMatrix, scaleMatrix, worldMatrix;

        protected void UpdateWorldMatrix(Action<Matrix> onMatrixChanged) {

            if(Billboard && Owner.Camera.Position != lastCameraPosition) {
                PositionValid = false;
                WorldMatrixValid = false;
            }

            if(WorldMatrixValid) {
                return;
            }

            UpdateVectorMatrices();

            worldMatrix = GetWorldMatrix();
            WorldMatrixValid = true;

            onMatrixChanged?.Invoke(worldMatrix);
        }

        private Matrix GetWorldMatrix() {
            var matrix = originMatrix * scaleMatrix;
            if(!Billboard) {
                matrix = matrix * rotationMatrix;
            }
            return matrix;
        }

        private void UpdateVectorMatrices() {
            if(!Billboard) {
                UpdateRotationMatrix();
            }
            UpdateOriginMatrix();
            UpdateScaleMatrix();
        }

        private Matrix GetWorldOriginMatrix() {
            return Matrix.CreateWorld(Position,Orientation.Forward,Orientation.Up);
        }

        private Matrix GetBillboardOriginMatrix() {
            var camera = Owner.Camera as AngleCamera;
            if(camera == null) return GetWorldMatrix();

            var rotationAxis = new Vector3(0,0,1);

            lastCameraPosition = camera.Position;

            return Matrix.CreateConstrainedBillboard(
                Position,
                camera.Position,
                rotationAxis,
                camera.Forward,
                Orientation.Forward
            );
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
                MathHelper.ToRadians(rotation.X + Orientation.WorldYawOffset),
                MathHelper.ToRadians(rotation.Y + Orientation.WorldPitchOffset),
                MathHelper.ToRadians(rotation.Z)
            );
            RotationValid = true;
        }
    }
}
