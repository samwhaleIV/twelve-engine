using System;
using System.Collections;
using System.Collections.Generic;
using TwelveEngine.EntitySystem.EntityContainer;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    internal static class EntityManager {
        internal const int START_ID = 0;
        internal const int STARTING_CAPACITY = 128;
    }
    public sealed partial class EntityManager<TEntity,TOwner> where TEntity:Entity<TOwner> where TOwner:GameState {

        private const string ILLEGAL_ITERATION = "Illegal nested iteration. Cannot iterate inside of another iteration operation.";
        private const string ILLEGAL_MODIFICATION_UNLOADED = "Cannot mutate contained entities, the entity manager is unloaded.";
        private const string ILLEGAL_ITERATION_UNLOADED = "Cannot iterate entity list, the entity manager is unloaded.";

        private readonly TOwner Owner;
        private readonly EntityContainer<TEntity,TOwner> Container = new EntityContainer<TEntity,TOwner>();

        private bool IsIterating = false, IsUnloaded = false, IsIteratingSorted = false;

        private readonly SortedList<int,TEntity> Entities = new SortedList<int,TEntity>(EntityManager.STARTING_CAPACITY);
        private readonly List<(TEntity Entity, int ID)> EntitiesBuffer = new List<(TEntity Entity, int ID)>(EntityManager.STARTING_CAPACITY);
        private bool bufferNeedsUpdate = false;

        public EntityManager(TOwner owner) {
            if(owner == null) {
                throw new ArgumentNullException(nameof(owner));
            }
            owner.OnUnload += Owner_Unload;
            Owner = owner;
        }

        private int IDCounter = EntityManager.START_ID;
        private int GetNextID() => IDCounter++;

        private void TryUpdateBuffer() {
            if(!bufferNeedsUpdate) {
                return;
            }
            EntitiesBuffer.Clear();
            foreach(var kvp in Entities) {
                var entity = kvp.Value;
                EntitiesBuffer.Add((entity, entity.ID));
            }
            bufferNeedsUpdate = false;
        }

        private readonly Queue<(TEntity Entity,int ID)> additionQueue = new Queue<(TEntity Entity, int ID)>();

        public void Iterate<TData>(Action<TEntity,TData> action,TData data) {
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
            foreach(var item in EntitiesBuffer) {
                /* Check if the entity was removed during another entity's invoked method call. I.e. Update() */
                if(item.Entity.ID != item.ID) {
                    continue;
                }
                action(item.Entity,data);
                if(IsUnloaded) {
                    IsIterating = false;
                    return;
                }
            }
            while(additionQueue.TryDequeue(out var item)) {
                /* This handles a weird edge case: where you add an entity and remove it back in the same frame
                 * (or even weirder, if you add the entity back again and generate a new ID for it) */
                if(item.Entity.ID != item.ID) {
                    continue;
                }
                /* This loop is effectively recursive, if this action happens to add new entities */
                action(item.Entity,data);
            }
            IsIterating = false;
        }

        private class RenderBufferSort:IComparer<TEntity> {
            public int Compare(TEntity a,TEntity b) {
                if(a.Depth == b.Depth) {
                    return a.ID.CompareTo(b.ID);
                } else {
                    return a.Depth.CompareTo(b.Depth);
                }
            }
        }

        private readonly SortedSet<TEntity> renderBuffer = new SortedSet<TEntity>(new RenderBufferSort());

        public void IterateDepthSorted<TData>(Action<TEntity,TData> action,TData data) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating || IsIteratingSorted) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIteratingSorted = true;
            IsIterating = true;
            TryUpdateBuffer();

            foreach(var item in Entities) {
                renderBuffer.Add(item.Value);
            }
            foreach(var entity in renderBuffer) {
                action(entity,data);
            }
            renderBuffer.Clear();

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
            bufferNeedsUpdate = true;
            if(IsIterating) {
                additionQueue.Enqueue((entity, entity.ID));
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
            entity.Remove();
            entity.Unload();
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

            /* Just trying to drop as many references as possible, so GC can work faster
             * to cleanup any entities with huge amounts of data in their object */
            EntitiesBuffer.Clear();
            bufferNeedsUpdate = false;

            Queue<TEntity> entitiesCopy = new Queue<TEntity>(Entities.Count);
            foreach(var kvp in Entities) {
                entitiesCopy.Enqueue(kvp.Value);
            }
            while(entitiesCopy.TryDequeue(out var entity)) {
                Container.Writer.RemoveEntity(entity);
                entity.Unload();
                Entities.Remove(entity.ID);
            }
            Entities.Clear();
        }
    }
}
