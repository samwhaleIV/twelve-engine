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

        public Vector2 TileOrigin { get; private set; }
        public Point TopLeftTileCoordinate { get; private set; }

        public Vector2 ScreenCenter { get; private set; }

        private Vector2 _constraintPosition;

        private void UpdateConstraintPosition() {
            _constraintPosition = Position + PositionOffset;
        }

        internal void Update(Viewport viewport) {
            UpdateConstraintPosition();
            
            TileSize = TileInputSize * Scale;

            Rectangle bounds = viewport.Bounds;

            Vector2 screenSize = bounds.Size.ToVector2();
            ScreenSize = screenSize;

            if(screenSize.X % 2 == 1) screenSize.X += 1;
            if(screenSize.Y % 2 == 1) screenSize.Y += 1;

            ScreenCenter = screenSize * 0.5f;

            Vector2 leftSideTileCount = Vector2.Ceiling(ScreenCenter / TileSize);
            TileOrigin = ScreenCenter - leftSideTileCount * TileSize;

            Vector2 tileOffset = _constraintPosition * TileSize;
            tileOffset.X %= TileSize; tileOffset.Y %= TileSize;
            TileOrigin -= Vector2.Round(tileOffset);

            Vector2 topLeftTileCoordinate = Position - PositionOffset - leftSideTileCount;
            TopLeftTileCoordinate = Vector2.Floor(topLeftTileCoordinate).ToPoint();
        }

        public Vector2 GetRenderLocation(Entity2D entity) {
            return ScreenCenter - (entity.Position - _constraintPosition) * TileSize;
        }
    }
}
