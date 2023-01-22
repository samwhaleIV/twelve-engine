using System;
using TwelveEngine;

namespace Elves.UI {
    public abstract class Page<TElement> where TElement:Element {

        public string Name { get; set; } = "No Name";

        protected Book<TElement> Book { get; private set; }
        internal void SetBook(Book<TElement> book) => Book = book;

        protected TimeSpan Now { get; private set; }
        protected TimeSpan TransitionDuration { get; private set; }

        internal void SetTime(TimeSpan now) => Now = now;
        internal void SetTransitionDuration(TimeSpan duration) => TransitionDuration = duration;

        public abstract void Open();

        /// <summary>
        /// A good place to remove an element's event handlers.
        /// </summary>
        public abstract void Close();

        public abstract void Update(VectorRectangle viewport);

        public virtual void Back() { }

        public TElement DefaultFocusElement { get; protected set; }
    }
}
