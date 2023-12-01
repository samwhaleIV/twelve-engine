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

        public WalkingJohn() : base(new Vector2(16/16f)) {
            OnRender += WalkingJohn_OnRender;
            OnUpdate += WalkingJohn_OnUpdate;
        }

        private void WalkingJohn_OnUpdate() {
            Body.ApplyLinearImpulse(new Vector2(0.01f,0));
        }

        private Rectangle GetTextureSource() {
            return new Rectangle(0,16,16,16);
        }

        private void WalkingJohn_OnRender() {
            Vector2 position = Owner.Camera.GetRenderLocation(this);

            float rotation = 0f;
            float layerDepth = 0.5f;

            Vector2 scale = new Vector2(Owner.Camera.Scale);

            Rectangle textureSource = GetTextureSource();
            SpriteEffects spriteEffects = SpriteEffects.None;

            Vector2 origin = textureSource.Size.ToVector2() * new Vector2(0.5f);

            Owner.SpriteBatch.Draw(Owner.TileMapTexture,position,textureSource,Color.White,rotation,origin,scale,spriteEffects,layerDepth);
        }
    }
}
