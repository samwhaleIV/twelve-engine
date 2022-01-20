using System.Collections.Generic;

namespace TwelveEngine.EntitySystem {
    internal sealed class EntityContainerHandler<TEntity, TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        public EntityContainer<TEntity,TOwner> Container { get; set; }

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
                AddToTable(Container.Components,entity.ID,componentType);
            }
        }

        private void RemoveFromComponentTable(TEntity entity) {
            foreach(var componentType in entity.Components.Types) {
                RemoveFromTable(Container.Components,entity.ID,componentType);
            }
        }

        private void AddToTypeTable(TEntity entity) {
            AddToTable(Container.Types,entity.ID,entity.Type);
        }
        private void RemoveFromTypeTable(TEntity entity) {
            RemoveFromTable(Container.Types,entity.ID,entity.Type);
        }

        private void AddToLists(TEntity entity) {
            Container.IDs.Add(entity.ID,entity);
            if(entity.HasName) {
                Container.Names.Add(entity.Name,entity);
            }
            AddToTypeTable(entity);
            AddToComponentTable(entity);
        }
        private void RemoveFromLists(TEntity entity) {
            Container.IDs.Remove(entity.ID);
            if(entity.HasName) {
                Container.Names.Remove(entity.Name);
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
            AddToTable(Container.Components,entityID,componentType);
        }

        private void Entity_OnComponentRemoved(int entityID,int componentType) {
            RemoveFromTable(Container.Components,entityID,componentType);
        }

        private void Entity_OnNameChanged(int entityID,string oldName) {
            var entity = Container.IDs[entityID];
            Container.Names.Remove(oldName);
            Container.Names[entity.Name] = entity;
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
