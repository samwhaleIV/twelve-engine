namespace TwelveEngine.Game2D.Entities {
    public sealed class TestPlayer:PhysicsEntity2D {

        private readonly Texture2D _texture;
        private readonly TileData _tileData;

        public TestPlayer(Texture2D texture,TileData tileData) : base(new Vector2(1,1),Vector2.Zero) {
            _texture = texture;
            _tileData = tileData;
            OnUpdate += Update;
            OnRender += Render;
        }

        public float ImpulseForce { get; set; } = 1f;

        private bool _wasDown = false;

        private void Update() {
            Body.Enabled = false;
            var delta = Owner.ImpulseHandler.GetDigitalDelta2DWithModifier();
            delta *= 1;
            if(_wasDown && delta == Vector2.Zero) {
                _wasDown = false;
                return;
            }
            if(_wasDown && !Owner.ImpulseHandler.IsImpulseDown(Input.Impulse.Focus)) {
                return;
            }
            if(delta == Vector2.Zero) {
                return;
            }
            var change = delta * (1 / Owner.Camera.TileSize);
            if(!float.IsNaN(change.X) && !float.IsNaN(change.Y)) {
                Position += change;
            }
            _wasDown = true;
        }

        private void Render() {
            if(!Owner.Camera.TryGetRenderLocation(this,out var position)) {
                return;
            }
            Vector2 scale = new Vector2(Owner.Camera.Scale);
            Owner.SpriteBatch.Draw(_texture,position,_tileData.Source,_tileData.Color,0f,Vector2.Zero,scale,_tileData.SpriteEffects,LayerDepth);
        }
    }
}
