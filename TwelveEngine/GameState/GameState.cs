using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {
        internal void SetReferences(GameManager game) => this.game = game;

        private GameManager game = null;
        public GameManager Game => game;

        public event Action OnLoad, OnUnload;

        protected internal event Action<SerialFrame> OnExport, OnImport;

        protected internal event Action<GameTime> OnPreRender;

        private bool loaded = false;
        public bool IsLoaded => loaded;

        internal void Load() {
            OnLoad?.Invoke();
            loaded = true;
        }
        internal void Unload() {
            OnUnload?.Invoke();
        }

        public abstract void Render(GameTime gameTime);
        public abstract void Update(GameTime gameTime);

        public void PreRender(GameTime gameTime) => OnPreRender?.Invoke(gameTime);

        public void Export(SerialFrame frame) => OnExport?.Invoke(frame);
        public void Import(SerialFrame frame) => OnImport?.Invoke(frame);
    }
}
