namespace TwelveEngine.Game2D.Entities {
    public sealed class Player:PhysicsEntity2D {

        private readonly Texture2D _texture;
        private readonly TileData _tileData;

        public Player(Texture2D texture,TileData tileData) : base(new Vector2(1,1),Vector2.Zero) {
            _texture = texture;
            _tileData = tileData;

            OnUpdate += ApplyInput;
            OnRender += RenderSprite;
        }

        public float ImpulseForce { get; set; } = 1f;

        private void ApplyInput() {
            var delta = Owner.Impulse.GetDelta2D(allowSpeedModifiers: false) * ImpulseForce;
            Body.ApplyLinearImpulse(delta);
        }

        //private bool wasDown = false;
        //private void ApplyInput() {
        //    Body.Enabled = false;
        //    var delta = Owner.Impulse.GetDelta2D(allowSpeedModifiers: true);
        //    if(wasDown && delta == Vector2.Zero) {
        //        wasDown = false;
        //        return;
        //    }
        //    if(wasDown && !Owner.Impulse.IsImpulseDown(Input.Impulse.Focus)) {
        //        return;
        //    }
        //    if(delta == Vector2.Zero) {
        //        return;
        //    }
        //    var change = delta * (1 / Owner.Camera.TileSize);
        //    if(!float.IsNaN(change.X) && !float.IsNaN(change.Y)) {
        //        Position += change;
        //    }
        //    wasDown = true;
        //}

        private void RenderSprite() {
            Vector2 position = Owner.Camera.GetRenderLocation(this);

            float rotation = 0f; Vector2 origin = Vector2.Zero;

            Vector2 scale = new Vector2(Owner.Camera.Scale);

            Owner.SpriteBatch.Draw(_texture,position,_tileData.Source,_tileData.Color,rotation,origin,scale,_tileData.SpriteEffects,LayerDepth);
        }
    }
}
