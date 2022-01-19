using System;
using TwelveEngine.Serial;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner>:ISerializable where TOwner:GameState {

        protected abstract int GetEntityType();
        public int Type => GetEntityType();

        private string name = string.Empty;

        /* int: ID, string: NewName */
        internal event Action<int,string> OnNameChanged;

        public bool Deleted { get; internal set; }

        private int id = 0;
        public int ID => id;

        private GameManager game = null;
        private TOwner owner = null;

        protected GameManager Game => game;
        protected TOwner Owner => owner;

        internal void Register(int id,TOwner owner) {
            this.id = id;
            this.owner = owner;
            game = owner.Game;
        }

        protected event Action OnLoad, OnUnload;

        internal void Load() => OnLoad?.Invoke();

        internal void Unload() {
            OnUnload?.Invoke();
            id = 0;
            owner = null;
            game = null;
        }

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

        protected event Action<SerialFrame> OnExport, OnImport;

        public void Export(SerialFrame frame) {
            frame.Set(name);
            OnExport?.Invoke(frame);
        }
        public void Import(SerialFrame frame) {
            name = frame.GetString();
            OnImport?.Invoke(frame);
        }
    }
}
