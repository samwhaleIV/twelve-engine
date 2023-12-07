namespace TwelveEngine.Game2D {
    public sealed class Camera {

        public Vector2 Position { get; set; } = Vector2.Zero;

        public Vector2 MinBound { get; set; } = new(float.NegativeInfinity);
        public Vector2 MaxBound { get; set; } = new(float.PositiveInfinity);

        public float MinX {
            get {
                return MinBound.X;
            }
            set {
                var minBound = MinBound;
                minBound.X = value;
                MinBound = minBound;
            }
        }
        public float MinY {
            get {
                return MinBound.Y;
            }
            set {
                var minBound = MinBound;
                minBound.Y = value;
                MinBound = minBound;
            }
        }

        public float MaxX {
            get {
                return MaxBound.X;
            }
            set {
                var maxBound = MaxBound;
                maxBound.X = value;
                MaxBound = maxBound;
            }
        }

        public float MaxY {
            get {
                return MaxBound.Y;
            }
            set {
                var maxBound = MaxBound;
                maxBound.Y = value;
                MaxBound = maxBound;
            }
        }

        /// <summary>
        /// The camera needs to know the physical bounds of the world to calculate <see cref="_constraintOffset"/>.
        /// </summary>
        public Vector2 MaxSize { get; set; } = new(float.PositiveInfinity);

        public float TileSize { get; private set; } = 1f;
        public int TileInputSize { get; set; } = 1;
        public float Scale { get; set; } = 10f;

        public Vector2 ScreenSize { get; private set; }
        public Vector2 ScreenCenter { get; private set; }

        private Rectangle ScreenArea { get; set; }

        public Vector2 RenderOrigin { get; private set; }
        public Point TileStart { get; private set; }

        public Point TileStride { get; private set; }

        private Vector2 _constraintOffset;

        private void UpdateConstraintOffset(bool allowX,bool allowY) {
            _constraintOffset = Vector2.Zero;
            Vector2 screenCenter = Position;

            Vector2 topLeft = screenCenter - ScreenCenter / TileSize;
            Vector2 bottomRight = topLeft + ScreenSize / TileSize;

            if(allowX) {
                if(topLeft.X < MinX) {
                    _constraintOffset.X += MinX - topLeft.X;
                } else if(bottomRight.X > MaxX) {
                    _constraintOffset.X += MaxX - bottomRight.X;
                }
            }

            if(allowY) {
                if(topLeft.Y < MinY) {
                    _constraintOffset.Y += MinY - topLeft.Y;
                } else if(bottomRight.Y > MaxY) {
                    _constraintOffset.Y += MaxY - bottomRight.Y;
                }
            }
        }

        internal void Update(Viewport viewport) {
            Update(viewport,true,true);
        }

        private void Update(Viewport viewport,bool allowConstraintX,bool allowConstraintY) {
            TileSize = TileInputSize * Scale;

            ScreenArea = viewport.Bounds;
            Vector2 screenSize = ScreenArea.Size.ToVector2();
            if(screenSize.X % 2 == 1) screenSize.X += 1;
            if(screenSize.Y % 2 == 1) screenSize.Y += 1;

            ScreenSize = screenSize;
            ScreenCenter = screenSize * 0.5f;

            UpdateConstraintOffset(allowConstraintX,allowConstraintY);

            Vector2 cameraPosition = Position + _constraintOffset;

            Vector2 tileOffset = cameraPosition;
            tileOffset.X %= 1; tileOffset.Y %= 1;

            tileOffset = Vector2.Round(tileOffset * TileSize);

            Vector2 leftSideTileCount = Vector2.Ceiling((ScreenCenter - tileOffset) / TileSize);
            RenderOrigin = ScreenCenter - leftSideTileCount * TileSize;

            RenderOrigin -= tileOffset;

            Vector2 topLeftTileCoordinate = cameraPosition - leftSideTileCount;

            Vector2 tileStart = topLeftTileCoordinate;

            if(cameraPosition.X > 0) {
                tileStart.X = MathF.Floor(tileStart.X);
            } else {
                tileStart.X = MathF.Ceiling(tileStart.X);
            }

            if(cameraPosition.Y > 0) {
                tileStart.Y = MathF.Floor(tileStart.Y);
            } else {
                tileStart.Y = MathF.Ceiling(tileStart.Y);
            }

            TileStart = tileStart.ToPoint();

            TileStride = Vector2.Ceiling((ScreenSize - RenderOrigin) / TileSize).ToPoint();

            if(!allowConstraintX || !allowConstraintY) {
                return;
            }
            /* This is so fucking stupid. */
            var limitPosition = GetRenderLocation(MaxSize);
            if(limitPosition.X < screenSize.X) {
                allowConstraintX = false;
            }
            if(limitPosition.Y < screenSize.Y) {
                allowConstraintY = false;
            }
            if(allowConstraintX && allowConstraintY) {
                return;
            }
            /* I guess you could say this is a weird edge case, but it's still stupid. */
            Update(viewport,allowConstraintX,allowConstraintY);
        }

        public Vector2 GetRenderLocation(Entity2D entity) {
            Vector2 renderLocation = RenderOrigin + (entity.Position - TileStart.ToVector2()) * TileSize;
            renderLocation.Round();
            return renderLocation;
        }

        /// <summary>
        /// Get render location filtered with object offscreen detection.
        /// </summary>
        /// <param name="entity">The entity being rendered.</param>
        /// <param name="textureSourceSize">The texture source size of the rendered entity.</param>
        /// <param name="renderLocation">The top left corner of the object being rendered.</param>
        /// <returns>A <see cref="bool"/> indicating whether the entity is offscreen.</returns>
        public bool TryGetRenderLocation(Entity2D entity,out Vector2 renderLocation) {
            renderLocation = GetRenderLocation(entity.Position - entity.Origin * entity.Size);
            Vector2 size = Vector2.Floor(entity.Size * TileSize);
            var renderArea = new Rectangle(renderLocation.ToPoint(),size.ToPoint());
            return renderArea.Intersects(ScreenArea);
        }

        public Vector2 GetRenderLocation(Vector2 point) {
            Vector2 renderLocation = RenderOrigin + (point - TileStart.ToVector2()) * TileSize;
            renderLocation.Round();
            return renderLocation;
        }
    }
}
