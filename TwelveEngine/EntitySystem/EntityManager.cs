using System;
using System.Collections.Generic;
using System.Linq;
using TwelveEngine.Serial;

namespace TwelveEngine.EntitySystem {
    public static class EntityManager {
        internal const int START_ID = 0;
    }
    public sealed class EntityManager<TEntity,TOwner> where TEntity:Entity<TOwner> where TOwner:GameState {

        private const string NO_ITERATION_ALLOWED = "Cannot iterate an Entity list recursively!";
        private const string NO_MUTATION_ALLOWED = "Cannot modify Entity tables during immutable list iteration!";

        public EntityManager(TOwner owner,EntityFactory<TEntity,TOwner> factory) {
            owner.OnExport += Owner_Export;
            owner.OnImport += Owner_Import;
            owner.OnUnload += Owner_Unload;

            this.owner = owner;
            this.factory = factory;
        }

        private readonly TOwner owner;
        private readonly EntityFactory<TEntity,TOwner> factory;

        private int IDCounter = EntityManager.START_ID;
        private int getNextID() => IDCounter++;

        private readonly Dictionary<int,TEntity> entityDictionary = new Dictionary<int,TEntity>();
        private readonly Dictionary<string,TEntity> namedEntities = new Dictionary<string,TEntity>();

        /* Be warned: HashSet is unordered. Entites are not supposed to be dependent on creation/insertion order */
        private readonly Dictionary<int,HashSet<int>> typeTable = new Dictionary<int,HashSet<int>>(), componentTable = new Dictionary<int,HashSet<int>>();

        private TEntity[] entityList;
        private bool entityListQueued = false;

        public bool Locked { get; private set; } = false;
        public bool Paused { get; private set; } = false;
        public bool Iterating { get; private set; } = false;

        private void pauseChanges() {
            if(Paused) {
                return;
            }
            Paused = true;
        }
        private void resumeChanges() {
            if(!Paused) {
                return;
            }
            Paused = false;
            if(entityListQueued) {
                entityListChanged();
            }
        }

        private void assertIteration() {
            if(Iterating) {
                throw new InvalidOperationException(NO_ITERATION_ALLOWED);
            }
        }
        private void assertMutation() {
            if(Locked) {
                throw new InvalidOperationException(NO_MUTATION_ALLOWED);
            }
        }

        public void IterateImmutable(Action<TEntity> action) {
            assertIteration();
            Iterating = true;

            Locked = true;
            for(var i = 0;i<entityList.Length;i++) {
                action(entityList[i]);
            }
            Locked = false;

            Iterating = false;
        }

        public void IterateMutable(Action<TEntity> action) {
            assertIteration();
            Iterating = true;

            pauseChanges();
            for(var i = 0;i<entityList.Length;i++) {
                var entity = entityList[i];
                if(entity.Deleted) {
                    continue;
                }
                action(entityList[i]);
            }
            resumeChanges();

            Iterating = false;
        }


        #region COPY PASTA BULLSHIT

        /* These had to be copy pasted because we can't make a super duper generic TAction, but by God, I tried. */

        public void IterateImmutable<TData>(Action<TEntity,TData> action,TData data) {
            assertIteration();
            Iterating = true;

            Locked = true;
            for(var i = 0;i<entityList.Length;i++) {
                action(entityList[i],data);
            }
            Locked = false;

            Iterating = false;
        }

        public void IterateMutable<TData>(Action<TEntity,TData> action,TData data) {
            assertIteration();
            Iterating = true;

            pauseChanges();
            for(var i = 0;i<entityList.Length;i++) {
                var entity = entityList[i];
                if(entity.Deleted) {
                    continue;
                }
                action(entityList[i],data);
            }
            resumeChanges();

            Iterating = false;
        }
        #endregion

        public TEntity OfID(int ID) {
            if(!entityDictionary.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity;
        }
        public TEntity OfID<TType>(int ID) where TType:TEntity {
            if(!entityDictionary.TryGetValue(ID,out var entity)) {
                return null;
            }
            return entity as TType;
        }

        public TEntity OfName(string name) {
            if(!namedEntities.TryGetValue(name,out var entity)) {
                return null;
            }
            return entity;
        }
        public TType OfName<TType>(string name) where TType:TEntity {
            if(!namedEntities.TryGetValue(name,out var entity)) {
                return null;
            }
            return entity as TType;
        }

        public IEnumerable<TEntity> OfComponent(int componentType) {
            foreach(var ID in componentTable[componentType]) {
                yield return entityDictionary[ID];
            }
        }

        public IEnumerable<TEntity> OfComponents(HashSet<int> componentTypes) {
            if(componentTypes.Count == 0) {
                yield break;
            }
            foreach(var ID in componentTable[componentTypes.First()]) {
                var entity = entityDictionary[ID];
                if(!componentTypes.IsSubsetOf(entity.ComponentTypes)) {
                    continue;
                }
                yield return entity;
            }
        }

        public IEnumerable<TEntity> OfType(int entityType) {
            if(!typeTable.ContainsKey(entityType)) {
                yield break;
            }
            foreach(var ID in typeTable[entityType]) {
                yield return entityDictionary[ID];
            }
        }

        public IEnumerable<TType> OfType<TType>(int entityType) where TType:TEntity {
            if(!typeTable.ContainsKey(entityType)) {
                yield break;
            }
            foreach(var ID in typeTable[entityType]) {
                yield return entityDictionary[ID] as TType;
            }
        }

        private void entityListChanged() {
            if(Paused) {
                entityListQueued = true;
                return;
            }
            entityList = getEntityList();
            entityListQueued = false;
        }

        private void Entity_OnNameChanged(int ID,string oldName) {
            var source = entityDictionary[ID];
            string newName = source.Name;
            namedEntities.Remove(oldName);
            namedEntities.Add(newName,source);
        }

        private bool hasName(TEntity entity) {
            return !string.IsNullOrWhiteSpace(entity.Name);
        }
        private void removeNamedEntity(TEntity entity) {
            namedEntities.Remove(entity.Name);
        }
        private void addNamedEntity(TEntity entity) {
            namedEntities.Add(entity.Name,entity);
        }

        private void addToTable(int tableID,int hashID,Dictionary<int,HashSet<int>> table) {
            HashSet<int> hashSet;
            if(!typeTable.TryGetValue(tableID,out hashSet)) {
                hashSet = new HashSet<int>();
                typeTable[tableID] = hashSet;
            }
            hashSet.Add(hashID);
        }

        private void removeFromTable(int tableID,int hashID,Dictionary<int,HashSet<int>> table) {
            typeTable[tableID].Remove(hashID);
        }

        private void addToComponentTable(TEntity entity) {
            foreach(var componentType in entity.ComponentTypes) {
                addToTable(componentType,entity.ID,componentTable);
            }
        }

        private void removeFromComponentTable(TEntity entity) {
            foreach(var componentType in entity.ComponentTypes) {
                removeFromTable(componentType,entity.ID,componentTable);
            }
        }

        private void addToTypeTable(TEntity entity) {
            addToTable(entity.Type,entity.ID,typeTable);
        }

        private void removeFromTypeTable(TEntity entity) {
            removeFromTable(entity.Type,entity.ID,typeTable);
        }

        private void addToLists(TEntity entity) {
            entityDictionary.Add(entity.ID,entity);
            addToComponentTable(entity);
            addToTypeTable(entity);
            entity.Deleted = false;
            if(hasName(entity)) {
                addNamedEntity(entity);
            }
            entityListChanged();
        }

        private void removeFromLists(TEntity entity) {
            entityDictionary.Remove(entity.ID);
            removeFromComponentTable(entity);
            removeFromTypeTable(entity);
            entity.Deleted = true;
            if(hasName(entity)) {
                removeNamedEntity(entity);
            }
            entityListChanged();
        }

        private TEntity[] getEntityList() => entityDictionary.Values.ToArray();

        private void clearEntities(bool checkForStateLock = false) {
            var entityList = getEntityList();
            var entityCount = entityList.Length;
            for(var i = 0;i < entityCount;i++) {
                var entity = entityList[i];
                if(checkForStateLock && entity.StateLock) {
                    continue;
                }
                RemoveEntity(entity);
            }
        }

        private IEnumerable<TEntity> getSerializableEntities() {
            foreach(var entity in getEntityList()) {
                if(entity.StateLock) {
                    continue;
                }
                yield return entity;
            }
        }

        public void AddEntity(TEntity entity) {
            assertMutation();

            var ID = getNextID();
            entity.Register(ID,owner);

            addToLists(entity); //Stage 1
            entity.OnNameChanged += Entity_OnNameChanged; //Stage 2
            entity.OnComponentAdded += Entity_OnComponentAdded;
            entity.OnComponentRemoved += Entity_OnComponentRemoved;
            entity.Load(); //Stage 3
        }

        private void Entity_OnComponentRemoved(int ID,int componentType) {
            removeFromTable(componentType,ID,componentTable);
        }

        private void Entity_OnComponentAdded(int ID,int componentType) {
            addToTable(componentType,ID,componentTable);
        }

        public void AddEntities(params TEntity[] entities) {
            assertMutation();
            pauseChanges();
            foreach(var entity in entities) {
                AddEntity(entity);
            }
            resumeChanges();
        }

        public void RemoveEntity(TEntity entity) {
            assertMutation();
            /* Stage 1 and 2 are in reverse order for entity removal */
            entity.OnNameChanged -= Entity_OnNameChanged; //Stage 1
            entity.OnComponentAdded -= Entity_OnComponentAdded;
            entity.OnComponentRemoved -= Entity_OnComponentRemoved;
            removeFromLists(entity); //Stage 2
            entity.Unload(); //Stage 3
        }
        public void RemoveEntity(string name) => RemoveEntity(OfName(name));
        public void RemoveEntity(int ID) => RemoveEntity(entityDictionary[ID]);

        private void Owner_Unload() {
            assertMutation();
            pauseChanges();
            clearEntities();
            resumeChanges();
        }

        private void Owner_Export(SerialFrame frame) {
            var entityList = getSerializableEntities().ToArray();
            var entityCount = entityList.Length;
            frame.Set(entityList.Length);
            for(var i = 0;i<entityCount;i++) {
                var entity = entityList[i];
                frame.Set(entity.Type);
                frame.Set(entity);
            }
        }

        private void Owner_Import(SerialFrame frame) {
            assertMutation();

            pauseChanges();
            if(IDCounter > 0) {
                clearEntities(checkForStateLock: true);
            }

            int entityCount = frame.GetInt();

            for(var i = 0;i<entityCount;i++) {
                var type = frame.GetInt();
                var entity = factory.Create(type);
                frame.Get(entity);
                AddEntity(entity);
            }
            resumeChanges();
        }
    }
}
