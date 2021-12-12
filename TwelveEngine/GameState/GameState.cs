using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {
        private GameManager game = null;
        public GameManager Game => game;
        internal void SetGameReference(GameManager game) => this.game = game;

        public event Action OnLoad;
        public event Action OnUnload;

        protected internal event Action<SerialFrame> OnExport;
        protected internal event Action<SerialFrame> OnImport;

        private bool loaded = false;
        public bool IsLoaded => loaded;

        internal void Load() {
            OnLoad?.Invoke();
            loaded = true;
        }
        internal void Unload() => OnUnload?.Invoke();

        public void Export(SerialFrame frame) => OnExport?.Invoke(frame);
        public void Import(SerialFrame frame) => OnImport?.Invoke(frame);

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime);
    }
}
