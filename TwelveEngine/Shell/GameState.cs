using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class GameState {

        private StateData data;
        public StateData Data {
            get {
                if(!(IsLoaded || IsLoading)) {
                    /* If you need to access data before a 'Load' call, don't use 'Data.Args'; Use regular parameters in the state constructor */
                    throw new InvalidOperationException("Cannot access 'Data' before the state has started loading!");
                }
                return data;
            }
            internal set {
                data = value;
            }
        }

        public bool HasFlag(StateFlags flag) {
            return Data.Flags.HasFlag(flag);
        }

        public bool FadeInIsFlagged {
            get {
                return Data.Flags.HasFlag(StateFlags.FadeIn);
            }
        }

        public GameManager Game { get; private set; } = null;
        public ContentManager Content => Game?.Content;

        public TimeSpan Now => Game?.Time.TotalGameTime ?? TimeSpan.Zero;
        public GameTime Time => Game?.Time;

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

        private TimeSpan startTime;

        public TimeSpan StartTime {
            get {
                if(!hasStartTime) {
                    throw new InvalidOperationException("Property 'StartTime' cannot be evaluated before the first 'Update' frame.");
                }
                return startTime;
            }
            private set {
                startTime = value;
            }
        }

        public string Name { get; set; } = string.Empty;

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

        private bool hasStartTime = false;

        private void UpdateStartTime() {
            TimeSpan now = Now;
            transitionStartTime = now;
            StartTime = now;
            hasStartTime = true;
        }

        private void HandleTransitionOut(TransitionData data) {
            if(data.Generator != null) {
                Game.SetState(data.Generator,data.Data);
            } else {
                Game.SetState(data.State,data.Data);
            }
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
            if(TransitionState == TransitionState.Out && transitionOutData.HasValue) {
                HandleTransitionOut(transitionOutData.Value);
                transitionOutData = null;
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

        private TransitionData? transitionOutData = null;

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

        public void TransitionOut(TransitionData transitionData) {
            if(transitionData.Generator != null && transitionData.State != null) {
                throw new InvalidOperationException("Transition out data cannot contain two game state values.");
            }
            TransitionState = TransitionState.Out;
            transitionStartTime = Now;
            transitionDuration = transitionData.Duration;
            transitionOutData = transitionData;
        }

        public void TransitionIn(TimeSpan duration) {
            TransitionState = TransitionState.In;
            transitionDuration = duration;
            transitionOutData = null;
        }

        public Color ClearColor { get; set; } = Color.Black;

        public virtual void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
            Game.GraphicsDevice.Clear(ClearColor);
        }

    }
}
