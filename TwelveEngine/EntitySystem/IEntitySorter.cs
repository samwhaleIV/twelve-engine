using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public interface IEntitySorter<TEntity,TGameState> where TEntity:Entity<TGameState> where TGameState:GameState {
        public sealed class DefaultComparison:IComparer<TEntity> {
            public int Compare(TEntity a,TEntity b) => a.ID.CompareTo(b.ID);
        }
        IComparer<TEntity> GetEntitySorter();
    }
}