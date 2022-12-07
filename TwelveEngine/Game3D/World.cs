using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell.States;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game3D {
    public class World:InputGameState {

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public World() {
            OnLoad += World_OnLoad;
            OnUnload += World_OnUnload;
            OnUpdate += World_OnUpdate;
            OnRender += World_OnRender;
            OnWriteDebug += World_OnWriteDebug;
        }

        private void World_OnWriteDebug(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Camera.Position);
            if(!(Camera is AngleCamera angleCamera)) {
                return;
            }
            writer.WriteXY(angleCamera.Yaw,angleCamera.Pitch,"Yaw","Pitch");
        }

        private VertexBuffer currentVertexBuffer = null;
        private IndexBuffer currentIndexBuffer = null;


        public void ApplyBuffer(SharedBuffer buffer) => ApplyBuffer(buffer.Vertices,buffer.Indices);
        public void ApplySharedBuffer() => ApplyBuffer(sharedBuffer.Vertices,sharedBuffer.Indices);

        public void ApplyBuffer(VertexBuffer vertices,IndexBuffer indices) {
            if(currentVertexBuffer != vertices) {
                GraphicsDevice.SetVertexBuffer(vertices);
                currentVertexBuffer = vertices;
            }
            if(currentIndexBuffer != indices) {
                GraphicsDevice.Indices = indices;
                currentIndexBuffer = indices;
            }
        }

        public EntityManager<Entity3D,World> Entities { get; private set; }

        private SharedBuffer sharedBuffer = null;
        public SharedBuffer SharedBuffer => sharedBuffer;

        private void World_OnLoad() {
            var sharedBuffer = new SharedBuffer();
            sharedBuffer.Load(GraphicsDevice);
            this.sharedBuffer = sharedBuffer;
            Entities = new EntityManager<Entity3D,World>(this);
            ApplySharedBuffer();
        }

        private void World_OnUnload() {
            sharedBuffer?.Unload();
            sharedBuffer = null;
        }

        public Color ClearColor { get; set; } = Color.Black;

        private void World_OnRender(GameTime gameTime) {
            Game.GraphicsDevice.Clear(ClearColor);
            GraphicsDevice.DepthStencilState = DepthStencilState.Default;
            GraphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }

        private void World_OnUpdate(GameTime gameTime) {
            UpdateInputs(gameTime);
            _camera?.Update(AspectRatio); /* An entity might need to use orthographic projection information */
            Entities.Iterate(Entity3D.Update,gameTime);
            _camera?.Update(AspectRatio);
        }

        public void RenderEntities(GameTime gameTime) {
            Entities.IterateDepthSorted(Entity3D.Render,gameTime);
        }

        public void PreRenderEntities(GameTime gameTime) {
            Entities.Iterate(Entity3D.PreRender,gameTime);
        }

        private Camera3D _camera;
        public Camera3D Camera { get => _camera; set => SetNewCamera(value); }

        public Matrix ViewMatrix => _camera?.ViewMatrix ?? Matrix.Identity;
        public Matrix ProjectionMatrix => _camera?.ProjectionMatrix ?? Matrix.Identity;

        public event Action<Matrix> OnViewMatrixChanged, OnProjectionMatrixChanged;

        private void FireProjectionMatrixChanged(Matrix projectionMatrix) {
            OnProjectionMatrixChanged?.Invoke(projectionMatrix);
        }
        private void FireViewMatrixChanged(Matrix viewMatrix) {
            OnViewMatrixChanged?.Invoke(viewMatrix);
        }

        public float AspectRatio => Game.Viewport.AspectRatio;

        private void SetNewCamera(Camera3D newCamera) {
            var oldCamera = _camera;
            if(newCamera == oldCamera) {
                return;
            }
            if(oldCamera != null) {
                oldCamera.OnProjectionMatrixChanged -= FireProjectionMatrixChanged;
                oldCamera.OnViewMatrixChanged -= FireViewMatrixChanged;
            }
            _camera = newCamera;
            if(newCamera == null) {
                FireProjectionMatrixChanged(Matrix.Identity);
                FireViewMatrixChanged(Matrix.Identity);
                return;
            }

            newCamera.OnProjectionMatrixChanged += FireProjectionMatrixChanged;
            newCamera.OnViewMatrixChanged += FireViewMatrixChanged;

            FireProjectionMatrixChanged(newCamera.ProjectionMatrix);
            FireViewMatrixChanged(newCamera.ViewMatrix);
        }

        public BufferSet CreateBufferSet<TVertices>(TVertices[] vertices) where TVertices : struct {
            return BufferSet.Create(GraphicsDevice,vertices);
        }
    }
}
