using System;
using System.Collections.Generic;
using TwelveEngine.Serial;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner>:ISerializable where TOwner:GameState {

        private readonly HashSet<int> componentTypes = new HashSet<int>();
        private readonly Dictionary<int,int[]> components = new Dictionary<int,int[]>();

        internal HashSet<int> ComponentTypes => componentTypes;
        /* Fire event nulling isn't strictly required, but it is included as a fail safe in the 'event' that EntityManager is flawed */
        private void SetComponent(int componentType,int[] value,bool fireEvent = false) {
            components[componentType] = value;
            if(ComponentTypes.Add(componentType) && fireEvent) {
                OnComponentAdded?.Invoke(ID,componentType);
            }
        }

        public void RemoveComponent(int componentType) {
            components.Remove(componentType);
            if(ComponentTypes.Remove(componentType)) {
                OnComponentRemoved?.Invoke(ID,componentType);
            }
        }

        public void SetComponent(int componentType,int[] value) {
            SetComponent(componentType,value,fireEvent: true);
        }

        public int[] GetComponent(int componentType) {
            if(!components.TryGetValue(componentType,out var component)) {
                return null;
            }
            return component;
        }

        public bool HasComponent(int componentType) {
            return components.ContainsKey(componentType);
        }

        internal event Action<int,int> OnComponentRemoved, OnComponentAdded;

        protected abstract int GetEntityType();
        public int Type { get; set; }

        private string name = string.Empty;

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
            ID = EntityManager.START_ID - 1;
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
            foreach(var component in components) {
                frame.Set(component.Key);
                frame.Set(component.Value);
            }
        }
        private void importComponents(SerialFrame frame) {
            var componentCount = frame.GetInt();
            for(int i = 0;i<componentCount;i++) {
                SetComponent(frame.GetInt(),frame.GetIntArray());
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
