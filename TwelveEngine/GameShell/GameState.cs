using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;
using TwelveEngine.Serial;

namespace TwelveEngine {
    public class GameState:ISerializable {
        public GameManager Game { get; private set; } = null;

        public ImpulseHandler Input {
            get {
                if(!IsLoaded && !IsLoading) {
                    throw new InvalidOperationException("Input cannot be accessed before loading.");
                }
                return Game.ImpulseHandler;
            }
        }

        public event Action OnLoad, OnUnload;

        public Action<SerialFrame> OnExport, OnImport;
        public event Action<GameTime> OnUpdate, OnRender, OnPreRender;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        internal void Load(GameManager game) {
            IsLoading = true;
            Game = game;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;
        }

        internal void Unload() {
            OnUnload?.Invoke();
            Game = null;
            IsLoaded = false;
        }

        internal void Update(GameTime gameTime) => OnUpdate?.Invoke(gameTime);
        internal void Render(GameTime gameTime) => OnRender?.Invoke(gameTime);
        internal void PreRender(GameTime gameTime) => OnPreRender?.Invoke(gameTime);

        public void Export(SerialFrame frame) => OnExport?.Invoke(frame);
        public void Import(SerialFrame frame) => OnImport?.Invoke(frame);
    }
}
