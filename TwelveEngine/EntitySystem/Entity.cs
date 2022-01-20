using System;
using System.Collections.Generic;
using TwelveEngine.Serial;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner>:ISerializable where TOwner:GameState {

        protected internal HashSet<int> ComponentTypes { get; protected set; }
        private Dictionary<int,Component> components = new Dictionary<int,Component>();

        public void AddComponent(Component component) {
            components[component.Type] = component;
            ComponentTypes.Add(component.Type);
        }

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

        private void exportComponents(SerialFrame frame) {
            var componentCount = components.Count;
            frame.Set(componentCount);
            foreach(var component in components.Values) {
                frame.Set(component.Type);
                frame.Set(component.Value);
            }
        }
        private void importComponents(SerialFrame frame) {
            var componentCount = frame.GetInt();
            for(int i = 0;i<componentCount;i++) {
                AddComponent(new Component() {
                    Type = frame.GetInt(),
                    Value = frame.GetIntArray()
                });
            }
        }

        public void Export(SerialFrame frame) {
            frame.Set(name);
            exportComponents(frame);
            OnExport?.Invoke(frame);
        }
        public void Import(SerialFrame frame) {
            name = frame.GetString();
            importComponents(frame);
            OnImport?.Invoke(frame);
        }
    }
}
