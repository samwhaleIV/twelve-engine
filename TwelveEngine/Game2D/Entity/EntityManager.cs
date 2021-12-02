using System;
using System.Collections.Generic;
using System.Linq;

namespace TwelveEngine.Game2D {
    public sealed class EntityManager:ISerializable {
        public GameManager Game { get; set; } = null;
        public Grid2D Grid { get; set; } = null;

        public EntityManager(Grid2D owner) {
            this.Game = owner.Game;
            this.Grid = owner;
        }
        private readonly Dictionary<long,IRenderable> renderList = new Dictionary<long,IRenderable>();
        private readonly Dictionary<long,IUpdateable> updateList = new Dictionary<long,IUpdateable>();
        private readonly Dictionary<long,Entity> entities = new Dictionary<long,Entity>();

        private readonly Dictionary<string,Dictionary<long,Entity>> nameList = new Dictionary<string,Dictionary<long, Entity>>();

        private long IDCounter = 0;
        private long getNextID() {
            return IDCounter++;
        }

        private Action<IRenderable[]> renderListChangedAction = null;
        public Action<IRenderable[]> RenderListChanged {
            get {
                return renderListChangedAction;
            }
            set {
                renderListChangedAction = value;
                renderListChanged();
            }
        }

        private Action<IUpdateable[]> updateListChangedAction = null;
        public Action<IUpdateable[]> UpdateListChanged {
            get {
                return updateListChangedAction;
            }
            set {
                updateListChangedAction = value;
                updateListChanged();
            }
        }

        private bool listChangesPaused = false;
        private bool needsToSendUpdateList = false;
        private bool needsToSendRenderList = false;

        public void ResumeListChanges() {
            if(!listChangesPaused) {
                return;
            }
            listChangesPaused = false;
            if(needsToSendRenderList) {
                needsToSendRenderList = false;
                renderListChanged();
            }
            if(needsToSendUpdateList) {
                needsToSendUpdateList = false;
                updateListChanged();
            }
        }
        public void PauseListChanges() {
            listChangesPaused = true;
        }

        public bool Paused {
            get => listChangesPaused;
            set {
                if(value) {
                    PauseListChanges();
                } else {
                    ResumeListChanges();
                }
            }
        }

        private void renderListChanged() {
            if(listChangesPaused) {
                needsToSendRenderList = true;
                return;
            }
            if(RenderListChanged != null) {
                RenderListChanged(renderList.Values.ToArray());
                needsToSendRenderList = false;
            } else {
                needsToSendRenderList = true;
            }
        }
        private void updateListChanged() {
            if(listChangesPaused) {
                needsToSendUpdateList = true;
                return;
            }
            if(UpdateListChanged != null) {
                UpdateListChanged(updateList.Values.ToArray());
                needsToSendUpdateList = false;
            } else {
                needsToSendUpdateList = true;
            }
        }

        private void addToNamedList(Entity entity) {
            Dictionary<long,Entity> entities;
            if(nameList.ContainsKey(entity.Name)) {
                entities = nameList[entity.Name];
            } else {
                entities = new Dictionary<long,Entity>();
                nameList[entity.Name] = entities;
            }
            entities[entity.ID] = entity;

        }
        private void removeFromNamedList(Entity entity) {
            var nameGroup = nameList[entity.Name];
            nameGroup.Remove(entity.ID);
            if(nameGroup.Keys.Count < 1) {
                nameList.Remove(entity.Name);
            }
        }

        public Entity GetEntity(string name) {
            return nameList[name].Values.First();
        }
        public Entity[] GetAllEntities(string name) {
            return nameList[name].Values.ToArray();
        }

        private bool hasName(Entity entity) {
            return string.IsNullOrWhiteSpace(entity.Name);
        }

        private void addToLists(Entity entity) {
            long ID = entity.ID;

            bool updateRenderList = false;
            bool updateUpdateList = false;

            if(entity is IRenderable renderable) {
                renderList[ID] = renderable;
                updateRenderList = true;
            }
            if(entity is IUpdateable updateable) {
                updateList[ID] = updateable;
                updateUpdateList = true;
            }
            entities[ID] = entity;

            if(hasName(entity)) addToNamedList(entity);

            if(updateRenderList) {
                renderListChanged();
            }
            if(updateUpdateList) {
                updateListChanged();
            }
        }

        private void removeFromLists(Entity entity) {
            long ID = entity.ID;

            bool updateRenderList = false;
            bool updateUpdateList = false;

            if(entity is IRenderable) {
                renderList.Remove(ID);
                updateRenderList = true;
            }
            if(entity is IUpdateable) {
                updateList.Remove(ID);
                updateUpdateList = true;
            }
            entities.Remove(ID);
            
            if(hasName(entity)) removeFromNamedList(entity);

            if(updateRenderList) {
                renderListChanged();
            }
            if(updateUpdateList) {
                updateListChanged();
            }
        }

        public void AddEntity(Entity entity) {
            if(entity.Owner != null) return;

            entity.Owner = this;
            entity.Game = Game;
            entity.Grid = Grid;

            long ID = getNextID();
            entity.ID = ID;

            addToLists(entity);

            entity.Load();
        }
        public void RemoveEntity(Entity entity) {
            if(entity.Owner != this) {
                throw new ArgumentException("Entity does not belong to this EntityManager!","entity");
            }
            removeFromLists(entity);
            entity.Unload();

            entity.Owner = null;
            entity.Game = null;
            entity.Grid = null;
        }

        public void Unload() {
            clearEntities();
            Game = null;
            Grid = null;
        }

        private void clearEntities(bool checkForStateLock = false) {
            var entities = this.entities.Values.ToArray();
            for(var i = 0;i < entities.Length;i++) {
                var entity = entities[i];
                if(checkForStateLock && entity.StateLock) {
                    continue;
                }
                RemoveEntity(entity);
            }
        }

        public void Export(SerialFrame frame) {
            var entities = this.entities.Values.ToArray();

            var addressOffset = 0;
            for(var i = 0;i<entities.Length;i++) {
                var entity = entities[i];
                if(entity.StateLock) {
                    addressOffset--;
                    continue;
                }

                string addressBase = $"Entity-{i+addressOffset}";

                frame.Set($"{addressBase}-Name",entity.Name);
                frame.Set($"{addressBase}-ID",entity.FactoryID);
                frame.Set(addressBase,entity);
            }
            frame.Set("EntityCount",entities.Length + addressOffset);
        }

        public void Import(SerialFrame frame) {
            bool startedPaused = Paused;
            Paused = true;

            if(IDCounter > 0) {
                clearEntities(checkForStateLock: true);
            }

            long entityCount = frame.GetInt("EntityCount");

            for(var i = 0;i<entityCount;i++) {
                string addressBase = $"Entity-{i}";

                var name = frame.GetString($"{addressBase}-Name");
                var factoryID = frame.GetString($"{addressBase}-ID");
                var entity = EntityFactory.GetEntity(factoryID);

                entity.Name = name;
                entity.FactoryID = factoryID;
                frame.Get(addressBase,entity);

                AddEntity(entity);
            }

            if(startedPaused) {
                return;
            }
            Paused = false;
        }
    }
}
