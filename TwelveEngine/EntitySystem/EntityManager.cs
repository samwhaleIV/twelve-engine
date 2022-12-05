using System;
using System.Collections.Generic;
using TwelveEngine.EntitySystem.EntityContainer;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    internal static class EntityManager {
        internal const int START_ID = 0;
        internal const int STARTING_CAPACITY = 256;
    }
    public sealed partial class EntityManager<TEntity,TOwner> where TEntity:Entity<TOwner> where TOwner:GameState {

        private const string ILLEGAL_ITERATION = "Illegal nested iteration! Do not iterate inside of another iteration action.";
        private const string ILLEGAL_MUTATION = "Cannot mutate entity buffer during an iteration action!";

        private const string ILLEGAL_MODIFICATION_UNLOADED = "Cannot mutate contained entities, the entity manager is unloaded.";
        private const string ILLEGAL_ITERATION_UNLOADED = "Cannot iterate entity list, the entity manager is unloaded.";

        private readonly TOwner Owner;
        private readonly EntityContainer<TEntity,TOwner> Container = new EntityContainer<TEntity,TOwner>();

        private bool IsIterating = false, IsUnloaded = false;

        private readonly SortedList<int,TEntity> Entities = new SortedList<int,TEntity>(EntityManager.STARTING_CAPACITY);
        private readonly List<(TEntity Entity, int ID)> EntitiesBuffer = new List<(TEntity Entity, int ID)>(EntityManager.STARTING_CAPACITY);

        public EntityManager(TOwner owner) {
            if(owner == null) {
                throw new ArgumentNullException(nameof(owner));
            }
            owner.OnUnload += Owner_Unload;
            Owner = owner;
        }

        private int IDCounter = EntityManager.START_ID;
        private int GetNextID() => IDCounter++;

        public void UpdateBuffer() {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_MODIFICATION_UNLOADED);
            }
            if(IsIterating) {
                throw new EntityManagerException(ILLEGAL_MUTATION);
            }
            EntitiesBuffer.Clear();
            foreach(var kvp in Entities) {
                var entity = kvp.Value;
                EntitiesBuffer.Add((entity,entity.ID));
            }
        }

        public void Iterate(Action<TEntity> action) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIterating = true;
            foreach(var item in EntitiesBuffer) {
                if(item.Entity.ID != item.ID) {
                    continue;
                }
                action(item.Entity);
                if(IsUnloaded) {
                    IsIterating = false;
                    return;
                }
            }
            IsIterating = false;
        }

        public void Iterate<TData>(Action<TEntity,TData> action,TData data) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            foreach(var item in EntitiesBuffer) {
                if(item.Entity.ID != item.ID) {
                    continue;
                }
                action(item.Entity,data);
                if(IsUnloaded) {
                    IsIterating = false;
                    return;
                }
            }
            IsIterating = false;
        }

        public TEntity Add(TEntity entity) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_MODIFICATION_UNLOADED);
            }
            if(entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }
            if(entity.EntityManager != null) {
                throw new EntityManagerException("Entity is already registered!");
            }
            entity.Register(GetNextID(),Owner,this);
            Container.Writer.AddEntity(entity);
            entity.Load();
            Entities.Add(entity.ID,entity);
            return entity;
        }

        public TEntity Remove(TEntity entity) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_MODIFICATION_UNLOADED);
            }
            if(entity == null) {
                throw new ArgumentNullException(nameof(entity));
            }
            if(entity.EntityManager != this) {
                throw new EntityManagerException("Cannot remove entity, it is not registered to this entity manager!");
            }
            Container.Writer.RemoveEntity(entity);
            entity.Remove();
            entity.Unload();
            Entities.Remove(entity.ID);
            return entity;
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
            IsUnloaded = true;
            if(Entities.Count <= 0) {
                return;
            }
            Queue<TEntity> entitiesCopy = new Queue<TEntity>(Entities.Count);
            foreach(var kvp in Entities) {
                entitiesCopy.Enqueue(kvp.Value);
            }
            while(entitiesCopy.TryDequeue(out var entity)) {
                Container.Writer.RemoveEntity(entity);
                entity.Unload();
                Entities.Remove(entity.ID);
            }
        }
    }
}
