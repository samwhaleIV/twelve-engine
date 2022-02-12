using System;
using System.Collections.Generic;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public sealed class EntityFactory<TEntity,TOwner> where TEntity:Entity<TOwner> where TOwner:GameState {

        private readonly Dictionary<int,Func<TEntity>> generators;

        public EntityFactory(params (int entityType, Func<TEntity> generator)[] types) {
            generators = new Dictionary<int,Func<TEntity>>();
            foreach(var type in types) {
                generators[type.entityType] = type.generator;
            }
        }

        public TEntity Create(int type) {
            if(!generators.ContainsKey(type)) {
                return null;
            }
            return generators[type].Invoke();
        }
    }
}
