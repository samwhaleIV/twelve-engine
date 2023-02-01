namespace TwelveEngine.UI.Book {
    public abstract class Book<TElement>:InteractionAgent<BookElement> where TElement:BookElement {

        public static readonly TimeSpan DefaultAnimationDuration = Constants.UI.DefaultAnimationDuration;
        public static readonly TimeSpan DefaultTransitionDuration = Constants.UI.DefaultTransitionDuration;

        /// <summary>
        /// Must be iterated by a parent class in order to, for example, create a rendering method. See <c>SpriteBook</c> for an example.
        /// </summary>
        protected readonly List<TElement> Elements = new();

        /// <summary>
        /// The current, active page.
        /// </summary>
        public BookPage<TElement> Page { get; private set; } = null;

        /// <summary>
        /// The transition duration when changing the active page. By default, <c>DefaultTransitionDuration</c>.
        /// </summary>
        public TimeSpan TransitionDuration { get; set; } = DefaultTransitionDuration;
        private readonly Interpolator pageTransitionAnimator = new(DefaultAnimationDuration);

        /// <summary>
        /// Flag controlled by <c>UnlockPageControls</c> and <c>LockElementsForTransition</c>. Checked in <c>Update</c>.
        /// </summary>
        private bool _elementsAreLocked = true;

        /// <summary>
        /// The time that <c>Update()</c> was last invoked with.
        /// </summary>
        private TimeSpan _currentTime = TimeSpan.FromHours(-1); /* Hopefully your users aren't time travelers. */

        protected override bool GetContextTransitioning() {
            return !pageTransitionAnimator.IsFinished;
        }

        protected override TimeSpan GetCurrentTime() => _currentTime;

        protected override IEnumerable<BookElement> GetElements() => Elements;

        public void SetPage(BookPage<TElement> newPage) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }

            pageTransitionAnimator.Duration = TransitionDuration;

            BookPage<TElement> oldPage = Page;

            if(oldPage is not null) {
                LockElementsForTransition(_currentTime);
                pageTransitionAnimator.Reset(_currentTime);

                oldPage.SetTime(_currentTime);
                oldPage.SetTransitionDuration(TransitionDuration);
                oldPage.Close();
            } else {
                /* This means this is the very first page, we do no want to animate a transition to it. */
                pageTransitionAnimator.Finish();
            }

            newPage.SetTime(_currentTime);
            newPage.SetTransitionDuration(TransitionDuration);

            DefaultFocusElement = newPage.Open();
            Page = newPage;
        }

        /// <summary>
        /// Adds an element to the element pool. Elements should not be added dynamically. Allocate your elements before setting pages. Dynamic elements will result in undefined behavior.
        /// </summary>
        /// <param name="element">The element to be added to the pool. Don't add the same element more than once.</param>
        /// <returns>The element that has been added. Provided as syntactic sugar for instantiating an element in the argument.</returns>
        public virtual TElement AddElement(TElement element) {
            Elements.Add(element);
            return element;
        }

        /// <summary>
        /// Ends the page transition period and allows interactable elements to be pressed and selected.
        /// </summary>
        private void UnlockPageControls() {
            foreach(var element in Elements) {
                element.UnlockKeyAnimation();
            }
            if(DefaultFocusElement is null) {
                Logger.WriteLine($"UI page has been opened without a default keyboard focus element!",LoggerLabel.UI);
            }
            FocusDefault();
            _elementsAreLocked = false;
        }

        public void Update(TimeSpan now,FloatRectangle viewport) {
            _currentTime = now;
            pageTransitionAnimator.Update(now);
            if(_elementsAreLocked && !IsTransitioning) {
                UnlockPageControls();
            }
            Page?.SetTime(now);
            Page?.Update(viewport);
            foreach(var element in Elements) {
                element.Update(now,viewport);
            }
        }

        /// <summary>
        /// Called on every element when a page is closed. <c>TElement</c> is provided in a key locked state (<c>Element.LockKeyAnimation</c>) and with <c>null</c> focus directives (<c>Element.ClearKeyFocus</c>).
        /// </summary>
        /// <param name="element">The <c>TElement</c> that needs to be reset.</param>
        protected virtual void ResetElement(TElement element) {
            element.Scale = 0;
            element.Flags = ElementFlags.None;
        }

        private void LockAndResetElement(TimeSpan now,TElement element) {
            element.KeyAnimation(now,TransitionDuration);
            element.LockKeyAnimation();
            element.ClearKeyFocus();
            ResetElement(element);
        }

        /// <summary>
        /// Hides and resets all elements to a default/non-interactable state. Prevents new animation keying or modifying current selected or pressed element. Clears interaction state.
        /// </summary>
        /// <param name="now">Current total time.</param>
        private void LockElementsForTransition(TimeSpan now) {
            foreach(var element in Elements) {
                LockAndResetElement(now,element);
            }
            ResetInteractionState();
            _elementsAreLocked = true;
        }

        protected override bool BackButtonPressed() {
            if(Page is null) {
                return false;
            }
            return Page.Back();
        }
    }
}
