using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine.EntitySystem {
    public sealed partial class EntityManager<TEntity, TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {
        public TEntity Get(int ID) {
            if(!container.IDs.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity;
        }
        public TEntity Get<TType>(int ID) where TType : TEntity {
            if(!container.IDs.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity as TType;
        }

        public TEntity Get(string name) {
            if(!container.Names.TryGetValue(name,out var entity)) {
                return null;
            }
            return entity;
        }
        public TType Get<TType>(string name) where TType : TEntity {
            if(!container.Names.TryGetValue(name,out var entity)) {
                return null;
            }
            return entity as TType;
        }

        public IEnumerable<TEntity> GetByComponent(int componentType) {
            foreach(var ID in container.Components[componentType]) {
                yield return container.IDs[ID];
            }
        }

        public IEnumerable<TEntity> GetByComponents(HashSet<int> componentTypes) {
            if(componentTypes.Count == 0) {
                yield break;
            }
            foreach(var ID in container.Components[componentTypes.First()]) {
                var entity = container.IDs[ID];
                if(!componentTypes.IsSubsetOf(entity.Components.Types)) {
                    continue;
                }
                yield return entity;
            }
        }

        public IEnumerable<TEntity> GetByType(int entityType) {
            if(!container.Types.ContainsKey(entityType)) {
                yield break;
            }
            foreach(var ID in container.Types[entityType]) {
                yield return container.IDs[ID];
            }
        }

        public IEnumerable<TType> GetByType<TType>(int entityType) where TType : TEntity {
            if(!container.Types.ContainsKey(entityType)) {
                yield break;
            }
            foreach(var ID in container.Types[entityType]) {
                yield return container.IDs[ID] as TType;
            }
        }

        public bool Has(string name) {
            return container.Names.ContainsKey(name);
        }
        public bool Has(int ID) {
            return container.IDs.ContainsKey(ID);
        }
    }
}
