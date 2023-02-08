namespace TwelveEngine.UI.Book {

    public class BookElement:InteractionElement<BookElement> {

        private readonly Interpolator animator;
        public BookElement() => animator = new(Book<BookElement>.DefaultAnimationDuration);

        public ComputedArea ComputedArea { get; private set; } = ComputedArea.Empty;

        private ElementLayoutData? oldLayout = null;
        private ElementLayoutData layout = new();

        public CoordinateMode SizeModeX { get; set; }
        public CoordinateMode SizeModeY { get; set; }

        public CoordinateMode PositionModeX { get; set; }
        public CoordinateMode PositionModeY { get; set; }

        public CoordinateMode SizeMode { set { SizeModeX = value; SizeModeY = value; } }

        public CoordinateMode PositionMode { set { PositionModeX = value; PositionModeY = value; } }

        public Vector2 Size { get => GetSize(); set => SetSize(value); }
        public Vector2 Position { get => GetPosition(); set => SetPosition(value); }

        /// <summary>
        /// Offset is multiplied by the computed layout size then added to the computed position.
        /// </summary>
        public Vector2 Offset { get => layout.Offset; set => layout.Offset = value; }

        public void SetSize(Vector2 size) {
            if(SizeModeX == CoordinateMode.Absolute) size.X /= Viewport.Width;
            if(SizeModeY == CoordinateMode.Absolute) size.Y /= Viewport.Height;
            layout.Size = size;
        }

        public void SetPosition(Vector2 position) {
            if(PositionModeX == CoordinateMode.Absolute) position.X /= Viewport.Width;
            if(PositionModeY == CoordinateMode.Absolute) position.Y /= Viewport.Height;
            layout.Position = position;
        }

        public Vector2 GetSize() {
            var size = layout.Size;
            if(SizeModeX == CoordinateMode.Absolute) size.X *= Viewport.Width;
            if(SizeModeY == CoordinateMode.Absolute) size.Y *= Viewport.Height;
            return size;
        }

        public Vector2 GetPosition() {
            var position = layout.Position;
            if(PositionModeX == CoordinateMode.Absolute) position.X *= Viewport.Width;
            if(PositionModeY == CoordinateMode.Absolute) position.Y *= Viewport.Height;
            return position;
        }

        /// <summary>
        /// Element scale, multipled by the computed size.
        /// </summary>
        public float Scale { get => layout.Scale; set => layout.Scale = value; }

        /// <summary>
        /// Rotation attribute. Uses a renderer specific implementation, but assume a center, relative origin. Not realted to <c>Offset</c>.
        /// </summary>
        public float Rotation { get => layout.Rotation; set => layout.Rotation = value; }

        public bool SmoothStep { get; set; } = false;

        public ComputedArea GetStaticComputedArea() => GetComputedArea(layout);
        public TimeSpan DefaultAnimationDuration { get; set; } = Book<BookElement>.DefaultAnimationDuration;


        private ComputedArea GetComputedArea(ElementLayoutData layout) {
            Vector2 size = layout.Size * Viewport.Size;

            size *= layout.Scale;

            Vector2 position = layout.Position * Viewport.Size;

            /* If viewport is fullscreen, viewport.Position should be (0,0) */
            position += size * layout.Offset + Viewport.Position;

            return new(new(position,size),layout.Rotation);
        }

        /// <summary>
        /// Call before changing element layout data to animate the changes.
        /// </summary>
        /// <param name="now">Current, total elapsed time.</param>
        public void KeyAnimation(TimeSpan now,TimeSpan? overrideAnimationDuration = null) {
            if(!AnimationKeyingEnabled) {
                return;
            }
            if(oldLayout is not null) {
                oldLayout = ElementLayoutData.Interpolate(oldLayout.Value,layout,animator.Value,SmoothStep);
            } else {
                oldLayout = layout;
            }
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

        internal void DisableKeying() => AnimationKeyingEnabled = false;
        internal void EnableKeying() => AnimationKeyingEnabled = true;

        /// <summary>
        /// Skip the remainder of the current animation. Useful if you need to immediately hide an element or position it off-screen.
        /// </summary>
        public void SkipAnimation() {
            animator.Finish();
        }

        protected FloatRectangle Viewport { get; private set; }

        internal void SetViewport(FloatRectangle viewport) {
            Viewport = viewport;
        }

        /// <summary>
        /// Update animation interpolation and calculate <see cref="ComputedArea"/>.
        /// </summary>
        /// <param name="now">Current, total elapsed time.</param>
        public void Update(TimeSpan now) {
            animator.Update(now);
            if(InputIsPaused && animator.IsFinished) {
                InputIsPaused = false;
                KeyAnimation(now);
            }
            ElementLayoutData layout;
            if(oldLayout is not null) {
                layout = ElementLayoutData.Interpolate(oldLayout.Value,this.layout,animator.Value,SmoothStep);
            } else {
                layout = this.layout;
            }
            if(CanUpdate && !InputIsPaused) {
                OnUpdate?.Invoke(now);
            }
            ComputedArea = GetComputedArea(layout);
        }

        public void PauseInputForAnimation() => InputIsPaused = true;
        public override FloatRectangle GetScreenArea() => ComputedArea.Destination;

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
