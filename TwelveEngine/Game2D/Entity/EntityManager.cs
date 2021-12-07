using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine.Game2D {
    public sealed class EntityManager:ISerializable {

        private readonly GameManager game;
        private readonly Grid2D grid;

        public GameManager Game => game;
        public Grid2D Grid => grid;

        private int IDCounter = 0;
        private int getNextID() => IDCounter++;

        private readonly Dictionary<int,Entity> entityDictionary;
        private readonly Dictionary<string,Entity> namedEntities;

        public EntityManager(Grid2D owner) {
            game = owner.Game;
            grid = owner;
            entityDictionary = new Dictionary<int,Entity>();
            namedEntities = new Dictionary<string,Entity>();
        }

        private bool updateListQueued = false, renderListQueued = false;

        internal event Action<IRenderable[]> OnRenderListChanged;
        internal event Action<IUpdateable[]> OnUpdateListChanged;

        private bool listChangesPaused = false;
        private void pauseListChanges() {
            listChangesPaused = true;
        }

        private void resumeListChanges() {
            listChangesPaused = false;
            if(renderListQueued) {
                renderListChanged();
            }
            if(updateListQueued) {
                updateListChanged();
            }
        }

        public bool Paused {
            get => listChangesPaused;
            set {
                if(value == listChangesPaused) {
                    return;
                } else if(value) {
                    pauseListChanges();
                } else {
                    resumeListChanges();
                }
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
            foreach(var entity in namedEntities.Values) {
                if(entity is T typeCast) {
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
            if(entity.Owner != null) return;

            entity.Owner = this; entity.Game = Game; entity.Grid = Grid;

            var ID = getNextID();
            entity.ID = ID;

            addToLists(entity); //Stage 1
            entity.OnNameChanged += Entity_OnNameChanged; //Stage 2
            entity.Load(); //Stage 3
        }

        public void RemoveEntity(Entity entity) {
            if(entity.Owner != this) {
                throw new ArgumentException("Entity does not beint to this EntityManager!","entity");
            }

            /* Stage 1 and 2 are in reverse order for entity removal */
            entity.OnNameChanged -= Entity_OnNameChanged; //Stage 1
            removeFromList(entity); //Stage 2
            entity.Unload(); //Stage 3

            entity.Owner = null; entity.Game = null; entity.Grid = null;
        }

        public void Unload() => clearEntities();

        private void clearEntities(bool checkForStateLock = false) {
            bool startedPaused = Paused;
            Paused = true;
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
            Paused = false;
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
            Paused = true;

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
            Paused = false;
        }
    }
}
