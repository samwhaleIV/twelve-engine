using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine {
    public abstract class GameState:ISerializable {
        internal GameManager Game = null;

        internal event Action OnLoad;
        internal event Action OnUnload;

        internal virtual void Load() => OnLoad?.Invoke();
        internal virtual void Unload() => OnUnload?.Invoke();

        internal abstract void Update(GameTime gameTime);
        internal abstract void Draw(GameTime gameTime);

        public abstract void Export(SerialFrame frame);
        public abstract void Import(SerialFrame frame);
    }
}
