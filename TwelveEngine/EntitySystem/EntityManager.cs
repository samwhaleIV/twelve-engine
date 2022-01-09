using System.Collections.Generic;
using System.Linq;
using TwelveEngine.Serial;

namespace TwelveEngine.EntitySystem {
    public sealed class EntityManager<TEntity,TOwner>:ISerializable where TEntity:Entity<TOwner> where TOwner:GameState {

        public EntityManager(TOwner owner,EntityFactory<TEntity,TOwner> factory) {
            entityDictionary = new Dictionary<int,TEntity>();
            namedEntities = new Dictionary<string,TEntity>();

            this.owner = owner;
            this.factory = factory;
        }

        private readonly TOwner owner;
        private readonly EntityFactory<TEntity,TOwner> factory;

        private int IDCounter = 0;
        private int getNextID() => IDCounter++;

        private readonly Dictionary<int,TEntity> entityDictionary;
        private readonly Dictionary<string,TEntity> namedEntities;

        private bool updateListQueued = false, renderListQueued = false;

        public IRenderable[] RenderList { get; private set; }
        public IUpdateable[] UpdateList { get; private set; }

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

        public TEntity Get(string name) {
            TEntity entity;
            if(namedEntities.TryGetValue(name,out entity)) {
                return entity;
            }
            return null;
        }
        public TEntity[] GetAll() {
            return entityDictionary.Values.ToArray();
        }
        public TEntity[] GetAllOfType(int type) {
            var queue = new Queue<TEntity>();
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
            RenderList = getRenderList();
            renderListQueued = false;
        }
        private void updateListChanged() {
            if(listChangesPaused) {
                updateListQueued = true;
                return;
            }
            UpdateList = getUpdateList();
            updateListQueued = false;
        }

        private void Entity_OnNameChanged(int ID,string oldName) {
            var source = entityDictionary[ID];
            string newName = source.Name;
            namedEntities.Remove(oldName);
            namedEntities.Add(newName,source);
        }
        private bool hasName(TEntity entity) {
            return string.IsNullOrWhiteSpace(entity.Name);
        }
        private void removeNamedEntity(TEntity entity) {
            namedEntities.Remove(entity.Name);
        }
        private void addNamedEntity(TEntity entity) {
            namedEntities.Add(entity.Name,entity);
        }

        private void addToLists(TEntity entity) {
            entityDictionary.Add(entity.ID,entity);
            if(hasName(entity)) addNamedEntity(entity);
            if(entity is IRenderable) renderListChanged();
            if(entity is IUpdateable) updateListChanged();
        }

        private void removeFromList(TEntity entity) {
            entityDictionary.Remove(entity.ID);
            if(hasName(entity)) removeNamedEntity(entity);
            if(entity is IRenderable) renderListChanged();
            if(entity is IUpdateable) updateListChanged();
        }

        public void AddEntity(TEntity entity) {
            var ID = getNextID();
            entity.Register(ID,owner);

            addToLists(entity); //Stage 1
            entity.OnNameChanged += Entity_OnNameChanged; //Stage 2
            entity.Load(); //Stage 3
        }

        public void RemoveEntity(TEntity entity) {
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

        private TEntity[] getSerializableEntities() {
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
