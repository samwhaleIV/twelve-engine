using System.Collections.Generic;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem.EntityContainer {
    internal sealed class EntityContainer<TEntity,TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        public EntityContainer() {
            containerWriter = new ContainerWriter<TEntity,TOwner>(this);
        }

        private readonly Dictionary<int,TEntity> entityDictionary = new Dictionary<int,TEntity>();
        private readonly Dictionary<string,TEntity> namedEntities = new Dictionary<string,TEntity>();

        private readonly Dictionary<int,HashSet<int>> typeTable = new Dictionary<int,HashSet<int>>();

        internal Dictionary<int,TEntity> IDs => entityDictionary;
        internal Dictionary<string,TEntity> Names => namedEntities;

        internal Dictionary<int,HashSet<int>> Types => typeTable;

        private readonly ContainerWriter<TEntity,TOwner> containerWriter;

        public ContainerWriter<TEntity,TOwner> Writer => containerWriter;
    }
}
