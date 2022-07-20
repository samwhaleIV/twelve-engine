using Microsoft.Xna.Framework;
using tainicom.Aether.Physics2D.Dynamics;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game2D.Objects;
using TwelveEngine.Serial;

namespace TwelveEngine.Game2D {
    public class PhysicsGrid2D:Grid2D {

        private readonly World _physicsWorld;
        public World PhysicsWorld => _physicsWorld;

        private readonly ObjectManager _objectManager;
        public ObjectManager Objects => _objectManager;

        private string pendingAtlas = null;
        private Texture2D _atlas = null;
        public Texture2D Atlas => _atlas;

        private void LoadAtlas() {
            if(pendingAtlas == null) {
                return;
            }
            _atlas = Game.Content.Load<Texture2D>(pendingAtlas);
            pendingAtlas = null;
        }

        public void SetAtlas(string atlasTexture) {
            pendingAtlas = atlasTexture;
            if(atlasTexture == null) {
                _atlas = null;
                return;
            }
            if(!IsLoaded) {
                return;
            }
            LoadAtlas();
        }

        public PhysicsGrid2D() {
            _physicsWorld = new World(Vector2.Zero);
            _objectManager = new ObjectManager(this);

            OnImport +=PhysicsGrid2D_OnImport;
            OnExport += PhysicsGrid2D_OnExport;

            OnUpdate += PhysicsGrid_OnUpdate;

            OnLoad += PhysicsGrid2D_OnLoad;
            OnUnload += PhysicsGrid2D_OnUnload;
        }

        private void PhysicsGrid2D_OnExport(SerialFrame frame) {
            frame.Set(Atlas.Name);
            Objects.Export(frame);
        }

        private void PhysicsGrid2D_OnImport(SerialFrame frame) {
            SetAtlas(frame.GetString());
            Objects.Import(frame);
        }

        private void PhysicsGrid2D_OnLoad() {
            LoadAtlas();
            Objects.Load();
        }

        private void PhysicsGrid2D_OnUnload() {
            Objects.Unload();
        }

        private void PhysicsGrid_OnUpdate(GameTime gameTime) {
            _physicsWorld.Step((float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        public bool RenderObjectHitboxes { get; set; }
        public Color HitboxColor { get; set; } = Color.FromNonPremultiplied(byte.MaxValue,0,0,byte.MaxValue / 2);
        private const float HITBOX_RENDER_DENOMINATOR = 256f;

        protected virtual void RenderObject(GameObject gameObject) {
            if(gameObject.Invisible || !OnScreen(gameObject)) {
                return;
            }
            Vector2 size = gameObject.Size, halfSize = size * 0.5f;
            float rotation = gameObject.Rotation;
            Rectangle source = gameObject.TextureSource;
            VectorRectangle destination = GetDestination(gameObject.Position + halfSize,size,source.Size);
            Vector2 origin = source.Size.ToVector2() * 0.5f;
            float depth = GetRenderDepth(((gameObject.Position - ScreenSpace.Location) * ScreenSpace.TileSize).Y);
            Game.SpriteBatch.Draw(Atlas,destination.Location,source,gameObject.Color,rotation,origin,destination.Size,gameObject.SpriteEffects,depth);
            if(!RenderObjectHitboxes || !(gameObject is PhysicsGameObject physicsGameObject)) {
                return;
            }
            Rectangle hitboxSource = new Rectangle(Point.Zero,(new Vector2(HITBOX_RENDER_DENOMINATOR) * physicsGameObject.HitboxSize).ToPoint());
            Vector2 originOffset = gameObject.Position + halfSize - physicsGameObject.HitboxOffset;
            destination = GetDestination(physicsGameObject.HitboxOffset + originOffset,physicsGameObject.HitboxSize,hitboxSource.Size);
            origin = originOffset * HITBOX_RENDER_DENOMINATOR;
            Game.SpriteBatch.Draw(BlankTexture,destination.Location,hitboxSource,HitboxColor,rotation,origin,destination.Size,SpriteEffects.None,depth);
        } 

        protected override void RenderGrid(GameTime gameTime) {
            Game.SpriteBatch.Begin(SpriteSortMode.BackToFront,null,SamplerState.PointClamp);
            RenderEntities(gameTime);
            foreach(GameObject gameObject in Objects.GetEnumerable()) {
                RenderObject(gameObject);
            }
            Game.SpriteBatch.End();
        }
    }
}
