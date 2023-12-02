using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;
using Microsoft.Xna.Framework.Graphics;

namespace John {
    public sealed class WalkingJohn:PhysicsEntity2D {

        private readonly JohnDecorator _decorator;

        public WalkingJohn(JohnDecorator johnDecorator) : base(new Vector2(14/16f),new Vector2(0.5f)) {
            _decorator = johnDecorator;
            OnRender += WalkingJohn_OnRender;
            OnUpdate += WalkingJohn_OnUpdate;
        }

        private void WalkingJohn_OnUpdate() {
            Body.ApplyLinearImpulse(new Vector2(0.01f,0));
        }

        public int PoolID { get; private set; } = -1;

        public void Enable(int poolID,Vector2 position) {
            PoolID = poolID;
            Position = position;
            Body.Enabled = true;
        }

        public void Disable() {
            Body.Enabled = false;
            Body.Position = new Vector2(-1);
            PoolID = -1;
        }

        public bool IsWalking => true;
        public bool FacingRight => true;

        public int ConfigID { get; set; } = -1;

        private static int GetAnimationFrameOffset(TimeSpan now) {
            int frameNumber = (int)Math.Floor(now / Constants.WALKING_ANIMATION_FRAME_LENGTH);

            /* Staggered animation pattern: { 0, 1, 0, 2 } */
            return frameNumber % Constants.WALKING_ANIMATION_FRAME_COUNT switch { 0 => 0, 1 => 1, 2 => 0, 3 => 2, _ => 0 };
        }

        private TimeSpan _walkingStart = TimeSpan.Zero; //TODO: Set to time when walking starts

        public Rectangle GetTextureSource() {
            Point location = _decorator.GetTextureOrigin(ConfigID);
            Point size = (Size * Owner.Camera.TileInputSize).ToPoint();

            if(IsWalking) {
                location.X += GetAnimationFrameOffset(Owner.Now - _walkingStart) * size.X;
            }

            return new Rectangle(location,size);
        }

        public SpriteEffects GetSpriteEffects() {
            return FacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        }

        private void WalkingJohn_OnRender() {
            Vector2 position = Owner.Camera.GetRenderLocation(this);

            Vector2 scale = new Vector2(Owner.Camera.Scale);

            Rectangle textureSource = GetTextureSource();
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 origin = textureSource.Size.ToVector2() * Origin;

            Owner.SpriteBatch.Draw(_decorator.Texture,position,textureSource,Color.White,0f,origin,scale,spriteEffects,LayerDepth);
        }
    }
}
