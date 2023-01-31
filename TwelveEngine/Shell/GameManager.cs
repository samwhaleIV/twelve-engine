using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Input;
using TwelveEngine.Shell.Automation;
using System.Collections.Generic;
using TwelveEngine.Shell.UI;
using System.Text;
using TwelveEngine.Shell.Hotkeys;

namespace TwelveEngine.Shell {
    public sealed class GameManager:Game {

        public bool DrawDebug { get; set; } = false;

        /// <summary>
        /// Do not use externally except for diagnostic purposes. Has poor parity during transitions. The value is undefined during certain operations.
        /// </summary>
        internal GameState State { get; private set; }

        /* State fields */
        private GameState pendingState = null;
        private (Func<GameState> Value, StateData Data) pendingGenerator = (null, StateData.Empty);

        public event Action<GameManager> OnLoad;

        /// <summary>
        /// Why does this exist? MonoGame calls a duplicate first update call.
        /// <see href="https://github.com/MonoGame/MonoGame/blob/2fa596123f834f322be19811d97fe6c20616d570/MonoGame.Framework/Game.cs#L489"/>
        /// </summary>
        public bool DidUpdateFirstFrame { get; private set; } = false;
        public bool AdvanceFrameIsQueued { get; private set; } = false;

        public GameManager(
            bool fullscreen = false,bool hardwareModeSwitch = false,bool verticalSync = true
        ) {
            GraphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger.WriteBooleanSet("Context settings",new string[] {
                "Fullscreen","HardwareModeSwitch","VerticalSync"
            },new bool[] {
                fullscreen, hardwareModeSwitch, verticalSync
            },LoggerLabel.GameManager);

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

            AutomationAgent.PlaybackStarted += ProxyTime.Pause;
            AutomationAgent.PlaybackStopped += ProxyTime.Resume;

            AutomationAgent.PlaybackStarted += AutomationAgent_PlaybackStarted;
            AutomationAgent.PlaybackStopped += AutomationAgent_PlaybackStopped;

            KeyWatcherSet = new HotkeySet(GetDebugControls());
            DebugWriter = new(this);
        }

        private readonly Stopwatch watch = new();
        private TimeSpan updateDuration, renderDuration;

        private (Keys Key, Action Action)[] GetDebugControls() => new (Keys,Action)[]{
            (Constants.PlaybackKey, AutomationAgent.TogglePlayback),
            (Constants.RecordingKey, AutomationAgent.ToggleRecording),

            (Constants.PauseGameKey, () => SetPaused(!_gameIsPaused)),
            (Constants.AdvanceFrameKey, () => AdvanceFrameIsQueued = true),
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

        /* Loading, automation, and time maniuplation state */
        private bool initialized = false, _gameIsPaused = false, updating = false, rendering = false;

        private bool fastForwarding = false, shouldAdvanceFrame = false;
        private int framesToSkip = 0;

        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        public RenderTargetStack RenderTarget { get; private set; }
        public Viewport Viewport => RenderTarget.GetViewport();

        private VCRDisplay VCRDisplay { get; init; }
        private HotkeySet KeyWatcherSet { get; init; }

        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont DebugFont { get; private set; }
        public DebugWriter DebugWriter { get; private init; }

        public bool IsPaused {
            get => _gameIsPaused;
            set => SetPaused(value);
        }

        public void SetPaused(bool paused) {
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
            if(!initialized) {
                throw new InvalidOperationException("Cannot change GameState until GameManager has loaded all of its own content.");
            }
            if(rendering) {
                throw new InvalidOperationException("Cannot change a GameState during a Draw operation.");
            }
            if(updating) {
                pendingGenerator = (stateGenerator, data ?? StateData.Empty);
                return;
            }
            GameState oldState = State;
            State = null;
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
            initialized = true;
            OnLoad?.Invoke(this);
        }

        protected override void UnloadContent() {
            RuntimeTextures.Unload();

            State?.Unload();
            State = null;

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

                shouldAdvanceFrame = false;
                framesToSkip = Constants.ShiftFrameSkip;
                VCRDisplay.AdvanceFramesMany(gameTime);
                frameAdvance = Constants.ShiftFrameSkip;

            } else if(_gameIsPaused && !shouldAdvanceFrame) {
                TimeSpan simulationTIme = TimeSpan.FromMilliseconds(Constants.SimFrameTime);
                ProxyTime.AddSimulationTime(simulationTIme);
                shouldAdvanceFrame = true;
                framesToSkip = 0;
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
            State.Update();

            AutomationAgent.EndUpdate();
        }

        private void FastForward() {
            if(fastForwarding) {
                return;
            }
            fastForwarding = true;
            int count = framesToSkip;
            framesToSkip = 0;

            int limit = AutomationAgent.FrameNumber + count;
            int? playbackFrameCount = AutomationAgent.PlaybackFrameCount;

            if(playbackFrameCount.HasValue) {
                limit = Math.Min(playbackFrameCount.Value,limit);
            }

            for(int i = AutomationAgent.FrameNumber;i < limit;i++) {
                UpdateGame(null); /* Automation agent supplies a game time when playback is active */
            }

            fastForwarding = false;
        }

        public void FastForward(int count) {
            if(!AutomationAgent.PlaybackActive) {
                return;
            }
            framesToSkip = count;
        }

        private void HotSwapPendingState() {
            State = pendingState;
            pendingState = null;
            if(!_gameIsPaused) {
                return;
            }
            shouldAdvanceFrame = true;
        }

        private TimeSpan ReadWatchAndReset() {
            watch.Stop();
            TimeSpan elapsed = watch.Elapsed;
            watch.Reset();
            return elapsed;
        }

        protected override void Update(GameTime gameTime) {
            if(!DidUpdateFirstFrame) {
                DidUpdateFirstFrame = true;
                return;
            }
            updating = true;
            watch.Start();
            if(State is null) {
                if(pendingState is not null) {
                    HotSwapPendingState();
                } else {
                    updateDuration = ReadWatchAndReset();
                    updating = false;
                    return;
                }
            }

            KeyboardState vanillaKeyboardState = Keyboard.GetState();
            KeyWatcherSet.Update(vanillaKeyboardState);
            if(AdvanceFrameIsQueued) {
                AdvanceFrame(vanillaKeyboardState,gameTime);
                AdvanceFrameIsQueued = false;
            }

            if(!_gameIsPaused) {
                UpdateGame(gameTime);             
            } else if(shouldAdvanceFrame) {
                UpdateGame(gameTime);
                shouldAdvanceFrame = false;
            } else if(framesToSkip > 0) {
                FastForward();
            }
            updateDuration = ReadWatchAndReset();
            updating = false;
        }

        private void TryApplyPendingStateGenerator() {
            if(pendingGenerator.Value is null) {
                return;
            }
            bool benchmark = Config.GetBool(Config.Keys.BenchmarkStateSwap);
            if(benchmark) {
                watch.Start();
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
            rendering = true;

            var gameState = State;

            if(gameState is not null && !gameState.IsLoaded) {
                throw new InvalidOperationException("Attempted to render game state after it has been unloaded.");
            }

            watch.Start();

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
                DebugWriter.DrawGameTimeDebug(updateDuration,renderDuration);
                gameState?.WriteDebug(DebugWriter);
            }

            SpriteBatch.End();

            renderDuration = ReadWatchAndReset();

            rendering = false;
            gameState?.UpdateTransition();
            TryApplyPendingStateGenerator();
        }
    }
}
