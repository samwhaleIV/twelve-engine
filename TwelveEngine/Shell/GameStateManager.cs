using System.Diagnostics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Automation;
using TwelveEngine.Shell.UI;
using TwelveEngine.Shell.Hotkeys;
using TwelveEngine.Input;

namespace TwelveEngine.Shell {
    public sealed class GameStateManager:Game {
        /// <summary>
        /// Do not use externally except for diagnostic purposes. Has poor parity during transitions. The value is undefined during certain operations.
        /// </summary>
        internal GameState GameState { get; private set; }

        /* State fields */
        private GameState pendingState = null;
        private (Func<GameState> Value, StateData Data) pendingGenerator = (null, StateData.Empty);

        internal event Action OnLoad;

        private bool _isInitialized = false, _gameIsPaused = false, _isUpdating = false, _isRendering = false, _didUpdateFirstFrame = false, _advanceFrameIsQueued = false;

        private bool _isFastForwarding = false, _shouldFireUpdate = false;
        private int _framesToSkip = 0;

        private VCRDisplay VCRDisplay { get; init; }
        private HotkeySet KeyWatcherSet { get; init; }

        private GraphicsDeviceManager GraphicsDeviceManager { get; set; }
        internal SpriteBatch SpriteBatch { get; private set; }
        internal SpriteFont DebugFont { get; private set; }

        internal RenderTargetStack RenderTarget { get; private set; }

        internal DebugWriter DebugWriter { get; private init; }

        private static readonly Stopwatch Watch = new();

        private TimeSpan _updateDuration, _renderDuration;

        internal bool DrawDebug { get; private init; }

        internal GameLoopSyncContext SyncContext { get; init; }

        internal Exception ReroutedException { get; set; }

        internal GameStateManager(
            bool fullscreen = false,bool hardwareModeSwitch = false,bool verticalSync = true,bool drawDebug = false
        ) {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger.WriteBooleanSet("Context settings",new string[] {
                "Fullscreen","HardwareModeSwitch","VerticalSync","DrawDebug"
            },new bool[] {
                fullscreen, hardwareModeSwitch, verticalSync, drawDebug
            },LoggerLabel.GameManager);

            DrawDebug = drawDebug;

            IsMouseVisible = true;

            Content.RootDirectory = Constants.ContentDirectory;

            GraphicsDeviceManager.SynchronizeWithVerticalRetrace = verticalSync;

            GraphicsDeviceManager.IsFullScreen = fullscreen;
            GraphicsDeviceManager.HardwareModeSwitch = hardwareModeSwitch;

            if(hardwareModeSwitch) {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

                int? width = Config.GetIntNullable(Config.Keys.HWFullScreenWidth);
                int? height = Config.GetIntNullable(Config.Keys.HWFullScreenHeight);

                GraphicsDeviceManager.PreferredBackBufferWidth = width ?? displayMode.Width;
                GraphicsDeviceManager.PreferredBackBufferHeight = height ?? displayMode.Height;
            }

            IsFixedTimeStep = false;

            VCRDisplay = new VCRDisplay(this);

            AddAutomationAgentEventHandlers();

            KeyWatcherSet = new HotkeySet(GetDebugControls());
            DebugWriter = new(this);
        }

        private static TimeSpan ReadWatchAndReset() {
            Watch.Stop();
            TimeSpan elapsed = Watch.Elapsed;
            Watch.Reset();
            return elapsed;
        }

        private (Keys Key, Action Action)[] GetDebugControls() => new (Keys,Action)[]{
            (Constants.PlaybackKey, AutomationAgent.TogglePlayback),
            (Constants.RecordingKey, AutomationAgent.ToggleRecording),
            (Constants.PauseGameKey, () => SetPaused(!_gameIsPaused)),
            (Constants.AdvanceFrameKey, () => _advanceFrameIsQueued = true),
            (Constants.FullscreenKey, GraphicsDeviceManager.ToggleFullScreen),
            (Constants.CycleTimeDisplay, () => DebugWriter.CycleTimeWriter())
        };

        private void AutomationAgent_PlaybackStopped() {
            IsFixedTimeStep = false;
        }

        private void AutomationAgent_PlaybackStarted() {
            TargetElapsedTime = AutomationAgent.GetAveragePlaybackFrameTime();
            IsFixedTimeStep = true;
        }

        internal bool IsPaused {
            get => _gameIsPaused;
            private set => SetPaused(value);
        }

        private void SetPaused(bool paused) {
            if(_gameIsPaused == paused) {
                return;
            }
            if(!_gameIsPaused) {
                ProxyTime.Pause();
            } else {
                ProxyTime.Resume();
            }
            _gameIsPaused = paused;
        }

        private void LoadState(GameState state) {
            RenderTarget.Reset();
            CustomCursor.State = CursorState.Default;
            state.Load(this);
            Logger.LogStateChange(state);
        }

        public void SetState(GameState state,StateData? data = null) => SetState(() => state,data ?? StateData.Empty);
        public void SetState<TState>(StateData? data = null) where TState : GameState, new() => SetState(() => new TState(),data ?? StateData.Empty);

        public void SetState(Func<GameState> stateGenerator,StateData? data = null) {
            if(pendingState is not null || pendingGenerator.Value is not null) {
                /* Recursive loading! Preload all your assets to your heart's content! */
                pendingGenerator = (null, StateData.Empty);
                if(pendingState?.IsLoaded ?? false) {
                    pendingState.Unload();
                }
                pendingState = stateGenerator.Invoke();
                pendingState.Data = data ?? StateData.Empty;
                LoadState(pendingState);
                return;
            }
            if(!_isInitialized) {
                throw new InvalidOperationException("Cannot change GameState until GameManager has loaded all of its own content.");
            }
            if(_isRendering) {
                throw new InvalidOperationException("Cannot change a GameState during a Draw operation.");
            }
            if(_isUpdating) {
                pendingGenerator = (stateGenerator, data ?? StateData.Empty);
                return;
            }
            GameState oldState = GameState;
            GameState = null;
            oldState?.Unload();
            pendingState = stateGenerator.Invoke();
            pendingState.Data = data ?? StateData.Empty;
            LoadState(pendingState);
        }

        protected override void Initialize() {
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        protected override void LoadContent() {
            RuntimeTextures.Load(GraphicsDevice);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RenderTarget = new RenderTargetStack(GraphicsDevice);
            DebugFont = Content.Load<SpriteFont>(Constants.DebugFont);
            VCRDisplay.Load();
            _isInitialized = true;
            OnLoad?.Invoke();
        }

        private void AddAutomationAgentEventHandlers() {
            AutomationAgent.PlaybackStarted += ProxyTime.Pause;
            AutomationAgent.PlaybackStopped += ProxyTime.Resume;

            AutomationAgent.PlaybackStarted += AutomationAgent_PlaybackStarted;
            AutomationAgent.PlaybackStopped += AutomationAgent_PlaybackStopped;
        }

        private void RemoveAutomationAgentEventHandlers() {
            AutomationAgent.PlaybackStarted -= ProxyTime.Pause;
            AutomationAgent.PlaybackStopped -= ProxyTime.Resume;

            AutomationAgent.PlaybackStarted -= AutomationAgent_PlaybackStarted;
            AutomationAgent.PlaybackStopped -= AutomationAgent_PlaybackStopped;
        }

        protected override void UnloadContent() {
            RemoveAutomationAgentEventHandlers();

            RuntimeTextures.Unload();
            GameState?.Unload();
            GameState = null;

            if(pendingState?.IsLoaded ?? false) {
                pendingState.Unload();
                pendingState = null;
            }

            SpriteBatch?.Dispose();
            SpriteBatch = null;

            Content.Dispose();
            GraphicsDeviceManager?.Dispose();
            GraphicsDeviceManager = null;
        }

        private void AdvanceFrame(KeyboardState keyboardState,GameTime gameTime) {
            int frameAdvance = 0;
            if(AutomationAgent.PlaybackActive && keyboardState.IsKeyDown(Keys.LeftShift)) {

                _shouldFireUpdate = false;
                _framesToSkip = Constants.ShiftFrameSkip;
                VCRDisplay.AdvanceFramesMany(gameTime);
                frameAdvance = Constants.ShiftFrameSkip;

            } else if(_gameIsPaused && !_shouldFireUpdate) {
                TimeSpan simulationTIme = TimeSpan.FromMilliseconds(Constants.SimFrameTime);
                ProxyTime.AddSimulationTime(simulationTIme);
                _shouldFireUpdate = true;
                _framesToSkip = 0;
                VCRDisplay.AdvanceFrame(gameTime);
                frameAdvance = 1;
            }

            if(frameAdvance > 0 && AutomationAgent.PlaybackActive) {
                int maxFrame = AutomationAgent.PlaybackFrameCount.HasValue ? AutomationAgent.PlaybackFrameCount.Value - 1 : int.MaxValue;
                Console.WriteLine($"[Automation Agent] Advanced to frame {MathF.Min(AutomationAgent.FrameNumber + frameAdvance,maxFrame)}");
            }
        }

        private void UpdateGame(GameTime trueGameTime) {
            ProxyTime.Update(trueGameTime);

            AutomationAgent.StartUpdate();

            if(AutomationAgent.PlaybackActive) {
                ProxyTime.AddSimulationTime(AutomationAgent.GetFrameTime());
            }
            if(AutomationAgent.RecordingActive) {
                AutomationAgent.UpdateRecordingFrame(ProxyTime.FrameDelta);
            }

            InputStateCache.Update(IsActive);
            GameState.Update();

            AutomationAgent.EndUpdate();
        }

        private void FastForward() {
            if(_isFastForwarding) {
                return;
            }
            _isFastForwarding = true;
            int count = _framesToSkip;
            _framesToSkip = 0;

            int limit = AutomationAgent.FrameNumber + count;
            int? playbackFrameCount = AutomationAgent.PlaybackFrameCount;

            if(playbackFrameCount.HasValue) {
                limit = Math.Min(playbackFrameCount.Value,limit);
            }

            for(int i = AutomationAgent.FrameNumber;i < limit;i++) {
                UpdateGame(null); /* Automation agent supplies a game time when playback is active */
            }

            _isFastForwarding = false;
        }

        private void HotSwapPendingState() {
            GameState = pendingState;
            pendingState = null;
            if(!_gameIsPaused) {
                return;
            }
            _shouldFireUpdate = true;
        }

        protected override void Update(GameTime gameTime) {
            if(!_didUpdateFirstFrame) {
                /* Why does this exist? MonoGame calls a duplicate first update call.
                 * See https://github.com/MonoGame/MonoGame/blob/2fa596123f834f322be19811d97fe6c20616d570/MonoGame.Framework/Game.cs#L489 */
                _didUpdateFirstFrame = true;
                return;
            }
            _isUpdating = true;
            Watch.Start();
            if(GameState is null) {
                if(pendingState is not null) {
                    HotSwapPendingState();
                } else {
                    _updateDuration = ReadWatchAndReset();
                    _isUpdating = false;
                    return;
                }
            }

            KeyboardState vanillaKeyboardState = Keyboard.GetState();
            KeyWatcherSet.Update(vanillaKeyboardState);
            if(_advanceFrameIsQueued) {
                AdvanceFrame(vanillaKeyboardState,gameTime);
                _advanceFrameIsQueued = false;
            }

            if(!_gameIsPaused) {
                UpdateGame(gameTime);             
            } else if(_shouldFireUpdate) {
                UpdateGame(gameTime);
                _shouldFireUpdate = false;
            } else if(_framesToSkip > 0) {
                FastForward();
            }
            SyncContext?.Update();
            if(ReroutedException is not null) {
                throw new Exception("Unhandled exception from game loop sync context.",ReroutedException);
            }
            _updateDuration = ReadWatchAndReset();
            _isUpdating = false;
        }

        private void TryApplyPendingStateGenerator() {
            if(pendingGenerator.Value is null) {
                return;
            }
            SyncContext?.Clear();
            bool benchmark = Config.GetBool(Config.Keys.BenchmarkStateSwap);
            if(benchmark) {
                Watch.Start();
            }
            Func<GameState> generator = pendingGenerator.Value;
            StateData data = pendingGenerator.Data;
            pendingGenerator = (null, StateData.Empty);
            SetState(generator,data);
            if(data.Flags.HasFlag(StateFlags.ForceGC) || Config.GetBool(Config.Keys.StateCleanUpGC)) {
                GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);
                if(benchmark) {
                    Logger.WriteLine($"Forced GC and state swap, elapsed time: {ReadWatchAndReset()}",LoggerLabel.Benchmark);
                }
            } else if(benchmark) {
                Logger.WriteLine($"State swap, elapsed time: {ReadWatchAndReset()}",LoggerLabel.Benchmark);
            }
        }

        protected override void Draw(GameTime trueGameTime) {
            _isRendering = true;

            var gameState = GameState;

            if(gameState is not null && !gameState.IsLoaded) {
                throw new InvalidOperationException("Attempted to render game state after it has been unloaded.");
            }

            Watch.Start();

            if(gameState is not null) {
                gameState.ResetGraphicsState(GraphicsDevice);
                gameState.PreRender();
                gameState.Render();
            } else {
                GraphicsDevice.Clear(Color.Black);
                /* Notice: No game state */
            }

            VCRDisplay.Render(trueGameTime);
            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.LinearClamp);

            if(DrawDebug) {
                DebugWriter.DrawGameTimeDebug(_updateDuration,_renderDuration);
                gameState?.WriteDebug(DebugWriter);
            }

            SpriteBatch.End();

            _renderDuration = ReadWatchAndReset();

            _isRendering = false;
            TryApplyPendingStateGenerator();
        }
    }
}
