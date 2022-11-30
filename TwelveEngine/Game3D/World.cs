using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell.States;

namespace TwelveEngine.Game3D {
    public class World:InputGameState {

        private readonly EntityFactory<Entity3D,World> entityFactory;

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;

        public World(EntityFactory<Entity3D,World> entityFactory = null) {
            if(entityFactory == null) {
                entityFactory = Entity3DType.GetFactory();
            }
            this.entityFactory = entityFactory;

            OnLoad += SetupEntityManager;
            OnUpdate += World_OnUpdate;
            OnRender += World_OnRender;

            cameraSerializer = new CameraSerializer(this);
            OnImport += cameraSerializer.ImportCamera;
            OnExport += cameraSerializer.ExportCamera;
        }

        private void World_OnRender(GameTime gameTime) {
            ResetGraphicsDeviceState(Game.GraphicsDevice);
        }

        private void World_OnUpdate(GameTime gameTime) {
            UpdateInputs(gameTime);
            var aspectRatio = GetAspectRatio();
            _camera?.Update(aspectRatio); /* An entity might need to use orthographic projection information */
            Entities.IterateMutable(Entity3D.Update,gameTime);
            _camera?.Update(aspectRatio);
        }

        public void RenderEntities(GameTime gameTime) {
            Entities.IterateImmutable(Entity3D.Render,gameTime);
        }

        public void PreRenderEntities(GameTime gameTime) {
            Entities.IterateImmutable(Entity3D.PreRender,gameTime);
        }

        private readonly CameraSerializer cameraSerializer;

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

        public EntityManager<Entity3D,World> Entities { get; private set; }
        private void SetupEntityManager() {
            Entities = new EntityManager<Entity3D,World>(this,entityFactory);
        }

        public float GetAspectRatio() => Game.Viewport.AspectRatio;

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
            newCamera.Update(GetAspectRatio());

            newCamera.OnProjectionMatrixChanged += FireProjectionMatrixChanged;
            newCamera.OnViewMatrixChanged += FireViewMatrixChanged;

            FireProjectionMatrixChanged(newCamera.ProjectionMatrix);
            FireViewMatrixChanged(newCamera.ViewMatrix);
        }

        public BufferSet CreateBufferSet<TVertices>(TVertices[] vertices) where TVertices:struct {
            return BufferSet.Create(GraphicsDevice,vertices);
        }

        protected virtual void ResetGraphicsDeviceState(GraphicsDevice graphicsDevice) {
            graphicsDevice.Clear(Color.Gray);
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
        }
    }
}
