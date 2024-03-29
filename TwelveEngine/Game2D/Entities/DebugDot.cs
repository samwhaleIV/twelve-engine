﻿namespace TwelveEngine.Game2D.Entities {
    public sealed class DebugDot:Entity2D {

        public DebugDot() {
            LayerDepth = 1f;
            OnRender += Render;
        }

        private Vector2 _position;

        protected override Vector2 GetPosition() {
            return _position;
        }

        protected override void SetPosition(Vector2 position) {
            _position = position;
        }

        private void Render() {
            Vector2 position = Owner.Camera.GetRenderLocation(this);
            float rotation = 0f; Vector2 origin = Vector2.Zero;
            Vector2 scale = new Vector2(1);
            Rectangle source = new Rectangle(0,0,16,16);
            position -= source.Size.ToVector2() * 0.5f * scale;
            Owner.SpriteBatch.Draw(Owner.TileMapTexture,position,source,Color.White,rotation,origin,scale,SpriteEffects.None,LayerDepth);
        }
    }
}
