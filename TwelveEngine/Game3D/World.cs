using System;
using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;

namespace TwelveEngine.Game3D {
    public class World:GameState {

        private readonly EntityFactory<Entity3D,World> entityFactory;

        public World(EntityFactory<Entity3D,World> entityFactory = null) {
            if(entityFactory == null ) {
                entityFactory = Entity3DType.GetFactory();
            }
            this.entityFactory = entityFactory;

            OnLoad += SetupEntityFactory;
            OnUpdate += World_OnUpdate;

            cameraSerializer = new CameraSerializer(this);
            OnImport += cameraSerializer.ImportCamera;
            OnExport += cameraSerializer.ExportCamera;
        }

        private void World_OnUpdate(GameTime gameTime) {
            Entities.IterateMutable(Entity3D.Update,gameTime);
            _camera?.Update(GetAspectRatio());
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
        private void SetupEntityFactory() {
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

    }
}
