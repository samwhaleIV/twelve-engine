using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Game2D;
using TwelveEngine.Input;
using nkast.Aether.Physics2D.Dynamics;

namespace John {
    public sealed class GrabbyClaw:Entity2D {

        private readonly CollectionGame _game;
        public GrabbyClaw(CollectionGame game) => _game = game;

        protected override void Load() {
            Owner.Impulse.OnEvent += Impulse_OnEvent;
        }

        protected override void Unload() {
            Owner.Impulse.OnEvent -= Impulse_OnEvent;
        }

        private void Impulse_OnEvent(ImpulseEvent impulseEvent) {
            if(impulseEvent.Impulse == Impulse.Accept && impulseEvent.Pressed) {
                ToggleGrip();
            }
        }

        public float MinX { get; set; } = float.NegativeInfinity;
        public float MinY { get; set; } = float.NegativeInfinity;
        public float MaxX { get; set; } = float.PositiveInfinity;
        public float MaxY { get; set; } = float.PositiveInfinity;

        public bool IsGripping { get; private set; } = false;


        private void ToggleGrip() {
            if(IsGripping) {
                Release();
            } else {
                Grab();
            }
        }

        private WalkingJohn _heldJohn = null;

        public void Release() {
            if(!IsGripping) {
                return;
            }

            if(_heldJohn == null) {
                IsGripping = false;
                return;
            }

            if(!_game.TileMap.TryGetValue(Vector2.Floor(_position).ToPoint(),out short value) || !Constants.NON_COLLIDING_TILES.Contains(value)) {
                return;
            }

            _heldJohn.Body.Enabled = true;
            _heldJohn.IsGrabbed = false;
            _heldJohn = null;
            IsGripping = false;
        }

        public void Grab() {
            if(IsGripping) {
                return;
            }
            IsGripping = true;

            /* I am absolutely baffled why this offset is required. I have no idea why it needs one. */
            var fixture = _game.TestPoint(_position - new Vector2(0.5f));

            if(fixture == null) {
                return;
            }
            if(!fixture.CollisionCategories.HasFlag(Category.Cat2)) {
                return;
            }
            if(!_game.JohnPool.TryGetJohn(fixture,out WalkingJohn john)) {
                return;
            }
            john.Body.Enabled = false;
            john.IsGrabbed = true;
            _game.Camera.GetRenderLocation(john);
            _heldJohn = john;
        }

        protected override void Update() {
            var delta = Owner.Impulse.GetDelta2D();

            _position += delta * Constants.CLAW_SPEED;

            if(_position.X < MinX) {
                _position.X = MinX;
            } else if(_position.X >= MaxX) {
                _position.X = MaxX;
            }

            if(_position.Y < MinY) {
                _position.Y = MinY;
            } else if(_position.Y >= MaxY) {
                _position.Y = MaxY;
            }

            if(_heldJohn == null) {
                return;
            }

            _heldJohn.Position = _position;
        }

        private readonly Rectangle ClosedClawSource = new(80,16,32,32), OpenClawSource = new(112,16,32,32), RopeSource = new(80,0,32,16);

        protected override void Render() {
            Vector2 position = Owner.Camera.GetRenderLocation(this);

            float rotation = 0f;
            Vector2 origin = new Vector2(16,30);

            Vector2 scale = new Vector2(Owner.Camera.Scale);

            Rectangle textureSource = IsGripping ? ClosedClawSource : OpenClawSource;
            Owner.SpriteBatch.Draw(Owner.TileMapTexture,position,textureSource,Color.White,rotation,origin,scale,SpriteEffects.None,0.8f);

            float distance = (position.Y - origin.Y * scale.Y) / scale.Y / RopeSource.Height;
            origin = new Vector2(16,0);

            scale.Y *= distance;

            if(scale.Y <= 0) {
                return;
            }

            position.Y = 0;

            Owner.SpriteBatch.Draw(Owner.TileMapTexture,position,RopeSource,Color.White,rotation,origin,scale,SpriteEffects.None,0.8f);
        }

        private Vector2 _position;

        protected override Vector2 GetPosition() {
            return _position;
        }

        protected override void SetPosition(Vector2 position) {
            _position = position;
        }
    }
}
