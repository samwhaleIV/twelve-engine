using System.Collections.Generic;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem.EntityContainer {
    internal sealed class ContainerWriter<TEntity,TOwner> where TEntity : Entity<TOwner> where TOwner : GameState {

        private readonly EntityContainer<TEntity,TOwner> container;
        public ContainerWriter(EntityContainer<TEntity,TOwner> container) {
            this.container = container;
        }

        private static void AddToTable(Dictionary<int,HashSet<int>> table,int entityID,int tableID) {
            if(!table.TryGetValue(tableID,out var hashSet)) {
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
        }
        private void RemoveFromLists(TEntity entity) {
            container.IDs.Remove(entity.ID);
            if(entity.HasName) {
                container.Names.Remove(entity.Name);
            }
            RemoveFromTypeTable(entity);
        }

        private void Entity_OnNameChanged(int entityID,string oldName) {
            var entity = container.IDs[entityID];
            container.Names.Remove(oldName);
            container.Names[entity.Name] = entity;
        }

        internal void AddEntity(TEntity entity) {
            AddToLists(entity);
            entity.OnNameChanged += Entity_OnNameChanged;
        }

        internal void RemoveEntity(TEntity entity) {
            entity.OnNameChanged -= Entity_OnNameChanged;
            RemoveFromLists(entity);
        }
    }
}
