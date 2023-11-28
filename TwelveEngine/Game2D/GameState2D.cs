using Microsoft.Xna.Framework.Graphics;
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

            OnUpdate.Add(UpdateEntities);
            OnUpdate.Add(UpdatePhysics);
            OnUpdate.Add(UpdateCamera);

            OnPreRender.Add(PreRenderEntities);

            OnRender.Add(RenderMapBackground);
            OnRender.Add(RenderEntities);
            //OnRender.Add(RenderMapForeground);
        }

        public TileMap TileMap { get; set; } = new TileMap() { Width = 0, Height = 0, Data = null };
        public Texture2D TileMapTexture { get; set; }
        public readonly Dictionary<ushort,TileData> TileDictionary = new Dictionary<ushort,TileData>();

        private void RenderMapBackground() {

            if(TileMap.Data == null || TileMapTexture == null) {
                return;
            }

            FloatRectangle tileBounds = Camera.TileBounds;
            Vector2 tileSize = new Vector2(Camera.TileRenderSize);
            Vector2 renderOffset = Vector2.Subtract(Vector2.Floor(tileBounds.TopLeft),tileBounds.TopLeft);
            Point tileLocation = (Vector2.Ceiling(renderOffset) + tileBounds.Position).ToPoint();
            Point tileStride = Vector2.Ceiling(tileBounds.Size - renderOffset).ToPoint();
            Vector2 renderLocation = Vector2.Round(renderOffset * tileSize);

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            for(int x = 0;x < tileStride.X;x++) {
                float yStart = renderLocation.Y;
                for(int y = 0;y < tileStride.Y;y++) {
                    if(TileMap.TryGetValue(tileLocation.X + x,tileLocation.Y + y,out ushort value) && TileDictionary.TryGetValue(value, out TileData tileData)) {
                        float rotation = 0f;
                        float layerDepth = 1f;
                        SpriteBatch.Draw(TileMapTexture,renderLocation,tileData.Source,tileData.Color,rotation,Vector2.Zero,Camera.Scale,tileData.SpriteEffects,layerDepth);
                    }
                    renderLocation.Y += tileSize.Y;
                }
                renderLocation.Y = yStart;
                renderLocation.X += tileSize.X;
            }
            SpriteBatch.End();
        }

        private void RenderMapForeground() {
            throw new NotImplementedException();
        }

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
