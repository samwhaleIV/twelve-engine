using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D.Entities {
    public sealed class TestSquare:Entity2D {

        private readonly Rectangle _textureSource;
        private readonly Color _color;

        public TestSquare(Rectangle textureSource,Vector2 size,Color color) {      
            _textureSource = textureSource;
            //OnUpdate += TestCube_OnUpdate;
            OnRender += TestCube_OnRender;
            _color = color;
        }

        private void TestCube_OnUpdate() {
            throw new NotImplementedException();
        }

        private void TestCube_OnRender() {
            Texture2D texture = Owner.SpriteSheet;
            Rectangle sourceRectangle = _textureSource;

            Vector2 position = Owner.Camera.GetScreenPosition(this);

            float rotation = 0f;
            Vector2 origin = Vector2.Zero;

            SpriteEffects effects = SpriteEffects.None;
            float layerDepth = 0.5f;

            Vector2 scale = Owner.Camera.SpriteScale;

            Owner.SpriteBatch.Draw(texture,position,sourceRectangle,_color,rotation,origin,scale,effects,layerDepth);
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
