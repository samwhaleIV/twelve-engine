using System.Collections.Generic;

namespace TwelveEngine.EntitySystem.EntityContainer {
    internal sealed class ContainerWriter<TEntity, TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        private readonly EntityContainer<TEntity,TOwner> container;
        public ContainerWriter(EntityContainer<TEntity,TOwner> container) {
            this.container = container;
        }

        private static void AddToTable(Dictionary<int,HashSet<int>> table,int entityID,int tableID) {
            HashSet<int> hashSet;
            if(!table.TryGetValue(tableID,out hashSet)) {
                hashSet = new HashSet<int>();
                table[tableID] = hashSet;
            }
            hashSet.Add(entityID);
        }

        private static void RemoveFromTable(Dictionary<int,HashSet<int>> table,int entityID,int tableID) {
            if(!table.ContainsKey(tableID)) {
                return;
            }
            table[tableID].Remove(entityID);
        }

        private void AddToComponentTable(TEntity entity) {
            foreach(var componentType in entity.Components.Types) {
                AddToTable(container.Components,entity.ID,componentType);
            }
        }

        private void RemoveFromComponentTable(TEntity entity) {
            foreach(var componentType in entity.Components.Types) {
                RemoveFromTable(container.Components,entity.ID,componentType);
            }
        }

        private void AddToTypeTable(TEntity entity) {
            AddToTable(container.Types,entity.ID,entity.Type);
        }
        private void RemoveFromTypeTable(TEntity entity) {
            RemoveFromTable(container.Types,entity.ID,entity.Type);
        }

        private void AddToLists(TEntity entity) {
            container.IDs.Add(entity.ID,entity);
            if(entity.HasName) {
                container.Names.Add(entity.Name,entity);
            }
            AddToTypeTable(entity);
            AddToComponentTable(entity);
        }
        private void RemoveFromLists(TEntity entity) {
            container.IDs.Remove(entity.ID);
            if(entity.HasName) {
                container.Names.Remove(entity.Name);
            }
            RemoveFromTypeTable(entity);
            RemoveFromComponentTable(entity);
        }

        private void AddEventHandlers(TEntity entity) {
            entity.OnNameChanged += Entity_OnNameChanged;
            entity.OnComponentAdded += Entity_OnComponentAdded;
            entity.OnComponentRemoved += Entity_OnComponentRemoved;
        }

        private void RemoveEventHandlers(TEntity entity) {
            entity.OnNameChanged -= Entity_OnNameChanged;
            entity.OnComponentAdded -= Entity_OnComponentAdded;
            entity.OnComponentRemoved -= Entity_OnComponentRemoved;
        }

        private void Entity_OnComponentAdded(int entityID,int componentType) {
            AddToTable(container.Components,entityID,componentType);
        }

        private void Entity_OnComponentRemoved(int entityID,int componentType) {
            RemoveFromTable(container.Components,entityID,componentType);
        }

        private void Entity_OnNameChanged(int entityID,string oldName) {
            var entity = container.IDs[entityID];
            container.Names.Remove(oldName);
            container.Names[entity.Name] = entity;
        }

        internal void AddEntity(TEntity entity) {
            AddToLists(entity);
            AddEventHandlers(entity);
        }

        internal void RemoveEntity(TEntity entity) {
            RemoveEventHandlers(entity);
            RemoveFromLists(entity);
        }
    }
}
