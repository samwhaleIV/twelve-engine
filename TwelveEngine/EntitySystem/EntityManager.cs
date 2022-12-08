using Microsoft.Xna.Framework;
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

        private readonly struct RegisteredEntity {

            public readonly TEntity Value;
            public readonly int RegisteredID;

            public RegisteredEntity(TEntity entity) {
                Value = entity;
                RegisteredID = entity.ID;
            }
        }

        private sealed class EntityComparer:IComparer<TEntity> {
            public int Compare(TEntity a,TEntity b) {
                if(a.Depth == b.Depth) {
                    return a.ID.CompareTo(b.ID); /* FIFO; oldest entity is rendered on the lowest virtual layer.
                                                  * Perceptually, newer entities are 'closer'. */
                } else {
                    return a.Depth.CompareTo(b.Depth);
                }
            }
        }

        private readonly LowMemorySortedSet<TEntity> Entities = new LowMemorySortedSet<TEntity>(EntityManager.STARTING_CAPACITY,new EntityComparer());
        private readonly Queue<RegisteredEntity> EntitiesBuffer = new Queue<RegisteredEntity>(EntityManager.STARTING_CAPACITY);

        private void Entity_OnDepthChanged(Entity<TOwner> entity) {
            Entities.Remove(entity.ID);
            Entities.Add(entity.ID,(TEntity)entity);
        }

        public EntityManager(TOwner owner) {
            if(owner == null) {
                throw new ArgumentNullException(nameof(owner));
            }
            owner.OnUnload += Owner_Unload;
            Owner = owner;
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

        private readonly Queue<RegisteredEntity> AdditionQueue = new Queue<RegisteredEntity>();

        public void Update(GameTime gameTime) {
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
                entity.Value.Update(gameTime);
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
                entity.Value.Update(gameTime);
            }
            IsIterating = false;
        }

        public void Render(GameTime gameTime) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating || IsIteratingSorted) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIteratingSorted = true;
            IsIterating = true;
            for(int i = 0;i<Entities.Count;i++) {
                Entities.List[i].Render(gameTime);
            }
            IsIteratingSorted = false;
            IsIterating = false;
        }

        public void PreRender(GameTime gameTime) {
            if(IsUnloaded) {
                throw new EntityManagerException(ILLEGAL_ITERATION_UNLOADED);
            }
            if(IsIterating || IsIteratingSorted) {
                throw new EntityManagerException(ILLEGAL_ITERATION);
            }
            IsIteratingSorted = true;
            IsIterating = true;
            for(int i = 0;i<Entities.Count;i++) {
                Entities.List[i].PreRender(gameTime);
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
            entity.OnDepthChanged += Entity_OnDepthChanged;
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
            entity.OnDepthChanged -= Entity_OnDepthChanged;
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

            Queue<TEntity> entitiesCopy = new Queue<TEntity>(Entities.Count);

            for(int i = 0;i<Entities.Count;i++) {
                entitiesCopy.Enqueue(Entities.List[i]);
            }

            while(entitiesCopy.TryDequeue(out var entity)) {
                Container.Writer.RemoveEntity(entity);
                entity.OnDepthChanged -= Entity_OnDepthChanged;
                entity.Unload();
            }
        }
    }
}
