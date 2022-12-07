using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Game3D.Entity;
using TwelveEngine.Shell;

namespace TwelveEngine.EntitySystem {
    public abstract class Entity<TOwner> where TOwner:GameState {

        private const int DEFAULT_ID = EntitySystem.EntityManager.START_ID - 1;

        private string _name = null;
        private void SetName(string newName) {
            var oldName = _name;
            _name = newName;
            if(newName != oldName) {
                OnNameChanged?.Invoke(ID,oldName);
            }
        }
        public string Name {
            get => _name;
            set => SetName(value);
        }

        public bool HasName => _name != null;

        /* int: ID, string: NewName */
        internal event Action<int,string> OnNameChanged;
        public int ID { get; private set; }

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public GameManager Game { get; private set; }
        public TOwner Owner { get; private set; } = null;

        internal object EntityManager { get; private set; } = null;

        internal void Register(int ID,TOwner owner,object entityManager) {
            this.ID = ID;
            Owner = owner;
            Game = owner.Game;
            EntityManager = entityManager;
        }

        protected event Action OnLoad, OnUnload, OnRemove;

        private float depth = 0f;

        protected virtual float GetDepth() => depth;
        protected virtual void SetDepth(float value) => depth = value;

        public float Depth { get => GetDepth(); set => SetDepth(value); }

        internal void Load() {
            IsLoading = true;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;
        }

        internal void Unload() {
            OnUnload?.Invoke();
            ID = DEFAULT_ID;
            Owner = null;
            Game = null;
            IsLoaded = false;
        }

        internal void Remove() {
            OnRemove?.Invoke();
        }

        public bool IsVisible { get; set; } = true;

        public event Action<GameTime> OnUpdate, OnRender, OnPreRender;

        public void Update(GameTime gameTime) => OnUpdate?.Invoke(gameTime);

        public void PreRender(GameTime gameTime) {
            if(!IsVisible) {
                return;
            }
            OnPreRender?.Invoke(gameTime);
        }

        public void Render(GameTime gameTime) {
            if(!IsVisible) {
                return;
            }
            OnRender?.Invoke(gameTime);
        }

        public void Update(Entity3D entity,GameTime gameTime) => entity.Update(gameTime);
        public void PreRender(Entity3D entity,GameTime gameTime) => entity.PreRender(gameTime);
        public void Render(Entity3D entity,GameTime gameTime) => entity.Render(gameTime);
    }
}
