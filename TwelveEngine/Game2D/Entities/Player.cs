using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Entities {
    public sealed class Player:PhysicsEntity2D {

        private readonly Texture2D _texture;
        private readonly Rectangle _textureSource;
        private readonly Color _color;

        public Player(Texture2D texture,Rectangle textureSource,Color color) : base(new Vector2(1,1)) {
            _texture = texture;
            _textureSource = textureSource;
            _color = color;
            OnUpdate += ApplyInput;
            OnRender += RenderSprite;
            Fixture.Friction = 1f;
            PhysicsBody.LinearDamping = 3f;
            PhysicsBody.Mass = 10f;
        }

        private void ApplyInput() {
            var delta = Owner.Impulse.GetDelta2D() * 2f;
            PhysicsBody.ApplyLinearImpulse(delta);
        }

        private void RenderSprite() {
            Rectangle sourceRectangle = _textureSource;

            Vector2 position = Owner.Camera.GetRenderLocation(this);

            float rotation = 0f;
            Vector2 origin = Vector2.Zero;

            SpriteEffects effects = SpriteEffects.None;
            float layerDepth = 0.5f;

            Vector2 scale = new Vector2(Owner.Camera.Scale);
            Owner.SpriteBatch.Draw(_texture,position,sourceRectangle,_color,rotation,origin,scale,effects,layerDepth);
        }
    }
}
