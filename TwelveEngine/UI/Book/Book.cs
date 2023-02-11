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

        private bool _transitionEnded = true;

        protected override bool GetContextTransitioning() {
            return !pageTransitionAnimator.IsFinished;
        }

        protected override IEnumerable<BookElement> GetElements() => Elements;

        protected event Action OnPageTransitionStart, OnPageTransitionEnd;

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
            
            OnPageTransitionStart?.Invoke();

            var now = Now;
            ResetInteractionState();
            pageTransitionAnimator.Reset(now);

            var viewport = GetViewport();

            oldPage.SetTime(now);
            oldPage.SetTransitionDuration(TransitionDuration);
            oldPage.SetViewport(viewport);
            oldPage.Close();

            newPage.SetTime(now);
            newPage.SetTransitionDuration(TransitionDuration);
            newPage.SetViewport(viewport);
            DefaultFocusElement = newPage.Open();

            Page = newPage;
            _transitionEnded = false;
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

            var viewport = GetViewport();
            foreach(var element in Elements) {
                element.SetViewport(viewport);
            }
            newPage.SetViewport(viewport);
            DefaultFocusElement = newPage.Open();
            newPage.Update();

            foreach(var element in Elements) {
                element.UpdateAnimatedComputedArea(now);
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

        private void Update(FloatRectangle viewport) {
            var now = Now;
            pageTransitionAnimator.Update(now);
            if(!_transitionEnded && !IsTransitioning) {
                _transitionEnded = true;
                OnPageTransitionEnd?.Invoke();
                FocusDefault();
            }
            if(Page is null) {
                foreach(var element in Elements) {
                    element.SetViewport(viewport);
                }
            } else {
                Page.SetTime(now);
                foreach(var element in Elements) {
                    element.SetViewport(viewport);
                }
                Page.SetViewport(viewport);
                Page.Update();
            }
            foreach(var element in Elements) {
                element.UpdateAnimatedComputedArea(now);
            }
        }

        public void Update() => Update(GetViewport());

        protected override bool BackButtonPressed() {
            if(Page is null) {
                return false;
            }
            return Page.Back();
        }
    }
}
