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

        public TransitionRenderer TransitionRenderer = TransitionRenderer.Default;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public bool IsUpdatingUI { get; private set; } = false;

        public bool IsUpdating { get; private set; } = false;
        public bool IsRendering { get; private set; } = false;
        public bool IsPreRendering { get; private set; } = false;

        public TimeSpan StartTime { get; private set; }

        public string Name { get; set; } = string.Empty;

        internal void Load(GameManager game) {
            game.CursorState = CursorState.Default;
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

        private bool hasStartTime = false;

        private void UpdateStartTime() {
            var now = Now;
            transitionStartTime = now;
            StartTime = now;
            hasStartTime = true;
        }

        internal void Update() {
            if(!hasStartTime) {
                UpdateStartTime();
            }
            IsUpdating = true;
            OnUpdate?.Invoke();
            IsUpdating = false;
            if(TransitionState == TransitionState.None) {
                return;
            }
            if(TransitionT < 1f) {
                return;
            }
            if(TransitionState == TransitionState.Out) {
                Game.SetState(newGameState);
                newGameState = null;
            }
            TransitionState = TransitionState.None;
        }

        internal void Render() {
            IsRendering = true;
            OnRender?.Invoke();
            if(TransitionState == TransitionState.Out) {
                TransitionRenderer.DrawOut(Game,TransitionT);
            } else if(TransitionState == TransitionState.In) {
                TransitionRenderer.DrawIn(Game,TransitionT);
            }
            IsRendering = false;
        }

        internal void PreRender() {
            IsPreRendering = true;
            OnPreRender?.Invoke();
            IsPreRendering = false;
        }

        public TransitionState TransitionState { get; private set; }

        public bool IsTransitioning => TransitionState != TransitionState.None;

        private TimeSpan transitionStartTime = TimeSpan.Zero;
        private TimeSpan transitionDuration = TimeSpan.Zero;

        private GameState newGameState = null;

        public float TransitionT {
            get {
                if(!IsTransitioning) {
                    return 0;
                }
                float t = (float)((Now - transitionStartTime) / transitionDuration);
                if(t < 0f) {
                    return t;
                } else if(t > 1f) {
                    return 1f;
                }
                return t;
            }
        }

        public void TransitionOut(GameState gameState,TimeSpan duration) {
            TransitionState = TransitionState.Out;
            transitionStartTime = Now;
            transitionDuration = duration;
            newGameState = gameState;
        }

        public void TransitionIn(TimeSpan duration) {
            TransitionState = TransitionState.In;
            transitionDuration = duration;
            newGameState = null;
        }

        public virtual void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.Default;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
        }

    }
}
