using System;
using System.Collections.Generic;
using System.Linq;
using TwelveEngine.Serial;
using TwelveEngine.EntitySystem.EntityContainer;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public static class EntityManager {
        internal const int START_ID = 0;
    }
    public sealed partial class EntityManager<TEntity,TOwner> where TEntity:Entity<TOwner> where TOwner:GameState {

        private const string NO_ITERATION_ALLOWED = "Cannot iterate an Entity list recursively!";
        private const string NO_MUTATION_ALLOWED = "Cannot modify Entity tables during immutable list iteration!";

        private readonly TOwner owner;

        private readonly EntityContainer<TEntity,TOwner> container = new EntityContainer<TEntity,TOwner>();

        public EntityManager(TOwner owner) {
            owner.OnUnload += Owner_Unload;
            this.owner = owner;
        }

        private int IDCounter = EntityManager.START_ID;
        private int GetNextID() => IDCounter++;

        private TEntity[] _entityList = new TEntity[0];
        private bool entityListQueued = false;

        public bool Locked { get; private set; } = false;
        public bool Paused { get; private set; } = false;
        public bool Iterating { get; private set; } = false;

        private void PauseChanges() {
            if(Paused) {
                return;
            }
            Paused = true;
        }
        private void ResumeChanges() {
            if(!Paused) {
                return;
            }
            Paused = false;
            if(entityListQueued) {
                TryUpdateList();
            }
        }

        private void AssertIteration() {
            if(Iterating) {
                throw new InvalidOperationException(NO_ITERATION_ALLOWED);
            }
        }
        private void AssertMutation() {
            if(Locked) {
                throw new InvalidOperationException(NO_MUTATION_ALLOWED);
            }
        }

        public TEntity Search(Func<TEntity,bool> predicate) {
            AssertIteration();
            Iterating = true;

            Locked = true;
            TEntity entity = null;
            for(var i = 0;i<_entityList.Length;i++) {
                var currentEntity = _entityList[i];
                if(!predicate.Invoke(currentEntity)) {
                    continue;
                }
                entity = currentEntity;
                break;
            }
            Locked = false;

            Iterating = false;
            return entity;
        }

        public TEntity Search<TData>(Func<TEntity,TData,bool> predicate,TData data) {
            AssertIteration();
            Iterating = true;

            Locked = true;
            TEntity entity = null;
            for(var i = 0;i<_entityList.Length;i++) {
                var currentEntity = _entityList[i];
                if(!predicate.Invoke(currentEntity,data)) {
                    continue;
                }
                entity = currentEntity;
                break;
            }
            Locked = false;

            Iterating = false;
            return entity;
        }

        public void IterateImmutable(Action<TEntity> action) {
            AssertIteration();
            Iterating = true;

            Locked = true;
            for(var i = 0;i<_entityList.Length;i++) {
                action(_entityList[i]);
            }
            Locked = false;

            Iterating = false;
        }

        public void IterateMutable(Action<TEntity> action) {
            AssertIteration();
            Iterating = true;

            PauseChanges();
            for(var i = 0;i<_entityList.Length;i++) {
                var entity = _entityList[i];
                if(entity.IsDeleted) {
                    continue;
                }
                action(_entityList[i]);
            }
            ResumeChanges();

            Iterating = false;
        }

        public void IterateImmutable<TData>(Action<TEntity,TData> action,TData data) {
            AssertIteration();
            Iterating = true;

            Locked = true;
            for(var i = 0;i<_entityList.Length;i++) {
                action(_entityList[i],data);
            }
            Locked = false;

            Iterating = false;
        }

        public void IterateMutable<TData>(Action<TEntity,TData> action,TData data) {
            AssertIteration();
            Iterating = true;

            PauseChanges();
            for(var i = 0;i<_entityList.Length;i++) {
                var entity = _entityList[i];
                if(entity.IsDeleted) {
                    continue;
                }
                action(_entityList[i],data);
            }
            ResumeChanges();

            Iterating = false;
        }

        private TEntity[] GetEntityList() {
            return container.IDs.Values.ToArray();
        }

        private void TryUpdateList() {
            if(Paused) {
                entityListQueued = true;
                return;
            }
            _entityList = GetEntityList();
            entityListQueued = false;
        }

        /* Non-asserted */
        private void _clearEntities() {
            var entityList = GetEntityList();
            for(var i = 0;i < entityList.Length;i++) {
                var entity = entityList[i];
                _removeEntity(entity);
            }
        }

        /* Non-asserted version */
        private void _addEntity(TEntity entity) {
            var ID = GetNextID();
            entity.Register(ID,owner);
            container.Writer.AddEntity(entity);
            TryUpdateList();
            entity.Load();
            entity.IsDeleted = false;
        }

        /* Non-asserted version */
        private void _removeEntity(TEntity entity) {
            container.Writer.RemoveEntity(entity);
            TryUpdateList();
            entity.Unload();
            entity.IsDeleted = true;
        }

        public TEntity Add(TEntity entity) {
            if(entity == null) {
                throw new ArgumentNullException("entity");
            }
            AssertMutation();
            _addEntity(entity);
            return entity;
        }
        public void Remove(TEntity entity) {
            if(entity == null) {
                throw new ArgumentNullException("entity");
            }
            AssertMutation();
            _removeEntity(entity);
        }

        public void Add(params TEntity[] entities) {
            AssertMutation();
            PauseChanges();
            foreach(var entity in entities) {
                _addEntity(entity);
            }
            ResumeChanges();
        }

        public void Remove(params TEntity[] entities) {
            AssertMutation();
            PauseChanges();
            foreach(var entity in entities) {
                _removeEntity(entity);
            }
            ResumeChanges();
        }

        private void Owner_Unload() {
            AssertMutation();
            PauseChanges();
            _clearEntities();
            ResumeChanges();
        }
    }
}
