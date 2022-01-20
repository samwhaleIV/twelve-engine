using System.Collections.Generic;

namespace TwelveEngine.EntitySystem {
    public sealed class EntityContainer<TEntity,TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        private readonly Dictionary<int,TEntity> entityDictionary = new Dictionary<int,TEntity>();
        private readonly Dictionary<string,TEntity> namedEntities = new Dictionary<string,TEntity>();

        private readonly Dictionary<int,HashSet<int>> typeTable = new Dictionary<int,HashSet<int>>();
        private readonly Dictionary<int,HashSet<int>> componentTable = new Dictionary<int,HashSet<int>>();

        internal Dictionary<int,TEntity> IDs => entityDictionary;
        internal Dictionary<string,TEntity> Names => namedEntities;

        internal Dictionary<int,HashSet<int>> Types => typeTable;
        internal Dictionary<int,HashSet<int>> Components => componentTable;
    }
}
