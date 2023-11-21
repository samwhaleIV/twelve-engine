using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D {
    public sealed class Camera {
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 Offset { get; set; } = new Vector2(-0.5f);

        public float OutputScale { get; set; } = 50f;
        public float InputScale { get; set; } = 4f;

        private Vector2 _screenSize, _screenCenter;

        public Vector2 SpriteScale { get; private set; } = Vector2.Zero;

        internal void Update(Viewport viewport) {
            Rectangle bounds = viewport.Bounds;
            _screenCenter = bounds.Center.ToVector2();
            _screenSize = bounds.Size.ToVector2();
            SpriteScale = new Vector2(OutputScale / InputScale);
        }

        public Vector2 GetScreenPosition(Entity2D entity) {
            Vector2 entityPosition = Position + Offset - entity.Position;
            return _screenCenter + entityPosition * OutputScale;
        }
    }
}
