using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {
        internal GameManager Game = null;

        internal event Action OnLoad;
        internal event Action OnUnload;

        internal event Action<SerialFrame> OnExport;
        internal event Action<SerialFrame> OnImport;

        internal void Load() => OnLoad?.Invoke();
        internal void Unload() => OnUnload?.Invoke();

        public void Export(SerialFrame frame) => OnExport?.Invoke(frame);
        public void Import(SerialFrame frame) => OnImport?.Invoke(frame);

        internal abstract void Update(GameTime gameTime);
        internal abstract void Draw(GameTime gameTime);
    }
}
