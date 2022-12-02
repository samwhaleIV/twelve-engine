using System;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner> where TOwner:GameState {

        private const int DEFAULT_ID = EntityManager.START_ID - 1;

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

        public bool HasName => !string.IsNullOrEmpty(_name);

        /* int: ID, string: NewName */
        internal event Action<int,string> OnNameChanged;
        public int ID { get; private set; }

        public bool IsDeleted { get; internal set; } = false;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public GameManager Game { get; private set; }
        public TOwner Owner { get; private set; } = null;

        internal void Register(int ID,TOwner owner) {
            this.ID = ID;
            Owner = owner;
            Game = owner.Game;
        }

        protected event Action OnLoad, OnUnload;

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
    }
}
