using System;
using System.Collections.Generic;

namespace TwelveEngine.EntitySystem {
    public sealed class EntityFactory<T,OwnerType> where T:Entity<OwnerType> where OwnerType:class {

        private readonly Dictionary<int,Func<T>> generators;

        public EntityFactory(params (int entityType, Func<T> generator)[] types) {
            generators = new Dictionary<int,Func<T>>();
            foreach(var type in types) {
                generators[type.entityType] = type.generator;
            }
        }

        public T Create(int type) {
            if(!generators.ContainsKey(type)) {
                return null;
            }
            return generators[type].Invoke();
        }
    }
}
