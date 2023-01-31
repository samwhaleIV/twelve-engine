using TwelveEngine.EntitySystem.EntityContainer;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    internal static class EntityManager {
        internal const int NO_ID = -1;
        internal const int START_ID = 0;
        internal const int STARTING_CAPACITY = 8;
    }
    public sealed partial class EntityManager<TEntity,TOwner> where TEntity:Entity<TOwner> where TOwner:GameState, IEntitySorter<TEntity,TOwner> {

        private const string ILLEGAL_ITERATION = "Illegal nested iteration. Cannot iterate inside of another iteration operation.";
        private const string ILLEGAL_MODIFICATION_UNLOADED = "Cannot mutate contained entities, the entity manager is unloaded.";
        private const string ILLEGAL_ITERATION_UNLOADED = "Cannot iterate entity list, the entity manager is unloaded.";

        private readonly TOwner Owner;
        private readonly EntityContainer<TEntity,TOwner> Container = new();

        private bool IsIterating = false, IsUnloaded = false, IsIteratingSorted = false;

        private readonly struct RegisteredEntity {

            public readonly TEntity Value;
            public readonly int RegisteredID;

            public RegisteredEntity(TEntity entity) {
                Value = entity;
                RegisteredID = entity.ID;
            }
        }

        private readonly LowMemorySortedSet<TEntity> Entities;

        public EntityManager(TOwner owner) {
            if(owner == null) {
                throw new ArgumentNullException(nameof(owner));
            }
            Entities = new(EntityManager.STARTING_CAPACITY,owner.GetEntitySorter());
            owner.OnUnload += Owner_Unload;
            Owner = owner;
        }

        /// <summary>
        /// Call when the depth evaluation of entites has substantially changed. E.g., a 3D camera changes positions and your entities are Z sorted.
        /// </summary>
        public void RefreshSorting() => Entities.Refresh();

        private readonly Queue<RegisteredEntity> EntitiesBuffer = new(EntityManager.STARTING_CAPACITY);

        private void Entity_OnDepthChanged(Entity<TOwner> entity) {
            Entities.Remove(entity.ID);
            Entities.Add(entity.ID,(TEntity)entity);
        }

        private int IDCounter = EntityManager.START_ID;
        private int GetNextID() => IDCounter++;

        private bool bufferNeedsUpdate = false;

        private void TryUpdateBuffer() {
            if(!bufferNeedsUpdate) {
                return;
            }
            EntitiesBuffer.Clear();
            for(int i = 0;i<Entities.Count;i++) {
                EntitiesBuffer.Enqueue(new RegisteredEntity(Entities.List[i]));
            }
            bufferNeedsUpdate = false;
        }

        private readonly Queue<RegisteredEntity> AdditionQueue = new();

        public void Update() {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating || IsIteratingSorted) {
                /* This should prevent shitty programming practices and impossible to fix bugs.
                 * This class is logically threaded around the trust in this assertion */
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIterating = true;
            TryUpdateBuffer();
            foreach(var entity in EntitiesBuffer) {
                /* Check if the entity was removed during another entity's invoked method call. I.e. Update() */
                if(entity.Value.ID != entity.RegisteredID) {
                    continue;
                }
                entity.Value.Update();
                if(IsUnloaded) {
                    IsIterating = false;
                    return;
                }
            }
            while(AdditionQueue.TryDequeue(out var entity)) {
                /* This handles a weird edge case: where you add an entity and remove it back in the same frame
                 * (or even weirder, if you add the entity back again and generate a new ID for it) */
                if(entity.Value.ID != entity.RegisteredID) {
                    continue;
                }
                /* This loop is effectively recursive, if this action happens to add new entities */
                entity.Value.Update();
            }
            IsIterating = false;
        }

        public void Render() {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating || IsIteratingSorted) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIteratingSorted = true;
            IsIterating = true;
            for(int i = 0;i<Entities.Count;i++) {
                Entities.List[i].Render();
            }
            IsIteratingSorted = false;
            IsIterating = false;
        }

        public void PreRender() {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating || IsIteratingSorted) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIteratingSorted = true;
            IsIterating = true;
            for(int i = 0;i<Entities.Count;i++) {
                Entities.List[i].PreRender();
            }
            IsIteratingSorted = false;
            IsIterating = false;
        }

        public TEntity Add(TEntity entity) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_MODIFICATION_UNLOADED);
            }
            if(IsIteratingSorted) {
                throw new EntityManagerException("Cannot add an entity during sorted iteration!");
            }
            if(entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }
            if(entity.EntityManager != null) {
                throw new EntityManagerException($"Entity is already registered to {(entity.EntityManager == this ? "this" : "another")} entity manager.");
            }
            entity.Register(GetNextID(),Owner,this);
            Container.Writer.AddEntity(entity);
            entity.Load();
            Entities.Add(entity.ID,entity);
            entity.OnSortedOrderChange += Entity_OnDepthChanged;
            bufferNeedsUpdate = true;
            if(IsIterating) {
                AdditionQueue.Enqueue(new RegisteredEntity(entity));
            }
            return entity;
        }

        public void Remove(TEntity entity) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_MODIFICATION_UNLOADED);
            }
            if(IsIteratingSorted) {
                throw new EntityManagerException("Cannot remove an entity during sorted iteration!");
            }
            if(entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }
            if(entity.EntityManager != this) {
                throw new EntityManagerException(entity.EntityManager == null ?
                    "This entity does not belong to this entity manager. In fact, it doesn't belong to one at all." :
                    "Cannot remove entity because it is not registered to this entity manager."
                );
            }
            Container.Writer.RemoveEntity(entity);
            entity.Unload();
            entity.OnSortedOrderChange -= Entity_OnDepthChanged;
            Entities.Remove(entity.ID);
            bufferNeedsUpdate = true;
        }

        public void Add(params TEntity[] entities) {
            if(entities == null) {
                throw new ArgumentNullException(nameof(entities));
            }
            foreach(var entity in entities) {
                Add(entity);
            }
        }

        public void Remove(params TEntity[] entities) {
            if(entities == null) {
                throw new ArgumentNullException(nameof(entities));
            }
            foreach(var entity in entities) {
                Remove(entity);
            }
        }

        private void Owner_Unload() {
            if(IsUnloaded) {
                throw new EntityManagerException("Entity manager is already unloaded! (Your GameManager's execution ordering might be broken.)");
            }
            IsUnloaded = true;
            if(Entities.Count <= 0) {
                return;
            }

            bufferNeedsUpdate = false;

            Queue<TEntity> entitiesCopy = new(Entities.Count);

            for(int i = 0;i<Entities.Count;i++) {
                entitiesCopy.Enqueue(Entities.List[i]);
            }

            while(entitiesCopy.TryDequeue(out var entity)) {
                Container.Writer.RemoveEntity(entity);
                entity.OnSortedOrderChange -= Entity_OnDepthChanged;
                entity.Unload();
            }

            Entities.Clear();
        }
    }
}
