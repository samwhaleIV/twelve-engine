using nkast.Aether.Physics2D.Dynamics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Shell;

namespace TwelveEngine.Game2D {
    public class GameState2D:InputGameState, IEntitySorter<Entity2D,GameState2D> {

        private readonly World _physicsWorld = new World(Vector2.Zero);
        public World PhysicsWorld => _physicsWorld;

        public Camera Camera { get; private init; } = new Camera() { Position = Vector2.Zero };

        public GameState2D() {
            OnLoad.Add(LoadEntities);

            //OnWriteDebug.Add();
            //OnUpdate.Add(UpdateCameraScreenSize,EventPriority.First);

            OnUpdate.Add(UpdateEntities);
            OnUpdate.Add(UpdatePhysics);
            OnUpdate.Add(UpdateCamera);

            OnPreRender.Add(PreRenderEntities);
            OnRender.Add(RenderEntities);
        }

        public Texture2D SpriteSheet { get; set; }

        public EntityManager<Entity2D,GameState2D> Entities { get; private set; }
        private void LoadEntities() => Entities = new EntityManager<Entity2D,GameState2D>(this);

        private void UpdateEntities() => Entities.Update();

        //todo: possibly remove, who knows
        private void PreRenderEntities() => Entities.PreRender();

        private void RenderEntities() {
            SpriteBatch.Begin(SpriteSortMode.BackToFront,null,SamplerState.PointClamp);
            Entities.Render();
            SpriteBatch.End();
        }

        private void UpdateCamera() {
            Camera.Update(Viewport);
        }

        private void UpdatePhysics() {
            _physicsWorld.Step(FrameDelta);
        }

        public sealed class EntityPrioritySorter:IComparer<Entity2D> {
            public int Compare(Entity2D entityA,Entity2D entityB) {
                EntityPriority priorityA = entityA.Priority, priorityB = entityB.Priority;
                if(priorityA == priorityB) {
                    return entityA.ID.CompareTo(entityB.ID);
                }
                return entityA.Priority.CompareTo(entityB.Priority);
            }
        }

        public IComparer<Entity2D> GetEntitySorter() => new EntityPrioritySorter();
    }
}
