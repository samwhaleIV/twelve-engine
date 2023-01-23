using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine.UI {

    public class Element {

        private readonly AnimationInterpolator animator = new(Book<Element>.DefaultAnimationDuration);

        /// <summary>
        /// The computed area to be supplied to a renderer or mouse input system. Coordinates in floating point pixels.
        /// </summary>
        public ComputedArea ComputedArea { get; private set; } = ComputedArea.Empty;

        private ElementLayoutData? oldLayout = null;
        private ElementLayoutData layout = new();

        /// <summary>
        /// Coordinate mode for sizing the element on the X axis.
        /// </summary>
        public CoordinateMode SizeModeX { get; set; }

        /// <summary>
        /// Coordinate mode for sizing element on the Y axis;
        /// </summary>
        public CoordinateMode SizeModeY { get; set; }

        /// <summary>
        /// Coordinate mode for positioning element on the X axis.
        /// </summary>
        public CoordinateMode PositionModeX { get; set; }

        /// <summary>
        /// Coordinate mode for positioning element on the Y axis.
        /// </summary>
        public CoordinateMode PositionModeY { get; set; }

        /// <summary>
        /// Coordinate mode for sizing element, X and Y axis;
        /// </summary>
        public CoordinateMode SizeMode {
            set {
                SizeModeX = value;
                SizeModeY = value;
            }
        }

        /// <summary>
        /// Coordinate mode for positioning element, X and Y axis;
        /// </summary>
        public CoordinateMode PositionMode {
            set {
                PositionModeX = value;
                PositionModeY = value;
            }
        }

        /// <summary>
        /// Offset is multiplied by the computed layout size then added to the computed position.
        /// </summary>
        public Vector2 Offset { get => layout.Offset; set => layout.Offset = value; }

        /// <summary>
        /// Element position, later translated by <c>PositionMode</c>. Do not include viewport offset.
        /// </summary>
        public Vector2 Position { get => layout.Position; set => layout.Position = value; }

        /// <summary>
        /// Element size, later translated by <c>SizeMode</c>.
        /// </summary>
        public Vector2 Size { get => layout.Size; set => layout.Size = value; }

        /// <summary>
        /// Element scale, multipled by the computed size.
        /// </summary>
        public float Scale { get => layout.Scale; set => layout.Scale = value; }

        /// <summary>
        /// Rotation attribute. Uses a renderer specific implementation, but assume a center, relative origin. Not realted to <c>Offset</c>.
        /// </summary>
        public float Rotation { get => layout.Rotation; set => layout.Rotation = value; }

        public bool SmoothStep { get; set; } = false;

        private static float GetCoordinate(float value,float dimension,CoordinateMode mode) => mode switch {
            CoordinateMode.Absolute => value,
            CoordinateMode.Relative => value * dimension,
            _ => value
        };

        private void UpdateComputedArea(ElementLayoutData layout,VectorRectangle viewport) {
            Vector2 size = new(GetCoordinate(layout.Size.X,viewport.Width,SizeModeX),
                               GetCoordinate(layout.Size.Y,viewport.Height,SizeModeY));

            size *= layout.Scale;

            Vector2 position = new(GetCoordinate(layout.Position.X,viewport.Width,PositionModeX),
                                   GetCoordinate(layout.Position.Y,viewport.Height,PositionModeY));

            /* If viewport is fullscreen, viewport.Position should be (0,0) */
            position += size * layout.Offset + viewport.Position;

            ComputedArea = new(new(position,size),layout.Rotation);
        }

        public TimeSpan DefaultAnimationDuration { get; set; } = TimeSpan.FromSeconds(0.1f);


        /// <summary>
        /// Call before changing element layout data to animate the changes.
        /// </summary>
        /// <param name="now">Current, total elapsed time.</param>
        public void KeyAnimation(TimeSpan now,TimeSpan? overrideAnimationDuration = null) {
            if(keyAnimationLocked) {
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

        private bool keyAnimationLocked = false;

        internal void LockKeyAnimation() {
            keyAnimationLocked = true;
        }
        internal void UnlockKeyAnimation() {
            keyAnimationLocked = false;
        }

        /// <summary>
        /// Skip the remainder of the current animation. Useful if you need to immediately hide an element or position it off-screen.
        /// </summary>
        public void SkipAnimation() {
            animator.Finish();
        }

        /// <summary>
        /// Update animation interpolation and calculate <c>ComputedArea</c>.
        /// </summary>
        /// <param name="now">Current, total elapsed time.</param>
        /// <param name="viewport">The viewport of of the target area.</param>
        public void Update(TimeSpan now,VectorRectangle viewport) {
            animator.Update(now);
            if(waitingForAnimation && animator.IsFinished) {
                waitingForAnimation = false;
                KeyAnimation(now);
            }
            ElementLayoutData layout;
            if(oldLayout is not null) {
                layout = ElementLayoutData.Interpolate(oldLayout.Value,this.layout,animator.Value,SmoothStep);
            } else {
                layout = this.layout;
            }
            if(Flags.HasFlag(ElementFlags.CanUpdate) && !waitingForAnimation) {
                OnUpdate?.Invoke(now);
            }
            UpdateComputedArea(layout,viewport);
        }

        private bool waitingForAnimation = false;

        public void PauseInputForAnimation() {
            waitingForAnimation = true;
        }

        /// <summary>
        /// Update layout properties based on <c>Pressed</c> and <c>Selected</c> properties.
        /// </summary>
        protected internal event Action<TimeSpan> OnUpdate;

        protected internal event Action<TimeSpan> OnActivated;

        private bool _pressed, _selected;

        /// <summary>
        /// Filtered by <c>WaitingForAnimationBeforeInput</c>.
        /// </summary>
        public bool Pressed {
            get => _pressed && !waitingForAnimation;
        }

        /// <summary>
        /// Filtered by <c>WaitingForAnimationBeforeInput</c>.
        /// </summary>
        public bool Selected {
            get => _selected && !waitingForAnimation;
        }

        internal void SetSelected() => _selected = true;
        internal void SetPressed() => _pressed = true;

        internal void ClearSelected() => _selected = false;
        internal void ClearPressed() => _pressed = false;

        public void Activate(TimeSpan now) {
            if(!Flags.HasFlag(ElementFlags.Interactable) || waitingForAnimation) {
                return;
            }
            OnActivated?.Invoke(now);
        }

        public Element PreviousElement { get; set; } = null;
        public Element NextElement { get; set; } = null;

        public ElementFlags Flags { get; set; } = ElementFlags.None;

        public void SetKeyFocus(Element previous,Element next) {
            NextElement = next;
            PreviousElement = previous;
        }

        public void ClearKeyFocus() {
            PreviousElement = null;
            NextElement = null;
        }
    }
}
