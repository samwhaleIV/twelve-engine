using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner> where TOwner:GameState {

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

        public GameManager Game { get; private set; }
        public TOwner Owner { get; private set; } = null;

        public TimeSpan Now => Owner.Now;
        public TimeSpan TimeDelta => Owner.TimeDelta;

        internal object EntityManager { get; private set; } = null;

        internal void Register(int ID,TOwner owner,object entityManager) {
            this.ID = ID;
            Owner = owner;
            Game = owner.Game;
            EntityManager = entityManager;
        }

        protected event Action OnLoad, OnUnload;

        internal event Action<Entity<TOwner>> OnSortedOrderChange;
        protected void NotifySortedOrderChange() => OnSortedOrderChange?.Invoke(this);

        internal void Load() {
            IsLoading = true;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;
        }

        internal void Unload() {
            OnUnload?.Invoke();
            ID = DEFAULT_ID;
            Owner = null;
            Game = null;
            IsLoaded = false;
        }

        public bool IsVisible { get; set; } = true;

        public event Action OnUpdate, OnRender, OnPreRender;

        public void Update() => OnUpdate?.Invoke();

        public void PreRender() {
            if(!IsVisible) {
                return;
            }
            OnPreRender?.Invoke();
        }

        public void Render() {
            if(!IsVisible) {
                return;
            }
            OnRender?.Invoke();
        }
    }
}
