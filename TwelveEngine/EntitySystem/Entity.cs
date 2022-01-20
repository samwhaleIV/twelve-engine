using System;
using TwelveEngine.Serial;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner>:ISerializable where TOwner:GameState {

        private readonly EntityComponents components = new EntityComponents();
        public EntityComponents Components => components;

        internal event Action<int,int> OnComponentAdded, OnComponentRemoved;

        public Entity() {
            Components.OnAdded += Components_OnComponentAdded;
            Components.OnRemoved += Components_OnComponentRemoved;
        }

        private void Components_OnComponentAdded(int componentType) {
            OnComponentAdded?.Invoke(ID,componentType);
        }

        private void Components_OnComponentRemoved(int componentType) {
            OnComponentRemoved?.Invoke(ID,componentType);
        }

        private const int DEFAULT_ID = EntityManager.START_ID - 1;

        protected abstract int GetEntityType();
        public int Type => GetEntityType();

        private string _name = string.Empty;
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

        public bool Deleted { get; internal set; }

        public int ID { get; private set; }

        private GameManager game = null;
        private TOwner owner = null;

        protected GameManager Game => game;
        protected TOwner Owner => owner;

        internal void Register(int ID,TOwner owner) {
            this.ID = ID;
            this.owner = owner;
            game = owner.Game;
        }

        protected event Action OnLoad, OnUnload;

        internal void Load() => OnLoad?.Invoke();

        internal void Unload() {
            OnUnload?.Invoke();
            ID = DEFAULT_ID;
            owner = null;
            game = null;
        }

        public bool StateLock { get; set; } = false;

        protected event Action<SerialFrame> OnExport, OnImport;

        public void Export(SerialFrame frame) {
            frame.Set(Name);
            Components.Export(frame);
            OnExport?.Invoke(frame);
        }
        public void Import(SerialFrame frame) {
            Name = frame.GetString();
            Components.Import(frame);
            OnImport?.Invoke(frame);
        }
    }
}
