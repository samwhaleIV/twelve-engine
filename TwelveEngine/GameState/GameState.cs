using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {
        internal void SetReferences(GameManager game) => Game = game;

        public GameManager Game { get; private set; } = null;

        public event Action OnLoad, OnUnload;

        public Action<SerialFrame> OnExport, OnImport;
        protected event Action<GameTime> OnPreRender;

        public bool IsLoaded { get; private set; } = false;

        internal void Load() {
            OnLoad?.Invoke();
            IsLoaded = true;
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
