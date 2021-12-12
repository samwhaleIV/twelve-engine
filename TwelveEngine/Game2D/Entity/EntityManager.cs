using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine.Game2D {
    public sealed class EntityManager:ISerializable {

        private readonly GameManager game;
        private readonly Grid2D grid;

        private int IDCounter = 0;
        private int getNextID() => IDCounter++;

        private readonly Dictionary<int,Entity> entityDictionary;
        private readonly Dictionary<string,Entity> namedEntities;

        public EntityManager(Grid2D owner) {
            entityDictionary = new Dictionary<int,Entity>();
            namedEntities = new Dictionary<string,Entity>();

            game = owner.Game;
            grid = owner;
        }

        private bool updateListQueued = false, renderListQueued = false;

        internal event Action<IRenderable[]> OnRenderListChanged;
        internal event Action<IUpdateable[]> OnUpdateListChanged;

        private IRenderable[] getRenderList() => GetAllOfType<IRenderable>();
        private IUpdateable[] getUpdateList() => GetAllOfType<IUpdateable>();

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

        public Entity Get(string name) {
            Entity entity;
            if(namedEntities.TryGetValue(name,out entity)) {
                return entity;
            }
            return null;
        }
        public Entity[] GetAll() {
            return entityDictionary.Values.ToArray();
        }
        public T[] GetAllOfType<T>() {
            var queue = new Queue<T>();
            foreach(var entity in entityDictionary.Values) {
                if(entity is T typeCast) {
                    queue.Enqueue(typeCast);
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

        private void Entity_OnNameChanged(Entity source,string oldName) {
            string newName = source.Name;
            namedEntities.Remove(oldName);
            namedEntities.Add(newName,source);
        }
        private bool hasName(Entity entity) {
            return string.IsNullOrWhiteSpace(entity.Name);
        }
        private void removeNamedEntity(Entity entity) {
            namedEntities.Remove(entity.Name);
        }
        private void addNamedEntity(Entity entity) {
            namedEntities.Add(entity.Name,entity);
        }

        private void addToLists(Entity entity) {
            entityDictionary.Add(entity.ID,entity);
            if(hasName(entity)) addNamedEntity(entity);
            if(entity is IRenderable) renderListChanged();
            if(entity is IUpdateable) updateListChanged();
        }

        private void removeFromList(Entity entity) {
            entityDictionary.Remove(entity.ID);
            if(hasName(entity)) removeNamedEntity(entity);
            if(entity is IRenderable) renderListChanged();
            if(entity is IUpdateable) updateListChanged();
        }

        public void AddEntity(Entity entity) {
            if(entity.GetOwner() != null) return;

            entity.SetReferences(this,grid,game);

            var ID = getNextID();
            entity.ID = ID;

            addToLists(entity); //Stage 1
            entity.OnNameChanged += Entity_OnNameChanged; //Stage 2
            entity.Load(); //Stage 3
        }

        public void RemoveEntity(Entity entity) {
            if(entity.GetOwner() != this) {
                throw new ArgumentException("Entity does not beint to this EntityManager!","entity");
            }

            /* Stage 1 and 2 are in reverse order for entity removal */
            entity.OnNameChanged -= Entity_OnNameChanged; //Stage 1
            removeFromList(entity); //Stage 2
            entity.Unload(); //Stage 3

            entity.ClearReferences();
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

        private Entity[] getSerializableEntities() {
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
                var type = frame.GetEntityType();
                var entity = EntityFactory.GetEntity(type);
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
