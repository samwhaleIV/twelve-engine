using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game3D {
    public abstract class Camera3D {

        public Vector3 Position { get; set; }
        public float FieldOfView { get; set; } = 75f;
        public float NearPlane { get; set; } = 1f;
        public float FarPlane { get; set; } = 100f;
        public bool Orthographic { get; set; } = false;

        protected abstract Matrix GetViewMatrix();

        public Matrix ViewMatrix { get; private set; }
        public Matrix ProjectionMatrix { get; private set; }

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
            if(Orthographic) {
                return Matrix.CreateOrthographic(width,height,NearPlane,FarPlane);
            } else {
                return Matrix.CreatePerspectiveFieldOfView(GetFieldOfView(),aspectRatio,NearPlane,FarPlane);
            }
        }

        public void Update(float aspectRatio) {
            ProjectionMatrix = GetProjectionMatrix(aspectRatio);
            ViewMatrix = GetViewMatrix();
        }
    }
}
