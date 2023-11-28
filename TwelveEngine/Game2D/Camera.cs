using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game2D {
    public sealed class Camera {

        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 PositionOffset { get; set; } = new Vector2(-0.5f);
        public Vector2 UnboundPositionOffset { get; set; } = Vector2.Zero;

        public float MinX { get; set; } = float.NegativeInfinity;
        public float MinY { get; set; } = float.NegativeInfinity;

        public float MaxX { get; set; } = float.PositiveInfinity;
        public float MaxY { get; set; } = float.PositiveInfinity;

        public float TileRenderSize => TileInputSize * Scale;
        public float TileInputSize { get; set; } = 4f;
        public float Scale { get; set; } = 10f;

        private Vector2 _physicalScreenCenter, _outputScreenCenter;

        internal void Update(Viewport viewport) {
            Rectangle bounds = viewport.Bounds;
            _physicalScreenCenter = bounds.Center.ToVector2();
            Vector2 _physicalScreenSize = bounds.Size.ToVector2();

            Vector2 size = _physicalScreenSize / TileRenderSize;

            Vector2 topLeft = Position + PositionOffset - size * 0.5f;
            Vector2 bottomRight = topLeft + size;

            Vector2 marginLimitOffset = Vector2.Zero;

            if(topLeft.X < MinX) {
                marginLimitOffset.X += MinX - topLeft.X;
            } else if(bottomRight.X > MaxX) {
                marginLimitOffset.X -= MaxX - bottomRight.X;
            }
            if(topLeft.Y < MinY) {
                marginLimitOffset.Y += MinY - topLeft.Y;
            } else if(bottomRight.Y > MaxY) {
                marginLimitOffset.Y -= MaxY - bottomRight.Y;
            }

            TileBounds = new FloatRectangle(topLeft + marginLimitOffset,size);
            _outputScreenCenter = Position + PositionOffset - marginLimitOffset + UnboundPositionOffset;
        }

        public FloatRectangle TileBounds { get; private set; }

        public Vector2 GetRenderLocation(Entity2D entity) {
            Vector2 entityPosition = _outputScreenCenter + UnboundPositionOffset - entity.Position;
            return _physicalScreenCenter + entityPosition * TileRenderSize;
        }
    }
}
