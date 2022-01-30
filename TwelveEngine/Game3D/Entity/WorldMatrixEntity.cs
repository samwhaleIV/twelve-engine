﻿using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D.Entity {
    public abstract class WorldMatrixEntity:Entity3D {

        private Vector3? lastCameraPosition = null;

        private Matrix originMatrix, rotationMatrix, scaleMatrix, worldMatrix;

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

        protected void UpdateWorldMatrix(Action<Matrix> onMatrixChanged) {
            ValidateBillboard();

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
                matrix *= rotationMatrix;
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
            return Matrix.CreateWorld(Position,Vector3.Forward,Vector3.Up);
        }

        private Matrix GetBillboardOriginMatrix() {
            var rotationAxis = new Vector3(0,0,1);
            return Matrix.CreateConstrainedBillboard(
                Position,Owner.Camera.Position,rotationAxis,
                Orientation.Forward,Orientation.Forward
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
                MathHelper.ToRadians(rotation.X),
                MathHelper.ToRadians(rotation.Y),
                MathHelper.ToRadians(rotation.Z)
            );
            RotationValid = true;
        }
    }
}
