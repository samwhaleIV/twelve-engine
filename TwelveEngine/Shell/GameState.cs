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

        public bool HasFlag(StateFlags flag) => Data.Flags.HasFlag(flag);
        public bool FadeInIsFlagged => Data.Flags.HasFlag(StateFlags.FadeIn);

        public GameManager Game { get; private set; } = null;
        public ContentManager Content => Game.Content;

        private TimeSpan _nowOffset, _realTimeOffset;

        private TimeSpan _transitionStartTime = TimeSpan.Zero, _transitionDuration = TimeSpan.Zero;

        public Color ClearColor { get; set; } = Color.Black;

        private TransitionData? _transitionOutData = null;

        public TimeSpan Now => Game?.ProxyTime.Now ?? throw new InvalidOperationException("Cannot access time before loading has started.");
        public TimeSpan RealTime => Game?.ProxyTime.RealTime ?? throw new InvalidOperationException("Cannot access time before loading has started.");

        public TimeSpan LocalNow {
            get {
                if(!_hasStartTime) {
                    throw new InvalidOperationException("Cannot use local time until the first update frame has started.");
                }
                return Game.ProxyTime.Now - _nowOffset;
            }
        }

        public TimeSpan LocalRealTime {
            get {
                if(!_hasStartTime) {
                    throw new InvalidOperationException("Cannot use local time until the first update frame has started.");
                }
                return Game.ProxyTime.RealTime - _realTimeOffset;
            }
        }

        public TimeSpan TimeDelta => Game.ProxyTime.FrameDelta;

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

        private bool _hasStartTime = false;

        private void SetStartTime() {
            _nowOffset = Game.ProxyTime.Now;
            _realTimeOffset = Game.ProxyTime.RealTime;
            _hasStartTime = true;
        }

        public string Name { get; set; } = string.Empty;

        private void TryTransitionIn() {
            if(!FadeInIsFlagged) {
                return;
            }
            TransitionIn(Data.TransitionDuration);
        }

        internal void Load(GameManager game) {
            IsLoading = true;
            Game = game;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;

            TryTransitionIn();
        }

        internal void Unload() {
            OnUnload?.Invoke();
            Game = null;
            IsLoaded = false;
        }

        internal void WriteDebug(DebugWriter writer) {
            OnWriteDebug?.Invoke(writer);
        }

        private void HandleTransitionOut(TransitionData data) {
            if(data.Generator != null) {
                Game.SetState(data.Generator,data.Data);
            } else {
                Game.SetState(data.State,data.Data);
            }
        }

        /* This method lends itself to having a frame of latency with the input system.
         * Not expected to impact user experience, with 1 frame generally over a total of 1000. */
        internal void UpdateTransition() {
            if(TransitionState == TransitionState.None || TransitionT < 1) {
                return;
            }
            if(TransitionState == TransitionState.Out && _transitionOutData.HasValue) {
                HandleTransitionOut(_transitionOutData.Value);
                _transitionOutData = null;
            }
            TransitionState = TransitionState.None;
        }

        internal void Update() {
            if(!_hasStartTime) {
                SetStartTime();
            }
            IsUpdating = true;
            OnUpdate?.Invoke();
            IsUpdating = false;
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

        public bool IsTransitioning {
            get {
                bool isTransitioning = TransitionState != TransitionState.None;
                return isTransitioning;
            }
        }

        public float TransitionT {
            get {
                if(!IsTransitioning) {
                    return 0;
                }
                float t = (float)((LocalNow - _transitionStartTime) / _transitionDuration);
                if(t < 0) {
                    return t;
                } else if(t >= 1) {
                    return 1;
                }
                return t;
            }
        }

        public void TransitionOut(TransitionData transitionData) {
            if(transitionData.Generator != null && transitionData.State != null) {
                throw new InvalidOperationException("Transition out data cannot contain two game state values.");
            }
            TransitionState = TransitionState.Out;
            _transitionStartTime = LocalNow;
            _transitionDuration = transitionData.Duration;
            _transitionOutData = transitionData;
        }

        private void TransitionIn(TimeSpan duration) {
            TransitionState = TransitionState.In;
            _transitionDuration = duration;
            _transitionOutData = null;
        }

        public virtual void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
            Game.GraphicsDevice.Clear(ClearColor);
        }
    }
}
