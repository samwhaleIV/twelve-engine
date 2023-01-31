using Microsoft.Xna.Framework.Content;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class GameState {

        private GameStateManager _game = null;

        public ContentManager Content => _game.Content;
        public SpriteBatch SpriteBatch => _game.SpriteBatch;
        public RenderTargetStack RenderTarget => _game.RenderTarget;

        public GraphicsDevice GraphicsDevice => _game.GraphicsDevice;
        public Viewport Viewport => _game.RenderTarget.GetViewport();

        public bool GameIsActive => _game.IsActive;
        public bool GameIsPaused => _game.IsPaused;

        #pragma warning disable CA1822 // Mark members as static

        /* Having time aliased in gamestate might save my ass some day. */
        public TimeSpan Now => ProxyTime.Now;
        public TimeSpan RealTime => ProxyTime.RealTime;
        public TimeSpan FrameDelta => ProxyTime.FrameDelta;

        #pragma warning restore CA1822 // Mark members as static

        public TimeSpan LocalNow => GetLocalNow();
        public TimeSpan LocalRealTime => GetLocalRealTime();

        private StateData _data;

        public StateData Data {
            get => GetValidatedStateData();
            internal set => _data = value;
        }

        public bool HasFlag(StateFlags flag) => Data.Flags.HasFlag(flag);
        public bool FadeInIsFlagged => Data.Flags.HasFlag(StateFlags.FadeIn);

        private TimeSpan _nowOffset, _realTimeOffset, _transitionStartTime, _transitionDuration;

        /// <summary>
        /// Graphics device clear color. See <see cref="ResetGraphicsState"/>.
        /// </summary>
        public Color ClearColor { get; set; } = Color.Black;

        private TransitionData? _transitionOutData = null;

        public event Action OnLoad, OnUnload, OnTransitionInFinished;

        public event Action<DebugWriter> OnWriteDebug;
        public event Action OnUpdate, OnRender, OnPreRender;

        public TransitionRenderer TransitionRenderer = TransitionRenderer.Default;

        public bool IsLoaded { get; private set; } = false;
        public bool IsLoading { get; private set; } = false;

        public bool IsUpdatingUI { get; private set; } = false;

        public bool IsUpdating { get; private set; } = false;
        public bool IsRendering { get; private set; } = false;
        public bool IsPreRendering { get; private set; } = false;

        public TransitionState TransitionState { get; private set; }
        public bool IsTransitioning => GetIsTransitioning();
        public float TransitionT => GetTransitionT();

        private bool _hasStartTime = false;

        /// <summary>
        /// For logging information. Not intended to be used for application control.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        private StateData GetValidatedStateData() {
            if(!(IsLoaded || IsLoading)) {
                /* If you need to access data before a 'Load' call, don't use 'Data.Args'; Use regular parameters in the state constructor. */
                throw new InvalidOperationException("Cannot access 'Data' before the state has started loading!");
            }
            return _data;
        }

        private TimeSpan GetLocalNow() {
            if(!_hasStartTime) {
                throw new InvalidOperationException("Cannot use local time until the first update frame has started.");
            }
            return Now - _nowOffset;
        }

        private TimeSpan GetLocalRealTime() {
            if(!_hasStartTime) {
                throw new InvalidOperationException("Cannot use local time until the first update frame has started.");
            }
            return RealTime - _realTimeOffset;
        }

        private void SetStartTime() {
            _nowOffset = Now;
            _realTimeOffset = RealTime;
            _hasStartTime = true;
        }

        private void TryTransitionIn() {
            if(!FadeInIsFlagged) {
                return;
            }
            TransitionIn(Data.TransitionDuration);
        }

        internal void Load(GameStateManager game) {
            IsLoading = true;
            _game = game;
            OnLoad?.Invoke();
            IsLoaded = true;
            IsLoading = false;
            TryTransitionIn();
        }

        internal void Unload() {
            OnUnload?.Invoke();
            _game = null;
            IsLoaded = false;
        }

        internal void WriteDebug(DebugWriter writer) {
            OnWriteDebug?.Invoke(writer);
        }

        private void HandleTransitionOut(TransitionData data) {
            if(data.Generator != null) {
                _game.SetState(data.Generator,data.Data);
            } else {
                _game.SetState(data.State,data.Data);
            }
        }

        /* This method of transitioning (with how its ordered in the game loop) lends itself to creatin a frame of latency against the input system.
         * Not expected to impact user experience, with 1 frame of loss (generally) over a total of 1000 (1 second long transition). */
        internal void UpdateTransition() {
            if(TransitionState == TransitionState.None || TransitionT < 1) {
                return;
            }
            if(TransitionState == TransitionState.Out && _transitionOutData.HasValue) {
                HandleTransitionOut(_transitionOutData.Value);
                _transitionOutData = null;
            }
            TransitionState oldTransitionState = TransitionState;
            TransitionState = TransitionState.None;
            if(oldTransitionState != TransitionState.In) {
                return;
            }
            OnTransitionInFinished?.Invoke();
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
                TransitionRenderer.DrawOut(SpriteBatch,Viewport.Bounds,TransitionT);
            } else if(TransitionState == TransitionState.In) {
                TransitionRenderer.DrawIn(SpriteBatch,Viewport.Bounds,TransitionT);
            }
            IsRendering = false;
        }

        internal void PreRender() {
            IsPreRendering = true;
            OnPreRender?.Invoke();
            IsPreRendering = false;
        }

        private bool GetIsTransitioning() {
            if((!IsLoaded || IsLoading) && FadeInIsFlagged) {
                return true;
            } else {
                return TransitionState != TransitionState.None;
            }
        }

        private float GetTransitionT() {
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

         protected internal virtual void ResetGraphicsState(GraphicsDevice graphicsDevice) {
            graphicsDevice.SamplerStates[0] = SamplerState.LinearClamp;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.BlendState = BlendState.AlphaBlend;
            graphicsDevice.BlendFactor = Color.White;
            graphicsDevice.Clear(ClearColor);
        }
    }
}
