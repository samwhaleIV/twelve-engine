using System.Collections.Generic;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public sealed partial class EntityManager<TEntity,TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        public TEntity Get(int ID) {
            if(!container.IDs.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity;
        }
        public TType Get<TType>(int ID) where TType : TEntity {
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

        public bool TryGet(int ID,out TEntity entity) {
            return container.IDs.TryGetValue(ID,out entity);
        }

        public bool TryGet(string name,out TEntity entity) {
            return container.Names.TryGetValue(name,out entity);
        }

        public bool TryGet<TType>(int ID,out TType entity) where TType:TEntity {
            var result = container.IDs.TryGetValue(ID,out TEntity _entity);
            entity = (TType)_entity;
            return result;
        }

        public bool TryGet<TType>(string name,out TType entity) where TType : TEntity {
            var result = container.Names.TryGetValue(name,out TEntity _entity);
            entity = (TType)_entity;
            return result;
        }
    }
}
