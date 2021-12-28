using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine.EntitySystem {
    public sealed class EntityManager<T,OwnerType>:ISerializable where T:Entity<OwnerType> where OwnerType:class {

        public EntityManager(GameManager game,OwnerType owner,EntityFactory<T,OwnerType> factory) {
            entityDictionary = new Dictionary<int,T>();
            namedEntities = new Dictionary<string,T>();

            this.game = game;
            this.owner = owner;
            this.factory = factory;
        }

        private readonly GameManager game;
        private readonly OwnerType owner;
        private readonly EntityFactory<T,OwnerType> factory;

        private int IDCounter = 0;
        private int getNextID() => IDCounter++;

        private readonly Dictionary<int,T> entityDictionary;
        private readonly Dictionary<string,T> namedEntities;

        private bool updateListQueued = false, renderListQueued = false;

        internal event Action<IRenderable[]> OnRenderListChanged;
        internal event Action<IUpdateable[]> OnUpdateListChanged;

        public Type[] GetAllOfType<Type>() {
            var queue = new Queue<Type>();
            foreach(var entity in entityDictionary.Values) {
                if(entity is Type typeCast) {
                    queue.Enqueue(typeCast);
                }
            }
            return queue.ToArray();
        }

        private IRenderable[] getRenderList() {
            return GetAllOfType<IRenderable>();
        }
        private IUpdateable[] getUpdateList() {
            return GetAllOfType<IUpdateable>();
        }

        private bool listChangesPaused = false;
        public bool Paused => listChangesPaused;
        
        public void PauseListChanges() {
            if(listChangesPaused) {
                return;
            }
            listChangesPaused = true;
        }
        public void ResumeListChanges() {
            if(!listChangesPaused) {
                return;
            }
            listChangesPaused = false;
            if(renderListQueued) {
                renderListChanged();
            }
            if(updateListQueued) {
                updateListChanged();
            }
        }

        public T Get(string name) {
            T entity;
            if(namedEntities.TryGetValue(name,out entity)) {
                return entity;
            }
            return null;
        }
        public T[] GetAll() {
            return entityDictionary.Values.ToArray();
        }
        public T[] GetAllOfType(int type) {
            var queue = new Queue<T>();
            foreach(var entity in entityDictionary.Values) {
                if(entity.Type == type) {
                    queue.Enqueue(entity);
                }
            }
            return queue.ToArray();
        }

        private void renderListChanged() {
            if(listChangesPaused) {
                renderListQueued = true;
                return;
            }
            OnRenderListChanged?.Invoke(getRenderList());
            renderListQueued = false;
        }
        private void updateListChanged() {
            if(listChangesPaused) {
                updateListQueued = true;
                return;
            }
            OnUpdateListChanged?.Invoke(getUpdateList());
            updateListQueued = false;
        }

        private void Entity_OnNameChanged(int ID,string oldName) {
            var source = entityDictionary[ID];
            string newName = source.Name;
            namedEntities.Remove(oldName);
            namedEntities.Add(newName,source);
        }
        private bool hasName(T entity) {
            return string.IsNullOrWhiteSpace(entity.Name);
        }
        private void removeNamedEntity(T entity) {
            namedEntities.Remove(entity.Name);
        }
        private void addNamedEntity(T entity) {
            namedEntities.Add(entity.Name,entity);
        }

        private void addToLists(T entity) {
            entityDictionary.Add(entity.ID,entity);
            if(hasName(entity)) addNamedEntity(entity);
            if(entity is IRenderable) renderListChanged();
            if(entity is IUpdateable) updateListChanged();
        }

        private void removeFromList(T entity) {
            entityDictionary.Remove(entity.ID);
            if(hasName(entity)) removeNamedEntity(entity);
            if(entity is IRenderable) renderListChanged();
            if(entity is IUpdateable) updateListChanged();
        }

        public void AddEntity(T entity) {
            var ID = getNextID();
            entity.Register(ID,game,owner);

            addToLists(entity); //Stage 1
            entity.OnNameChanged += Entity_OnNameChanged; //Stage 2
            entity.Load(); //Stage 3
        }

        public void RemoveEntity(T entity) {
            /* Stage 1 and 2 are in reverse order for entity removal */
            entity.OnNameChanged -= Entity_OnNameChanged; //Stage 1
            removeFromList(entity); //Stage 2
            entity.Unload(); //Stage 3
        }

        public void Unload() => clearEntities();

        private void clearEntities(bool checkForStateLock = false) {
            bool startedPaused = Paused;
            PauseListChanges();
            var entities = entityDictionary.Values.ToArray();
            for(var i = 0;i < entities.Length;i++) {
                var entity = entities[i];
                if(checkForStateLock && entity.StateLock) {
                    continue;
                }
                RemoveEntity(entity);
            }
            if(startedPaused) {
                return;
            }
            ResumeListChanges();
        }

        private T[] getSerializableEntities() {
            return entityDictionary.Values.Where(x => !x.StateLock).ToArray();
        }

        public void Export(SerialFrame frame) {
            var entities = getSerializableEntities();
            var entityCount = entities.Length;
            frame.Set(entityCount);
            for(var i = 0;i<entityCount;i++) {
                var entity = entities[i];
                frame.Set(entity.Type);
                frame.Set(entity);
            }
        }

        public void Import(SerialFrame frame) {
            bool startedPaused = Paused;
            PauseListChanges();

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

            if(startedPaused) {
                return;
            }
            ResumeListChanges();
        }
    }
}
