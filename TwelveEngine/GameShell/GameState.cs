using System;
using Microsoft.Xna.Framework;
using TwelveEngine.GameShell;
using TwelveEngine.Input;
using TwelveEngine.Serial;

namespace TwelveEngine {
    public class GameState:ISerializable {

        private readonly TimeoutManager timeoutManager;
        public GameManager Game { get; private set; } = null;

        public GameState() {
            timeoutManager = new TimeoutManager();
            OnUpdate += gameTime => timeoutManager.Update(gameTime.TotalGameTime);
        }

        public bool ClearTimeout(int ID) {
            return timeoutManager.Remove(ID);
        }
        public int SetTimeout(Action action,TimeSpan delay) {
            return timeoutManager.Add(action,delay,Game.Time.TotalGameTime);
        }

        public ImpulseHandler Input {
            get {
                if(!IsLoaded && !IsLoading) {
                    throw new InvalidOperationException("Input cannot be accessed before loading.");
                }
                return Game.ImpulseHandler;
            }
        }

        public event Action OnLoad, OnUnload;

        public event Action<SerialFrame> OnExport, OnImport;
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
