using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D.Entity {
    public abstract class WorldMatrixEntity:Entity3D {

        private Matrix originMatrix, rotationMatrix, scaleMatrix, worldMatrix;

        protected void UpdateWorldMatrix(Action<Matrix> onMatrixChanged) {
            if(WorldMatrixValid) {
                return;
            }

            UpdateVectorMatrices();

            worldMatrix = GetWorldMatrix();
            WorldMatrixValid = true;

            onMatrixChanged?.Invoke(worldMatrix);
        }

        private Matrix GetWorldMatrix() {
            return originMatrix * scaleMatrix * rotationMatrix;
        }

        private void UpdateVectorMatrices() {
            UpdateOriginMatrix();
            UpdateScaleMatrix();
            UpdateRotationMatrix();
        }

        private void UpdateOriginMatrix() {
            if(PositionValid) {
                return;
            }
            originMatrix = Matrix.CreateWorld(Position,Orientation.WorldForward,Orientation.WorldUp);
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
            rotationMatrix =  Matrix.CreateFromYawPitchRoll(rotation.X,rotation.Y,rotation.Z);
            RotationValid = true;
        }
    }
}
