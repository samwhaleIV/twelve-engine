using System;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<OwnerType>:ISerializable where OwnerType:class {

        protected abstract int GetEntityType();
        public int Type => GetEntityType();

        private string name = string.Empty;

        internal event Action<int,string> OnNameChanged;

        private int id = 0;
        public int ID => id;

        private GameManager game;
        private OwnerType owner;

        protected GameManager Game => game;
        protected OwnerType Owner => owner;

        internal void Register(int id,GameManager game,OwnerType owner) {
            this.id = id;
            this.owner = owner;
            this.game = game;
        }

        protected event Action OnLoad, OnUnload;

        internal void Load() => OnLoad?.Invoke();
        internal void Unload() => OnUnload?.Invoke();

        public bool StateLock { get; set; } = false;

        private void setName(string newName) {
            var oldName = name;
            name = newName;
            OnNameChanged?.Invoke(ID,oldName);
        }

        public string Name {
            get => name;
            set => setName(value);
        }

        public virtual void Export(SerialFrame frame) => frame.Set(name);

        public virtual void Import(SerialFrame frame) => name = frame.GetString();
    }
}
