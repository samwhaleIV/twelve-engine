﻿namespace TwelveEngine.UI.Book {
    public abstract class BookPage<TElement> where TElement:BookElement {

        /// <summary>
        /// Current total time. 
        /// </summary>
        protected TimeSpan Now { get; private set; }

        protected FloatRectangle Viewport { get; private set; }

        /// <summary>
        /// The latest transition duration. This may not reflect a changed value on the parent book, only the most recently used.
        /// </summary>
        protected TimeSpan TransitionDuration { get; private set; }

        internal void SetTime(TimeSpan now) => Now = now;
        internal void SetTransitionDuration(TimeSpan duration) => TransitionDuration = duration;
        internal void SetViewport(FloatRectangle viewport) => Viewport = viewport;

        /// <summary>
        /// Setup the page because it has been opened. No need to key element animations. They will be dropped until the page transition period has ended.
        /// </summary>
        /// <returns>Default focus element. A hint for the UI selection system on how to approach the first element when no previous selected element has been used on this display of the page.</returns>
        public abstract BookElement Open();

        /// <summary>
        /// A good place to remove an element's event handlers.
        /// </summary>
        public abstract void Close();

        public abstract void Update();

        /// <summary>
        /// Method is executed when a keyboard/gamepad cancel impulse has been pressed.
        /// </summary>
        /// <returns>A value indicating whether the back request was fulfilled or ignored. For diagnostic purposes.</returns>
        public virtual bool Back() {
            return false;
        }
    }
}
