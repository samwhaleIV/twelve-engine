using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Entities {
    public sealed class TestSquare:Entity2D {

        private readonly Rectangle _textureSource;
        private readonly Color _color;
        private readonly Texture2D _texture;


        public TestSquare(Texture2D texture,Rectangle textureSource,Vector2 size,Color color) {     
            _texture = texture;
            _textureSource = textureSource;
            //OnUpdate += TestCube_OnUpdate;
            OnRender += TestCube_OnRender;
            _color = color;
        }

        private void TestCube_OnUpdate() {
            throw new NotImplementedException();
        }

        private void TestCube_OnRender() {
            Rectangle sourceRectangle = _textureSource;

            Vector2 position = Owner.Camera.GetRenderLocation(this);

            float rotation = 0f;
            Vector2 origin = Vector2.Zero;

            SpriteEffects effects = SpriteEffects.None;
            float layerDepth = 0.5f;

            Vector2 scale = new Vector2(Owner.Camera.Scale);
            Owner.SpriteBatch.Draw(_texture,position,sourceRectangle,_color,rotation,origin,scale,effects,layerDepth);
        }

        private Vector2 position;

        protected override Vector2 GetPosition() {
            return position;
        }

        protected override void SetPosition(Vector2 position) {
            this.position = position;
        }
    }
}
