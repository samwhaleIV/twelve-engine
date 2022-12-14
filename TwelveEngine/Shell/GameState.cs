using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class GameState {

        public GameManager Game { get; private set; } = null;

        public TimeSpan Now => Game?.Time.TotalGameTime ?? TimeSpan.Zero;
        public GameTime Time => Game?.Time ?? null;

        public event Action OnLoad, OnUnload;

        public event Action<DebugWriter> OnWriteDebug;
        public event Action OnUpdate, OnRender, OnPreRender;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public bool IsUpdatingUI { get; private set; } = false;

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

        internal void WriteDebug(DebugWriter writer) {
            OnWriteDebug?.Invoke(writer);
        }

        internal void Update() {
            IsUpdating = true;
            OnUpdate?.Invoke();
            IsUpdating = false;
        }

        internal void Render() {
            IsRendering = true;
            OnRender?.Invoke();
            IsRendering = false;
        }

        internal void PreRender() {
            IsPreRendering = true;
            OnPreRender?.Invoke();
            IsPreRendering = false;
        }

        protected internal virtual void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
        }
    }
}
