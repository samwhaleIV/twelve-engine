using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public sealed partial class EntityManager<TEntity,TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        public TEntity Get(int ID) {
            if(!Container.IDs.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity;
        }
        public TType Get<TType>(int ID) where TType : TEntity {
            if(!Container.IDs.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity as TType;
        }

        public TEntity Get(string name) {
            if(!Container.Names.TryGetValue(name,out var entity)) {
                return null;
            }
            return entity;
        }
        public TType Get<TType>(string name) where TType : TEntity {
            if(!Container.Names.TryGetValue(name,out var entity)) {
                return null;
            }
            return entity as TType;
        }

        public bool Has(string name) {
            return Container.Names.ContainsKey(name);
        }
        public bool Has(int ID) {
            return Container.IDs.ContainsKey(ID);
        }

        public bool TryGet(int ID,out TEntity entity) {
            return Container.IDs.TryGetValue(ID,out entity);
        }

        public bool TryGet(string name,out TEntity entity) {
            return Container.Names.TryGetValue(name,out entity);
        }

        public bool TryGet<TType>(int ID,out TType entity) where TType:TEntity {
            var result = Container.IDs.TryGetValue(ID,out TEntity _entity);
            entity = (TType)_entity;
            return result;
        }

        public bool TryGet<TType>(string name,out TType entity) where TType : TEntity {
            var result = Container.Names.TryGetValue(name,out TEntity _entity);
            entity = (TType)_entity;
            return result;
        }
    }
}
