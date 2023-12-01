using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace John {
    public sealed class GrabbyClaw:Entity2D {


        public GrabbyClaw() {
            OnRender += GrabbyClaw_OnRender;
            OnUpdate += GrabbyClaw_OnUpdate;
            OnLoad +=GrabbyClaw_OnLoad;
            OnUnload +=GrabbyClaw_OnUnload;
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

        public bool IsGripping { get; set; }

        private void ToggleGrip() {
            IsGripping = !IsGripping;
        }

        private void GrabbyClaw_OnUpdate() {
            var delta = Owner.Impulse.GetDelta2D();

            _position += delta * 0.01f;

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
