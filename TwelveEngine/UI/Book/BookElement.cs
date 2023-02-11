namespace TwelveEngine.UI.Book {

    public class BookElement:InteractionElement<BookElement> {

        private readonly Interpolator animator;
        public BookElement() => animator = new(Book<BookElement>.DefaultAnimationDuration);

        private ComputedPropertySet ComputedPropertySet { get; set; } = ComputedPropertySet.Empty;

        private ElementLayoutData oldLayout = ElementLayoutData.Default;
        private ElementLayoutData layout = ElementLayoutData.Default;

        public CoordinateMode SizeModeX { get; set; } = CoordinateMode.Absolute;
        public CoordinateMode SizeModeY { get; set; } = CoordinateMode.Absolute;

        public CoordinateMode PositionModeX { get; set; } = CoordinateMode.Absolute;
        public CoordinateMode PositionModeY { get; set; } = CoordinateMode.Absolute;

        public CoordinateMode SizeMode { set { SizeModeX = value; SizeModeY = value; } }
        public CoordinateMode PositionMode { set { PositionModeX = value; PositionModeY = value; } }

        public Vector2 Size { get => GetSize(); set => SetSize(value); }
        public Vector2 Position { get => GetPosition(); set => SetPosition(value); }

        /// <summary>
        /// Offset is multiplied by the computed layout size then added to the computed position.
        /// </summary>
        public Vector2 Offset { get => layout.Offset; set => layout.Offset = value; }

        public Color Color { get => layout.Color; set => layout.Color = value; }

        public Color ComputedColor => ComputedPropertySet.Color;
        public FloatRectangle ComputedArea => ComputedPropertySet.Destination;
        public float ComputedRotation => ComputedPropertySet.Rotation;

        /// <summary>
        /// Element scale, multipled by the computed size.
        /// </summary>
        public float Scale { get => layout.Scale; set => layout.Scale = value; }

        /// <summary>
        /// Rotation attribute. Uses a renderer specific implementation, but assume a center, relative origin. Not realted to <c>Offset</c>.
        /// </summary>
        public float Rotation { get => layout.Rotation; set => layout.Rotation = value; }

        public bool SmoothStepAnimation { get; set; } = false;

        public void SetSize(Vector2 size) {
            if(SizeModeX == CoordinateMode.Absolute) {
                size.X = Viewport.Width == 0 ? 0 : size.X / Viewport.Width;
            }
            if(SizeModeY == CoordinateMode.Absolute) {
                size.Y = Viewport.Height == 0 ? 0 : size.Y / Viewport.Height;
            }
            layout.Size = size;
        }

        public void SetPosition(Vector2 position) {
            if(PositionModeX == CoordinateMode.Absolute) {
                position.X = Viewport.Width == 0 ? 0 : position.X / Viewport.Width;
            }
            if(PositionModeY == CoordinateMode.Absolute) {
                position.Y = Viewport.Height == 0 ? 0 : position.Y / Viewport.Height;
            }
            layout.Position = position;
        }

        public Vector2 GetSize() {
            var size = layout.Size;
            if(SizeModeX == CoordinateMode.Absolute) {
                size.X *= Viewport.Width;
            }
            if(SizeModeY == CoordinateMode.Absolute) {
                size.Y *= Viewport.Height;
            }
            return size;
        }

        public Vector2 GetPosition() {
            var position = layout.Position;
            if(PositionModeX == CoordinateMode.Absolute) {
                position.X *= Viewport.Width;
            }
            if(PositionModeY == CoordinateMode.Absolute) {
                position.Y *= Viewport.Height;
            }
            return position;
        }

        public FloatRectangle GetStaticComputedArea() => GetComputedArea(layout).Destination;
        public TimeSpan DefaultAnimationDuration { get; set; } = Book<BookElement>.DefaultAnimationDuration;

        private ComputedPropertySet GetComputedArea(ElementLayoutData layout) {
            Vector2 size = layout.Size * Viewport.Size;

            size *= layout.Scale;

            Vector2 position = layout.Position * Viewport.Size;

            /* If viewport is fullscreen, viewport.Position should be (0,0) */
            position += size * layout.Offset + Viewport.Position;

            return new(new(position,size),layout.Rotation,layout.Color);
        }

        /// <summary>
        /// Call before changing element layout data to animate the changes.
        /// </summary>
        /// <param name="now">Current, total elapsed time.</param>
        public void KeyFrame(TimeSpan now,TimeSpan? overrideAnimationDuration = null) {
            if(!AnimationKeyingEnabled) {
                return;
            }
            oldLayout = ElementLayoutData.Interpolate(oldLayout,layout,animator.Value,SmoothStepAnimation);
            TimeSpan duration;
            if(overrideAnimationDuration.HasValue) {
                duration = overrideAnimationDuration.Value;
            } else {
                duration = DefaultAnimationDuration;
            }
            animator.Duration = duration;
            animator.Reset(now);
        }

        public bool IsAnimating => !animator.IsFinished;

        private bool AnimationKeyingEnabled = true;

        public void DisableKeyFrames() => AnimationKeyingEnabled = false;
        public void EnableKeyFrames() => AnimationKeyingEnabled = true;

        /// <summary>
        /// Skip the remainder of the current animation. Useful if you need to immediately hide an element or position it off-screen.
        /// </summary>
        public void SkipAnimation() {
            animator.Finish();
            oldLayout = layout;
        }

        protected FloatRectangle Viewport { get; private set; }

        internal void SetViewport(FloatRectangle viewport) => Viewport = viewport;

        private void UpdateComputedArea(TimeSpan now) {
            ElementLayoutData layout = ElementLayoutData.Interpolate(oldLayout,this.layout,animator.Value,SmoothStepAnimation);
            if(CanUpdate && !InputIsPaused) {
                OnUpdate?.Invoke(now);
            }
            ComputedPropertySet = GetComputedArea(layout);
        }

        /// <summary>
        /// Update animation interpolation and calculate <see cref="ComputedPropertySet"/>.
        /// </summary>
        /// <param name="now">Current, total elapsed time.</param>
        internal void UpdateAnimatedComputedArea(TimeSpan now) {
            animator.Update(now);
            if(InputIsPaused && animator.IsFinished) {
                InputIsPaused = false;
            }
            UpdateComputedArea(now);
        }

        public void PauseInputForAnimation() => InputIsPaused = true;
        public override FloatRectangle GetScreenArea() => ComputedPropertySet.Destination;

        public bool CanUpdate { get; set; } = false;
        protected internal event Action<TimeSpan> OnUpdate;

        public ElementFlags Flags {
            get {
                ElementFlags flags = ElementFlags.None;
                if(CanUpdate) {
                    flags |= ElementFlags.Update;
                }
                if(CanInteract) {
                    flags |= ElementFlags.Interact;
                }
                return flags;
            }
            set {
                CanUpdate = value.HasFlag(ElementFlags.Update);
                CanInteract = value.HasFlag(ElementFlags.Interact);
            }
        }
    }
}
