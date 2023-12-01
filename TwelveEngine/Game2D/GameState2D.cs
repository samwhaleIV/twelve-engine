﻿using nkast.Aether.Physics2D.Dynamics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Shell;

namespace TwelveEngine.Game2D {
    public class GameState2D:InputGameState, IEntitySorter<Entity2D,GameState2D> {

        public float PhysicsScale { get; protected init; } = 1f;
        public World PhysicsWorld { get; private init; } = new World(Vector2.Zero);

        public Camera Camera { get; private init; } = new Camera() { Position = Vector2.Zero };

        public GameState2D() {
            OnLoad.Add(LoadEntities);

            OnUpdate.Add(UpdateEntities);
            OnUpdate.Add(UpdatePhysics);
            OnUpdate.Add(UpdateCamera,EventPriority.Last);

            OnRender.Add(RenderMapBackground);
            OnRender.Add(RenderEntities);
        }

        public TileMap TileMap { get; set; } = new TileMap() { Width = 0, Height = 0, Data = null };
        public Texture2D TileMapTexture { get; set; }
        public readonly Dictionary<short,TileData> TileDictionary = new Dictionary<short,TileData>();

        private const float BACKGROUND_DEPTH = 0.5f, FOREGROUND_DEPTH = 0.75f;

        private const int BACKGROUND_LAYER = 0, FOREGROUND_LAYER = 1;

        private void RenderMapBackground() {

            if(TileMap.Data == null || TileMapTexture == null) {
                return;
            }

            Vector2 tileSize = Camera.TileSize, renderOrigin = Camera.RenderOrigin;
            Point tileStride = Camera.TileStride, tileStart = Camera.TileStart;

            SpriteBatch.Begin(SpriteSortMode.FrontToBack,null,SamplerState.PointClamp);
            for(int x = 0;x < tileStride.X;x++) {
                float yStart = renderOrigin.Y;
                for(int y = 0;y < tileStride.Y;y++) {
                    Point tileIndex = new Point(tileStart.X + x,tileStart.Y + y);
                    if(!TileMap.InRange(tileIndex)) {
                        renderOrigin.Y += tileSize.Y;
                        continue;
                    }
                    TileData tileData;
                    if(TileDictionary.TryGetValue(TileMap.GetValue(tileIndex,BACKGROUND_LAYER),out tileData)) {
                        SpriteBatch.Draw(TileMapTexture,renderOrigin,tileData.Source,tileData.Color,0f,Vector2.Zero,Camera.Scale,tileData.SpriteEffects,BACKGROUND_DEPTH);
                    }
                    if(TileDictionary.TryGetValue(TileMap.GetValue(tileIndex,FOREGROUND_LAYER),out tileData)) {
                        SpriteBatch.Draw(TileMapTexture,renderOrigin,tileData.Source,tileData.Color,0f,Vector2.Zero,Camera.Scale,tileData.SpriteEffects,FOREGROUND_DEPTH);
                    }
                    renderOrigin.Y += tileSize.Y;
                }
                renderOrigin.Y = yStart;
                renderOrigin.X += tileSize.X;
            }
            SpriteBatch.End();
        }

        public EntityManager<Entity2D,GameState2D> Entities { get; private set; }

        private void LoadEntities() => Entities = new EntityManager<Entity2D,GameState2D>(this);
        private void UpdateEntities() => Entities.Update();

        private void RenderEntities() {
            SpriteBatch.Begin(SpriteSortMode.BackToFront,null,SamplerState.PointClamp);
            Entities.Render();
            SpriteBatch.End();
        }

        private void UpdateCamera() {
            Camera.Update(Viewport);
        }

        private void UpdatePhysics() {
            PhysicsWorld.Step(FrameDelta);
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

        public void SetCameraBounds() {
            Camera.MinX = 0;
            Camera.MaxX = TileMap.Width;
            Camera.MinY = 0;
            Camera.MaxY = TileMap.Height;
        }

        public void GenerateWorldCollision(Func<short,bool> isCollision,float surfaceFriction = 1f) {
            var tileEdgeDetector = new TileEdgeDetector(TileMap.Width,TileMap.Height);
            for(int x = 0;x<TileMap.Width;x++) {
                for(int y = 0;y<TileMap.Height;y++) {
                    short tileValue = TileMap.GetValue(x,y);
                    if(!isCollision(tileValue)) {
                        continue;
                    }
                    tileEdgeDetector.AddCollision(x,y);
                }
            }
            var terrainBody = PhysicsWorld.CreateBody(new Vector2(-0.5f*PhysicsScale,-0.5f*PhysicsScale),0f,BodyType.Static);
            foreach(var edge in tileEdgeDetector.CreateEdges()) {
                var fixture = terrainBody.CreateEdge(edge.Item1 * PhysicsScale,edge.Item2 * PhysicsScale);
                fixture.Friction = surfaceFriction;
            }
        }

        public void GenerateGenericTileDictionary(int tileSize,int spriteSheetColumns,int spriteSheetRows) {
            for(int x = 0;x<spriteSheetColumns;x++) {
                for(int y = 0;y<spriteSheetRows;y++) {
                    TileDictionary.Add((short)(y * spriteSheetColumns + x),new TileData() {
                        Source = new Rectangle(x*tileSize,y*tileSize,tileSize,tileSize),
                        Color = Color.White,
                        SpriteEffects = SpriteEffects.None
                    });
                }
            }
        }

        public void GenerateGenericTileDictionary() {
            int tileSize = Camera.TileInputSize;
            int spriteSheetColumns = TileMapTexture.Width / tileSize;
            int spriteSheetRows = TileMapTexture.Height / tileSize;
            GenerateGenericTileDictionary(tileSize,spriteSheetColumns,spriteSheetRows);
        }
    }
}
