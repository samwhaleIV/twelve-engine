﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game3D {
    public class GameState3D:InputGameState {

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public GameState3D() {
            OnLoad += GameState3D_OnLoad;
            OnUpdate += UpdateGame;
            OnWriteDebug += GameState3D_OnWriteDebug;
            OnPreRender += GameState3D_OnPreRender;
            OnRender += GameState3D_OnRender;
        }

        public Matrix ViewMatrix, ProjectionMatrix;

        public bool ProjectionMatrixUpdated { get; private set; } = true;
        public bool ViewMatrixUpdated { get; private set; } = true;

        private void GameState3D_OnPreRender() {
            if(ProjectionMatrixUpdated) {
                ProjectionMatrix = _camera?.ProjectionMatrix ?? Matrix.Identity;
            }
            if(ViewMatrixUpdated) {
                ViewMatrix = _camera?.ViewMatrix ?? Matrix.Identity;
            }
        }
        private void GameState3D_OnRender() {
            ProjectionMatrixUpdated = false;
            ViewMatrixUpdated = false;
        }

        private void GameState3D_OnWriteDebug(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Camera.Position);
            if(Camera is not AngleCamera angleCamera) {
                return;
            }
            writer.WriteXY(angleCamera.Yaw,angleCamera.Pitch,"Yaw","Pitch");
        }

        public EntityManager<Entity3D,GameState3D> Entities { get; private set; }

        private void GameState3D_OnLoad() {
            Entities = new EntityManager<Entity3D,GameState3D>(this);
        }

        protected void UpdateCameraScreenSize() {
            _camera?.UpdateScreenSize(AspectRatio);
        }

        protected void UpdateCamera() {
            _camera?.Update();
        }

        protected virtual void UpdateGame() {
            UpdateInputs();
            UpdateCameraScreenSize();
            Entities.Update();
            UpdateCamera();
        }

        public void RenderEntities() {
            Entities.Render();
        }

        public void PreRenderEntities() {
            Entities.PreRender();
        }

        public float AspectRatio => Game.Viewport.AspectRatio;

        private Camera3D _camera;
        public Camera3D Camera { get => _camera; set => SetNewCamera(value); }

        private void OnProjectionMatrixChanged() {
            ProjectionMatrixUpdated = true;
        }

        private void OnViewMatrixChanged() {
            ViewMatrixUpdated = true;
        }

        private void SetNewCamera(Camera3D newCamera) {
            Camera3D oldCamera = _camera;
            if(newCamera == oldCamera) {
                return;
            }
            _camera = newCamera;
            if(oldCamera is not null) {
                oldCamera.OnViewMatrixChanged -= OnViewMatrixChanged;
                oldCamera.OnProjectionMatrixChanged -= OnProjectionMatrixChanged;
            }
            ViewMatrixUpdated = true;
            ProjectionMatrixUpdated = true;
            if(newCamera is null) {
                return;
            }
            newCamera.OnViewMatrixChanged += OnViewMatrixChanged;
            newCamera.OnProjectionMatrixChanged += OnProjectionMatrixChanged;
        }

        public BufferSet CreateBufferSet<TVertices>(TVertices[] vertices) where TVertices:struct {
            return BufferSet.Create(GraphicsDevice,vertices);
        }

        public override void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
            Game.GraphicsDevice.Clear(ClearColor);
        }
    }
}
