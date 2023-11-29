using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.EntitySystem;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game2D {
    public sealed class Camera {

        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 PositionOffset { get; set; } = new Vector2(-0.5f);
        public Vector2 UnboundPositionOffset { get; set; } = Vector2.Zero;

        public float MinX { get; set; } = float.NegativeInfinity;
        public float MinY { get; set; } = float.NegativeInfinity;

        public float MaxX { get; set; } = float.PositiveInfinity;
        public float MaxY { get; set; } = float.PositiveInfinity;

        public float TileSize { get; private set; } = 0f;
        public float TileInputSize { get; set; } = 1f;
        public float Scale { get; set; } = 10f;

        public Vector2 ScreenSize { get; private set; }
        public Vector2 ScreenCenter { get; private set; }

        public Vector2 TileOrigin { get; private set; }
        public Point TopLeftTileCoordinate { get; private set; }

        private Vector2 _constraintOffset;

        private void UpdateConstraintOffset() {
            _constraintOffset = Vector2.Zero;

            Vector2 screenCenter = Position - PositionOffset;

            Vector2 topLeft = screenCenter - ScreenCenter / TileSize;
            Vector2 bottomRight = topLeft + ScreenSize / TileSize;

            if(topLeft.X < MinX) {
                _constraintOffset.X += MinX - topLeft.X;
            } else if(bottomRight.X > MaxX) {
                _constraintOffset.X += MaxX - bottomRight.X;
            }
            if(topLeft.Y < MinY) {
                _constraintOffset.Y += MinY - topLeft.Y;
            } else if(bottomRight.Y > MaxY) {
                _constraintOffset.Y += MaxY - bottomRight.Y;
            }

            return;
        }

        internal void Update(Viewport viewport) {
            TileSize = TileInputSize * Scale;

            Vector2 screenSize = viewport.Bounds.Size.ToVector2();
            if(screenSize.X % 2 == 1) screenSize.X += 1;
            if(screenSize.Y % 2 == 1) screenSize.Y += 1;

            ScreenSize = screenSize;
            ScreenCenter = screenSize * 0.5f;

            UpdateConstraintOffset();

            Vector2 tileOffset = (Position + PositionOffset + _constraintOffset) * TileSize;
            tileOffset.X %= TileSize; tileOffset.Y %= TileSize;

            tileOffset = Vector2.Round(tileOffset);

            Vector2 leftSideTileCount = Vector2.Ceiling((ScreenCenter - tileOffset) / TileSize);
            TileOrigin = ScreenCenter - leftSideTileCount * TileSize;

            TileOrigin -= tileOffset;

            Vector2 topLeftTileCoordinate = Position - PositionOffset - leftSideTileCount + _constraintOffset;
            TopLeftTileCoordinate = Vector2.Floor(topLeftTileCoordinate).ToPoint();
        }

        public Vector2 GetRenderLocation(Entity2D entity) {
            return ScreenCenter - (entity.Position - (Position + PositionOffset - _constraintOffset)) * TileSize;
        }
    }
}
