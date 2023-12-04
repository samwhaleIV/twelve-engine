using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwelveEngine.Game2D;
using TwelveEngine.Input;
using nkast.Aether.Physics2D.Dynamics;

namespace John {
    public sealed class GrabbyClaw:Entity2D {

        private readonly JohnCollectionGame _game;

        public GrabbyClaw(JohnCollectionGame game) {
            _game = game;
            OnRender += GrabbyClaw_OnRender;
            OnUpdate += GrabbyClaw_OnUpdate;
            OnLoad += GrabbyClaw_OnLoad;
            OnUnload += GrabbyClaw_OnUnload;
        }

        private void GrabbyClaw_OnUnload() {
            Owner.Impulse.OnEvent -= Impulse_OnEvent;
        }

        private void GrabbyClaw_OnLoad() {
            Owner.Impulse.OnEvent += Impulse_OnEvent;
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
            var fixture = _game.TestPoint(_position - new Vector2(0.5f,0.5f));
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

        private void GrabbyClaw_OnUpdate() {
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

        private readonly Rectangle ClosedClawSource = new Rectangle(80,16,32,32);
        private readonly Rectangle OpenClawSource = new Rectangle(112,16,32,32);
        private readonly Rectangle RopeSource = new Rectangle(80,0,32,16);

        private void GrabbyClaw_OnRender() {
            Vector2 position = Owner.Camera.GetRenderLocation(this);

            float rotation = 0f; Vector2 origin = new Vector2(16,30);

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
