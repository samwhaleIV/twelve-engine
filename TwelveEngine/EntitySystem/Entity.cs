using Microsoft.Xna.Framework.Content;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner> where TOwner:GameState {

        public TOwner Owner { get; private set; } = null;

        public ContentManager Content => Owner.Content;
        public SpriteBatch SpriteBatch => Owner.SpriteBatch;
        public RenderTargetStack RenderTarget => Owner.RenderTarget;
        public GraphicsDevice GraphicsDevice => Owner.GraphicsDevice;

        public Viewport Viewport => Owner.RenderTarget.GetViewport();

        public TimeSpan Now => Owner.Now;
        public TimeSpan RealTime => Owner.RealTime;
        public TimeSpan FrameDelta => Owner.FrameDelta;
        public TimeSpan LocalNow => Owner.LocalNow;
        public TimeSpan LocalRealTime => Owner.LocalRealTime;

        public bool GameIsActive => Owner.GameIsActive;
        public bool GameIsPaused => Owner.GameIsPaused;

        private const int DEFAULT_ID = EntitySystem.EntityManager.NO_ID;

        private string _name = null;
        private void SetName(string newName) {
            var oldName = _name;
            _name = newName;
            if(newName != oldName) {
                OnNameChanged?.Invoke(ID,oldName);
            }
        }
        public string Name {
            get => _name;
            set => SetName(value);
        }

        public bool HasName => _name != null;

        /* int: ID, string: NewName */
        internal event Action<int,string> OnNameChanged;
        public int ID { get; private set; }

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        internal object EntityManager { get; private set; } = null;

        internal void Register(int ID,TOwner owner,object entityManager) {
            this.ID = ID;
            Owner = owner;
            EntityManager = entityManager;
        }

        internal event Action<Entity<TOwner>> OnSortedOrderChange;
        protected void NotifySortedOrderChange() => OnSortedOrderChange?.Invoke(this);

        internal void Internal_Load() {
            IsLoading = true;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;
        }

        internal void Internal_Unload() {
            OnUnload?.Invoke();
            ID = DEFAULT_ID;
            Owner = null;
            IsLoaded = false;
        }

        public bool IsVisible { get; set; } = true;

        internal void Internal_Update() {
            OnUpdate?.Invoke();
        }

        internal void Internal_PreRender() {
            if(!IsVisible) {
                return;
            }
            OnPreRender?.Invoke();
        }

        internal void Internal_Render() {
            if(!IsVisible) {
                return;
            }
            OnRender?.Invoke();
        }

        protected event Action OnUpdate, OnRender, OnPreRender, OnLoad, OnUnload;
    }
}
