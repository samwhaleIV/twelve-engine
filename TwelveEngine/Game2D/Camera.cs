using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.EntitySystem;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Game2D {
    public sealed class Camera {

        public Vector2 Position { get; set; } = Vector2.Zero;
        public Vector2 PositionOffset { get; set; } = new Vector2(-0.5f);

        public float MinX { get; set; } = float.NegativeInfinity;
        public float MinY { get; set; } = float.NegativeInfinity;

        public float MaxX { get; set; } = float.PositiveInfinity;
        public float MaxY { get; set; } = float.PositiveInfinity;

        public Vector2 TileSize { get; private set; } = Vector2.One;
        public float TileInputSize { get; set; } = 1f;
        public float Scale { get; set; } = 10f;

        public Vector2 ScreenSize { get; private set; }
        public Vector2 ScreenCenter { get; private set; }

        public Vector2 RenderOrigin { get; private set; }
        public Point TileStart { get; private set; }

        public Point TileStride { get; private set; }

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
            TileSize = new Vector2(TileInputSize * Scale);

            Vector2 screenSize = viewport.Bounds.Size.ToVector2();
            if(screenSize.X % 2 == 1) screenSize.X += 1;
            if(screenSize.Y % 2 == 1) screenSize.Y += 1;

            ScreenSize = screenSize;
            ScreenCenter = screenSize * 0.5f;

            UpdateConstraintOffset();

            Vector2 tileOffset = (Position + PositionOffset + _constraintOffset) * TileSize;
            tileOffset.X %= TileSize.X; tileOffset.Y %= TileSize.Y;

            tileOffset = Vector2.Round(tileOffset);

            Vector2 leftSideTileCount = Vector2.Ceiling((ScreenCenter - tileOffset) / TileSize);
            RenderOrigin = ScreenCenter - leftSideTileCount * TileSize;

            RenderOrigin -= tileOffset;

            Vector2 topLeftTileCoordinate = Position - PositionOffset - leftSideTileCount + _constraintOffset;
            TileStart = Vector2.Floor(topLeftTileCoordinate).ToPoint();

            TileStride = Vector2.Ceiling((ScreenSize - RenderOrigin) / TileSize).ToPoint();
        }

        public Vector2 GetRenderLocation(Entity2D entity) {
            return ScreenCenter - (entity.Position - (Position + PositionOffset - _constraintOffset)) * TileSize;
        }
    }
}
