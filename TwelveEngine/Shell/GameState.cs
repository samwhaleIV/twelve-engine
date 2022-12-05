using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class GameState {

        public GameManager Game { get; private set; } = null;

        public event Action OnLoad, OnUnload;

        public event Action<DebugWriter> OnWriteDebug;
        public event Action<GameTime> OnUpdate, OnRender, OnPreRender;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public bool IsUpdating { get; private set; } = false;
        public bool IsRendering { get; private set; } = false;
        public bool IsPreRendering { get; private set; } = false;

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

        internal void WriteDebug(DebugWriter writer) => OnWriteDebug?.Invoke(writer);

        internal void Update(GameTime gameTime) {
            IsUpdating = true;
            OnUpdate?.Invoke(gameTime);
            IsUpdating = false;
        }

        internal void Render(GameTime gameTime) {
            IsRendering = true;
            OnRender?.Invoke(gameTime);
            IsRendering = false;
        }

        internal void PreRender(GameTime gameTime) {
            IsPreRendering = true;
            OnPreRender?.Invoke(gameTime);
            IsPreRendering = false;
        }
    }
}
