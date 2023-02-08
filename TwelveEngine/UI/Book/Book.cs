namespace TwelveEngine.UI.Book {
    public abstract class Book<TElement>:InteractionAgent<BookElement> where TElement:BookElement {

        public static readonly TimeSpan DefaultAnimationDuration = Constants.UI.DefaultAnimationDuration;
        public static readonly TimeSpan DefaultTransitionDuration = Constants.UI.DefaultTransitionDuration;

        protected abstract FloatRectangle GetViewport();

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
        private readonly Interpolator pageTransitionAnimator = new(DefaultTransitionDuration);

        /// <summary>
        /// Flag controlled by <c>UnlockPageControls</c> and <c>LockElementsForTransition</c>. Checked in <c>Update</c>.
        /// </summary>
        private bool _elementsAreLocked = false;

        protected override bool GetContextTransitioning() {
            return !pageTransitionAnimator.IsFinished;
        }

        protected override IEnumerable<BookElement> GetElements() => Elements;

        public void SetPage(BookPage<TElement> newPage) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }

            BookPage<TElement> oldPage = Page;
            if(oldPage is null) {
                throw new InvalidOperationException("Cannot set page when a first page has not ben set with SetFirstPage().");
            }
            if(!pageTransitionAnimator.IsFinished) {
                throw new InvalidOperationException("Cannot set a page during a transition.");
            }
            pageTransitionAnimator.Duration = TransitionDuration;

            var now = Now;
            LockElementsForTransition(now);
            pageTransitionAnimator.Reset(now);

            oldPage.SetTime(now);
            oldPage.SetTransitionDuration(TransitionDuration);
            oldPage.Close();

            newPage.SetTime(now);
            newPage.SetTransitionDuration(TransitionDuration);

            DefaultFocusElement = newPage.Open();
            Page = newPage;
        }

        public void SetFirstPage(BookPage<TElement> newPage) {
            if(newPage is null) {
                throw new ArgumentNullException(nameof(newPage));
            }
            if(Page is not null) {
                throw new InvalidOperationException("Cannot set first page when a page is already active.");
            }

            var now = Now;

            newPage.SetTime(now);
            newPage.SetTransitionDuration(TransitionDuration);

            DefaultFocusElement = newPage.Open();

            var viewport = GetViewport();
            newPage.Update(viewport);

            foreach(var element in Elements) {
                element.KeyAnimation(now);
                Update(viewport);
                element.SkipAnimation();
            }

            pageTransitionAnimator.Duration = TransitionDuration;
            pageTransitionAnimator.Finish();

            Page = newPage;
            FocusDefault();
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

        public virtual TSuper AddElement<TSuper>() where TSuper : TElement, new() {
            var element = new TSuper();
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

        private void Update(FloatRectangle viewport) {
            var now = Now;
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

        public void Update() => Update(GetViewport());

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
