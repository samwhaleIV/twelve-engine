using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Input;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public class GameState {

        internal GameStateManager Game { get; private set; }

        public ContentManager Content => Game.Content;
        public SpriteBatch SpriteBatch => Game.SpriteBatch;
        public RenderTargetStack RenderTarget => Game.RenderTarget;

        public GraphicsDevice GraphicsDevice => Game.GraphicsDevice;
        public Viewport Viewport => Game.RenderTarget.GetViewport();

        public bool GameIsActive => Game.IsActive;
        public bool GameIsPaused => Game.IsPaused;

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

        private Shell.TransitionData? _transitionOutData = null;

        public OrderedEvent OnUpdate = new(), OnRender = new(), OnPreRender = new(), OnLoad = new(), OnUnload = new(), OnTransitionIn = new();
        public DebugWriterEvent OnWriteDebug = new();

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
            OnWriteDebug.Writer = writer;
            OnWriteDebug.Invoke();
        }

        private void HandleTransitionOut(Shell.TransitionData data) {
            if(data.Generator != null) {
                Game.SetState(data.Generator,data.Data);
            } else {
                Game.SetState(data.State,data.Data);
            }
        }

        /* This method of transitioning (with how its ordered in the game loop) lends itself to creatin a frame of latency against the input system.
         * Not expected to impact user experience, with 1 frame of loss (generally) over a total of 1000 (1 second long transition). */
        private void UpdateTransition() {
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
            OnTransitionIn?.Invoke();
        }

        private readonly struct TransitionData {
            public readonly TransitionState State { get; init; }
            public readonly float T { get; init; }
            public static readonly TransitionData None = new() { State = TransitionState.None,T = 0 };
        }

        private TransitionData _oldTransitionData = TransitionData.None;

        internal void Update() {
            if(!_hasStartTime) {
                SetStartTime();
            }
            IsUpdating = true;

            /* Set before updating the transition so the renderer is behind one frame (so the last visible frame of the scene is transitionT == 1*/
            _oldTransitionData = new TransitionData { State = TransitionState, T = GetTransitionT() };
            UpdateTransition();

            OnUpdate.Invoke();
            IsUpdating = false;
        }

        internal void Render() {
            IsRendering = true;
            OnRender.Invoke();

            TransitionState oldState = _oldTransitionData.State;
            float oldT = _oldTransitionData.T;

            if(oldState == TransitionState.Out) {
                TransitionRenderer.DrawOut(SpriteBatch,Viewport.Bounds,oldT);
            } else if(oldState == TransitionState.In) {
                TransitionRenderer.DrawIn(SpriteBatch,Viewport.Bounds,oldT);
            }
            IsRendering = false;
        }

        internal void PreRender() {
            IsPreRendering = true;
            OnPreRender.Invoke();
            IsPreRendering = false;
        }

        /// <summary>
        /// The <c>T</c> normal point at which the input system is activated for a game state when it's transitioning in.
        /// </summary>
        public float TransitionInputThreshold { get; set; } = Constants.UI.DefaultTransitionInputThreshold;

        private bool GetIsTransitioning() {
            if((!IsLoaded || IsLoading) && FadeInIsFlagged) {
                return true;
            } else if(TransitionState == TransitionState.None) {
                return false;
            } else if(TransitionState == TransitionState.Out) {
                return true;
            }
            float t = GetTransitionT();
            return t < TransitionInputThreshold;
        }

        private float GetTransitionT() {
            if(TransitionState == TransitionState.None) {
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

        public void TransitionOut(Shell.TransitionData transitionData) {
            if(transitionData.Generator != null && transitionData.State != null) {
                throw new InvalidOperationException("Transition out data cannot contain two game state values.");
            }
            _transitionDuration = transitionData.Duration;
            if(TransitionState == TransitionState.In) {
                _transitionStartTime = LocalNow - (1 - GetTransitionT()) * _transitionDuration;
            } else {
                _transitionStartTime = LocalNow;
            }
            TransitionState = TransitionState.Out;
            _transitionOutData = transitionData;
        }

        private void TransitionIn(TimeSpan duration) {
            TransitionState = TransitionState.In;
            _transitionDuration = duration;
            _transitionOutData = null;
        }

        internal virtual bool GetCustomCursorHiddenState() {
            return false;
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
