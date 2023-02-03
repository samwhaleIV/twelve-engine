using TwelveEngine.EntitySystem;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game3D {
    public class GameState3D:InputGameState,IEntitySorter<Entity3D,GameState3D> {

        private readonly EntitySortMode entitySortMode;

        public GameState3D(EntitySortMode entitySortMode,bool updateCameraAfterEntities = true) {
            this.entitySortMode = entitySortMode;

            OnLoad.Add(LoadEntities);
            OnWriteDebug.Add(Write3DInfo);

            OnUpdate.Add(UpdateCameraScreenSize,EventPriority.First);
            OnPreRender.Add(TryUpdateLocalMatrices,EventPriority.First);
            OnRender.Add(ClearLocalMatrixUpdateFlags,EventPriority.First);

            if(updateCameraAfterEntities) {
                OnUpdate.Add(UpdateEntities);
                OnUpdate.Add(UpdateCamera);
            } else {
                OnUpdate.Add(UpdateCamera);
                OnUpdate.Add(UpdateEntities);
            }
        }

        public Matrix ViewMatrix, ProjectionMatrix;

        public bool ProjectionMatrixUpdated { get; private set; } = true;
        public bool ViewMatrixUpdated { get; private set; } = true;

        private void TryUpdateLocalMatrices() {
            if(ProjectionMatrixUpdated) {
                ProjectionMatrix = _camera?.ProjectionMatrix ?? Matrix.Identity;
            }
            if(ViewMatrixUpdated) {
                ViewMatrix = _camera?.ViewMatrix ?? Matrix.Identity;
            }
        }

        private void ClearLocalMatrixUpdateFlags() {
            ProjectionMatrixUpdated = false;
            ViewMatrixUpdated = false;
        }

        private void Write3DInfo(DebugWriter writer) {
            writer.ToTopLeft();
            writer.Write(Camera.Position);
            if(Camera is not AngleCamera angleCamera) {
                return;
            }
            writer.WriteXY(angleCamera.Yaw,angleCamera.Pitch,"Yaw","Pitch");
        }

        public EntityManager<Entity3D,GameState3D> Entities { get; private set; }

        private void LoadEntities() {
            Entities = new EntityManager<Entity3D,GameState3D>(this);
        }

        private void UpdateCameraScreenSize() {
            _camera?.UpdateScreenSize(AspectRatio);
        }

        protected void UpdateCamera() {
            _camera?.Update();
        }

        protected void UpdateEntities() {
            if(entitySortMode == EntitySortMode.CameraRelative) {
                UpdateCamera();
                if(ViewMatrixUpdated) {
                    Entities.RefreshSorting();
                }
            }
            Entities.Update();
        }

        public void RenderEntities() => Entities.Render();
        public void PreRenderEntities() => Entities.PreRender();

        public float AspectRatio => Viewport.AspectRatio;

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

        protected internal override void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
            graphicsDevice.Clear(ClearColor);
        }

        public static int CameraFixedSort(Entity3D a,Entity3D b) {
            return a.Depth == b.Depth ? a.ID.CompareTo(b.ID) : a.Depth.CompareTo(b.Depth);
        }

        public sealed class CameraFixedSorter:IComparer<Entity3D> {
            /* If the depth is the same fallback to ID sorting.
             * Oldest entity is rendered on the lowest virtual layer.
             * Perceptually, newer entities are 'closer'. */
            public int Compare(Entity3D a,Entity3D b) => CameraFixedSort(a,b);
        }

        public sealed class CameraRelativeSorter:IComparer<Entity3D> {

            private readonly GameState3D gameState;
            public CameraRelativeSorter(GameState3D gameState) => this.gameState = gameState;

            public int Compare(Entity3D entityA,Entity3D entityB) {

                Camera3D camera = gameState.Camera;
                if(camera is null) {
                    return CameraFixedSort(entityA,entityB);
                }

                Vector3 forward = camera.ViewMatrix.Forward;

                float aDot = Vector3.Dot(forward,camera.Position - entityA.Position);
                float bDot = Vector3.Dot(forward,camera.Position - entityB.Position);

                if(aDot == bDot) {
                    return entityA.ID.CompareTo(entityB.ID);
                }

                return aDot.CompareTo(bDot);
            }
        }

        public IComparer<Entity3D> GetEntitySorter() => entitySortMode switch {         
            EntitySortMode.CreationOrder => new IEntitySorter<Entity3D,GameState3D>.DefaultComparison(),
            EntitySortMode.CameraFixed => new CameraFixedSorter(),
            EntitySortMode.CameraRelative => new CameraRelativeSorter(this),
            _ => throw new NotImplementedException()
        };
    }
}
