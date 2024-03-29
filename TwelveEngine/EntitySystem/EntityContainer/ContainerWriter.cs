﻿using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem.EntityContainer {
    internal sealed class ContainerWriter<TEntity,TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        private readonly EntityContainer<TEntity,TOwner> container;
        public ContainerWriter(EntityContainer<TEntity,TOwner> container) {
            this.container = container;
        }

        private void AddToLists(TEntity entity) {
            container.IDs.Add(entity.ID,entity);
            if(entity.HasName) {
                container.Names.Add(entity.Name,entity);
            }
        }

        private void RemoveFromLists(TEntity entity) {
            container.IDs.Remove(entity.ID);
            if(entity.HasName) {
                container.Names.Remove(entity.Name);
            }
        }

        private void EntityNamedChanged(int entityID,string oldName) {
            var entity = container.IDs[entityID];
            if(oldName != null) {
                container.Names.Remove(oldName);
            }
            if(entity.HasName) {
                container.Names[entity.Name] = entity;
            }
        }

        internal void AddEntity(TEntity entity) {
            AddToLists(entity);
            entity.OnNameChanged += EntityNamedChanged;
        }

        internal void RemoveEntity(TEntity entity) {
            RemoveFromLists(entity);
            entity.OnNameChanged -= EntityNamedChanged;
        }
    }
}
