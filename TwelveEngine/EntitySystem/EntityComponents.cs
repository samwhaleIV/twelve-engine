using System;
using TwelveEngine.Serial;
using System.Collections.Generic;

namespace TwelveEngine.EntitySystem {
    public sealed class EntityComponents {

        private readonly HashSet<int> types = new HashSet<int>();
        private readonly Dictionary<int,int[]> components = new Dictionary<int,int[]>();

        internal HashSet<int> Types => types;
        internal event Action<int> OnRemoved, OnAdded;

        internal void Export(SerialFrame frame) {
            var componentCount = components.Count;
            frame.Set(componentCount);
            foreach(var component in components) {
                frame.Set(component.Key);
                frame.Set(component.Value);
            }
        }

        internal void Import(SerialFrame frame) {
            var componentCount = frame.GetInt();
            for(int i = 0;i<componentCount;i++) {
                Set(frame.GetInt(),frame.GetIntArray());
            }
        }

        public void Set(int type,int[] value) {
            components[type] = value;
            if(types.Add(type)) {
                OnAdded?.Invoke(type);
            }
        }

        public void Remove(int componentType) {
            components.Remove(componentType);
            if(types.Remove(componentType)) {
                OnRemoved?.Invoke(componentType);
            }
        }

        public void Set(params (int Type, int[] Value)[] components) {
            foreach(var component in components) {
                Set(component.Type,component.Value);
            }
        }

        public int[] Get(int type) {
            if(!components.TryGetValue(type,out var value)) {
                return null;
            }
            return value;
        }

        public bool TryGet(int type,out int[] value) {
            return components.TryGetValue(type,out value);
        }

        public bool Has(int type) {
            return components.ContainsKey(type);
        }
    }
}
