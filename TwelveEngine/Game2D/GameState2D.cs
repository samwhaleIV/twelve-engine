using nkast.Aether.Physics2D.Dynamics;
using TwelveEngine.EntitySystem;
using TwelveEngine.Shell;
using System.IO;

namespace TwelveEngine.Game2D {
    public class GameState2D:InputGameState, IEntitySorter<Entity2D,GameState2D> {

        private readonly World _physicsWorld = new World(Vector2.Zero);

        public World PhysicsWorld => _physicsWorld;
        public float PhysicsScale { get; protected init; } = 1f;

        public Camera Camera { get; private init; } = new Camera() { Position = Vector2.Zero };

        public GameState2D() {
            OnLoad.Add(LoadEntities);

            OnUpdate.Add(UpdateEntities);
            OnUpdate.Add(UpdatePhysics);
            OnUpdate.Add(UpdateCamera,EventPriority.Last);

            OnPreRender.Add(PreRenderEntities);

            OnRender.Add(RenderMapBackground);
            OnRender.Add(RenderEntities);
            //OnRender.Add(RenderMapForeground);
        }

        public TileMap TileMap { get; set; } = new TileMap() { Width = 0, Height = 0, Data = null };
        public Texture2D TileMapTexture { get; set; }
        public readonly Dictionary<ushort,TileData> TileDictionary = new Dictionary<ushort,TileData>();

        private readonly TileData DebugRedTile = new TileData() { Color = Color.Red,Source = new Rectangle(16,0,16,16) };
        private readonly TileData DebugBlueTile = new TileData() { Color = Color.Blue,Source = new Rectangle(16,0,16,16) };

        private void RenderMapBackground() {

            if(TileMap.Data == null || TileMapTexture == null) {
                return;
            }

            Vector2 tileSize = Camera.TileSize, renderOrigin = Camera.RenderOrigin;
            Point tileStride = Camera.TileStride, tileStart = Camera.TileStart;

            TileData tileData = new();
            float rotation = 0f, layerDepth = 1f;

            bool drawThisTile = false;

            float yStart;

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            for(int x = 0;x < tileStride.X;x++) {

                yStart = renderOrigin.Y;

                for(int y = 0;y < tileStride.Y;y++) {

                    drawThisTile = false;

                    if(x == 0 && y == 0) {
                        tileData = DebugRedTile;
                        drawThisTile = true;
                    } else if(x == tileStride.X - 1 && y == tileStride.Y - 1) {
                        tileData = DebugBlueTile;
                        drawThisTile = true;
                    }

                    drawThisTile = drawThisTile || TileMap.TryGetValue(tileStart.X + x,tileStart.Y + y,out ushort value) && TileDictionary.TryGetValue(value,out tileData);

                    if(drawThisTile) {
                        SpriteBatch.Draw(TileMapTexture,renderOrigin,tileData.Source,tileData.Color,rotation,Vector2.Zero,Camera.Scale,tileData.SpriteEffects,layerDepth);
                    }

                    renderOrigin.Y += tileSize.Y;
                }
                renderOrigin.Y = yStart;
                renderOrigin.X += tileSize.X;
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

        public void AddCollisionRectangle(Vector2 location,Vector2 size,float friction,float mass) {
            var body = PhysicsWorld.CreateBody(location * PhysicsScale,0f,BodyType.Static);
            body.Mass = mass;
            var fixture = body.CreateRectangle(size.X * PhysicsScale,size.Y * PhysicsScale,1f,Vector2.Zero);
            fixture.Friction = friction;
        }

        public void ImportTiledCSV(string file,HashSet<ushort> collisionValues,bool setCameraBounds = true,float surfaceFriction = 1f,float surfaceMass = 10f) {
            string[] lines = File.ReadAllLines(file);
            int height = lines.Length;
            int width = 1;
            string firstLine = lines[0];
            foreach(char character in firstLine) {
                if(character == ',') width++;
            }
            ushort[] tileData = new ushort[width * height];

            TileMap tileMap = new TileMap() {
                Width = width,
                Height = height,
                Data = tileData
            };

            int y = 0;
            foreach(var line in lines) {
                string[] splitLine = line.Split(',',StringSplitOptions.TrimEntries);
                if(splitLine.Length != width) {
                    throw new IndexOutOfRangeException("This row does not contain the right number of values.");
                }
                for(int x = 0;x<width;x++) {
                    if(!ushort.TryParse(splitLine[x],out ushort tileValue)) {
                        continue;
                    }
                    if(collisionValues.Contains(tileValue)) {
                        AddCollisionRectangle(new Vector2(x,y),Vector2.One,surfaceFriction,surfaceMass);
                    }
                    tileMap.SetValue(x,y,tileValue);
                }
                y++;
            }

            TileMap = tileMap;

            if(!setCameraBounds) {
                return;
            }
            Camera.MinX = 0;
            Camera.MaxX = width;
            Camera.MinY = 0;
            Camera.MaxY = height;
        }
    }
}
