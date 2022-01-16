using System;
using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game3D.Camera;

namespace TwelveEngine.Game3D {
    public sealed class ModelViewer:DataGameState<string> {

        public ModelViewer() {
            OnLoad += ModelTest_OnLoad;

            OnUpdate += ModelTest_OnUpdate;
            OnRender += ModelTest_OnRender;

            camera.NearPlane = 0.1f;
            camera.Position = new Vector3(0,0,-0.25f);
            camera.Target = Vector3.Zero;

            OnImport += ModelViewer_OnImport;
            OnExport += ModelViewer_OnExport;
        }

        private void ModelViewer_OnExport(SerialFrame frame) => frame.Set(angle);

        private void ModelViewer_OnImport(SerialFrame frame) {
            var startAngle = angle;
            angle = frame.GetVector3();
            if(startAngle != angle) {
                UpdateModelMatrix();
            }
        }

        public float RotationSpeed { get; set; } = 0.05f;
        private const float VELOCITY_BASE = 1f / (1000f / 60f);

        private Model model;

        private readonly TargetCamera camera = new TargetCamera();

        private static Matrix GetOriginMatrix() {
            return Matrix.CreateWorld(Vector3.Zero,Vector3.Forward,Vector3.Up);
        }

        private readonly Matrix originMatrix = GetOriginMatrix();

        public TargetCamera Camera => camera;

        private Vector3 angle = Vector3.Zero;
        private Matrix modelMatrix = GetOriginMatrix();

        private void ModelTest_OnLoad() => model = Game.Content.Load<Model>(Data);

        private float GetRotationVelocity(GameTime gameTime) {
            return (float)gameTime.ElapsedGameTime.TotalMilliseconds * VELOCITY_BASE * RotationSpeed;
        }

        private void UpdateModelMatrix() {
            modelMatrix = originMatrix * Matrix.CreateFromYawPitchRoll(angle.X,angle.Y,angle.Z);
        }

        private void ModelTest_OnUpdate(GameTime gameTime) {
            camera.UpdateMatrices(Game.GraphicsDevice.Viewport.AspectRatio);

            var rotationDelta = Input.Get3DRotationDelta();
            if(rotationDelta == Vector3.Zero) {
                return;
            }

            float velocity = GetRotationVelocity(gameTime);
            angle += rotationDelta * new Vector3(velocity);

            UpdateModelMatrix();
        }

        private void UpdateMeshEffect(BasicEffect effect) {
            effect.Projection = camera.ProjectionMatrix;
            effect.View = camera.ViewMatrix;
            effect.World = modelMatrix;
        }

        private void ConfigureSamplerState() {
            Game.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
        }

        private void ModelTest_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Gray);
            ConfigureSamplerState();
            foreach(var mesh in model.Meshes) {
                foreach(BasicEffect effect in mesh.Effects) {
                    UpdateMeshEffect(effect);
                }
                mesh.Draw();
            }
        }

    }
}
