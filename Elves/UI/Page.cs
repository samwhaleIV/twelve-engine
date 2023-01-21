using System;
using TwelveEngine;

namespace Elves.UI {
    public abstract class Page<TElement> where TElement:Element {

        protected Book<TElement> Book { get; private set; }
        internal void SetBook(Book<TElement> book) => Book = book;

        public abstract void Open(TimeSpan now);

        /// <summary>
        /// A good place to remove an element's event handlers.
        /// </summary>
        public abstract void Close();

        public abstract void Update(VectorRectangle viewport);

        public virtual void Back() { }

        public TElement DefaultFocusElement { get; protected set; }
    }
}
