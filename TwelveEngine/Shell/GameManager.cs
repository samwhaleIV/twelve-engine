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

namespace TwelveEngine.Shell {
    public sealed class GameManager:Game {

        public bool DrawDebug { get; set; } = false;

        public GameManager(
            bool fullscreen = false,bool hardwareModeSwitch = false,bool verticalSync = true
        ) {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger.WriteBooleanSet("Context settings",new string[] {
                "Fullscreen","HardwareModeSwitch","VerticalSync"
            },new bool[] {
                fullscreen, hardwareModeSwitch, verticalSync
            },LoggerLabel.GameManager);

            Content.RootDirectory = Constants.ContentDirectory;

            graphicsDeviceManager.SynchronizeWithVerticalRetrace = verticalSync;

            graphicsDeviceManager.IsFullScreen = fullscreen;
            graphicsDeviceManager.HardwareModeSwitch = hardwareModeSwitch;

            if(hardwareModeSwitch) {
                DisplayMode displayMode = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode;

                int? width = Config.GetIntNullable(Config.Keys.HWFullScreenWidth);
                int? height = Config.GetIntNullable(Config.Keys.HWFullScreenHeight);

                graphicsDeviceManager.PreferredBackBufferWidth = width ?? displayMode.Width;
                graphicsDeviceManager.PreferredBackBufferHeight = height ?? displayMode.Height;
            }

            IsFixedTimeStep = false;

            vcrDisplay = new VCRDisplay(this,automationAgent);

            automationAgent.PlaybackStarted += proxyGameTime.Freeze;
            automationAgent.PlaybackStopped += proxyGameTime.Unfreeze;

            automationAgent.PlaybackStarted += AutomationAgent_PlaybackStarted;
            automationAgent.PlaybackStopped += AutomationAgent_PlaybackStopped;

            keyWatcherSet = new HotkeySet(GetDebugControls());

            debugWriter = new DebugWriter(this);
        }

        private readonly Stopwatch watch = new();
        private TimeSpan updateDuration, renderDuration;

        private readonly FPSCounter fpsCounter = new();

        private readonly FrameTimeSmoother updateDurationSmoother = new();
        private readonly FrameTimeSmoother renderDurationSmoother = new();

        private void DrawGameTimeDebug(GameTime trueGameTime,DebugWriter writer) {
            TimeSpan trueNow = trueGameTime.TotalGameTime;

            writer.ToBottomRight();

            renderDurationSmoother.Update(trueNow,renderDuration);
            writer.WriteTimeMS(renderDurationSmoother.Average,"R");

            updateDurationSmoother.Update(trueNow,updateDuration);
            writer.WriteTimeMS(updateDurationSmoother.Average,"U");

            writer.ToBottomLeft();

            fpsCounter.Update(trueNow);
            writer.WriteFPS(fpsCounter.FPS);

            writer.Write(proxyGameTime.TotalGameTime,"PT");
        }

        private (Keys Key, Action Action)[] GetDebugControls() => new (Keys,Action)[]{
            (Constants.PlaybackKey, automationAgent.TogglePlayback),
            (Constants.RecordingKey, automationAgent.ToggleRecording),

            (Constants.PauseGameKey, () => SetPaused(!gamePaused)),
            (Constants.AdvanceFrameKey, () => advanceFrameQueued = true),
            (Constants.FullscreenKey, () => graphicsDeviceManager.ToggleFullScreen())
        };

        private void AutomationAgent_PlaybackStopped() {
            IsFixedTimeStep = false;
        }

        private void AutomationAgent_PlaybackStarted() {
            TargetElapsedTime = automationAgent.GetAveragePlaybackFrameTime();
            IsFixedTimeStep = true;
        }

        /* Loading, automation, and time maniuplation state */
        private bool initialized = false, gamePaused = false, updating = false, rendering = false;

        private bool fastForwarding = false, shouldAdvanceFrame = false;
        private int framesToSkip = 0;

        /* God class extensions */
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        private readonly VCRDisplay vcrDisplay;
        private readonly HotkeySet keyWatcherSet;

        private readonly AutomationAgent automationAgent = new();
        private readonly ProxyGameTime proxyGameTime = new();
        private readonly DebugWriter debugWriter;

        /* Public access */
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public AutomationAgent AutomationAgent => automationAgent;
        public GameTime Time => proxyGameTime;

        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont DebugFont { get; private set; }
        private RenderTargetStack RenderTargets { get; set; }

        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }
        public GamePadState GamePadState { get; private set; }

        public void PushRenderTarget(RenderTarget2D renderTarget) => RenderTargets.Push(renderTarget);
        public void PopRenderTarget() => RenderTargets.Pop();
        public Viewport Viewport => RenderTargets.GetViewport();

        public bool IsPaused {
            get => gamePaused;
            set => SetPaused(value);
        }

        public void SetPaused(bool paused) {
            if(gamePaused == paused) {
                return;
            }
            if(!gamePaused) {
                proxyGameTime.Freeze();
            } else {
                proxyGameTime.Unfreeze();
            }
            gamePaused = paused;
        }

        private static GamePadState GetGamepadState() {
            var state = GamePad.GetState(Config.GetInt(Config.Keys.GamePadIndex),GamePadDeadZone.Circular);
            return state;
        }

        private KeyboardState GetKeyboardState() {
            var state = Keyboard.GetState();
            return automationAgent.FilterKeyboardState(state);
        }

        private MouseState GetMouseState() {
            var state = Mouse.GetState();
            return automationAgent.FilterMouseState(state);
        }

        /* State fields */
        private GameState pendingState = null;
        private (Func<GameState> Value, StateData Data) pendingGenerator = (null, StateData.Empty);

        private GameState _gameState = null;

        private static readonly StringBuilder stringBuilder = new();

        private void ResetLeakyStateProperties() {
            GraphicsDevice.Reset();
            CursorState = CursorState.Default;
        }

        private void LoadState(GameState state) {
            ResetLeakyStateProperties();
            state.Load(this);
            stringBuilder.Append($"[{proxyGameTime.TotalGameTime}] Set state: ");
            string stateName = state.Name;
            stringBuilder.Append('"');
            stringBuilder.Append(string.IsNullOrEmpty(stateName) ? Logger.NO_NAME_TEXT : stateName);
            stringBuilder.Append("\" { Args = ");
            StateData data = state.Data;
            if(data.Args != null && data.Args.Length >= 1) {
                foreach(var arg in data.Args) {
                    if(string.IsNullOrWhiteSpace(arg)) {
                        continue;
                    }
                    stringBuilder.Append($"{arg}, ");
                }
                stringBuilder.Remove(stringBuilder.Length-2,2);
            } else {
                stringBuilder.Append("None");
            }
            stringBuilder.AppendLine($", Flags = {data.Flags.ToString()} }}");

            Logger.Write(stringBuilder);
            stringBuilder.Clear();
        }

        public void SetState(Func<GameState> stateGenerator,StateData? data = null) {
            if(pendingState != null || pendingGenerator.Value != null) {
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
            GameState oldState = _gameState;
            _gameState = null;
            oldState?.Unload();
            pendingState = stateGenerator.Invoke();
            pendingState.Data = data ?? StateData.Empty;
            LoadState(pendingState);
        }

        public void SetState(GameState state,StateData? data = null) {
            SetState(() => state,data ?? StateData.Empty);
        }

        public void SetState<TState>(StateData? data = null) where TState : GameState, new() {
            SetState(() => new TState(),data ?? StateData.Empty);
        }

        protected override void Initialize() {
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        public event Action<GameManager> OnLoad;
        
        public Texture2D EmptyTexture { get; private set; }

        private void SetEmptyTexture() {
            int size = Constants.EmptyTextureSize;
            int pixelCount = size * size;
            Texture2D emptyTexture = new(GraphicsDevice,size,size);
            Color[] pixels = new Color[pixelCount];
            for(int i = 0;i<pixelCount;i++) {
                pixels[i] = Color.White;
            }
            emptyTexture.SetData(pixels);
            EmptyTexture = emptyTexture;
        }

        protected override void LoadContent() {
            SetEmptyTexture();
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RenderTargets = new RenderTargetStack(GraphicsDevice);
            DebugFont = Content.Load<SpriteFont>(Constants.DebugFont);
            vcrDisplay.Load();
            initialized = true;
            OnLoad?.Invoke(this);
        }

        protected override void UnloadContent() {
            _gameState?.Unload();
            _gameState = null;

            EmptyTexture?.Dispose();
            EmptyTexture = null;

            if(pendingState?.IsLoaded ?? false) {
                pendingState.Unload();
                pendingState = null;
            }

            SpriteBatch?.Dispose();
            SpriteBatch = null;

            Content.Dispose();
            graphicsDeviceManager.Dispose();
        }

        private bool advanceFrameQueued = false;

        private void AdvanceFrame(KeyboardState keyboardState,GameTime gameTime) {
            int frameAdvance = 0;
            if(automationAgent.PlaybackActive && keyboardState.IsKeyDown(Keys.LeftShift)) {

                shouldAdvanceFrame = false;
                framesToSkip = Constants.ShiftFrameSkip;
                vcrDisplay.AdvanceFramesMany(gameTime);
                frameAdvance = Constants.ShiftFrameSkip;

            } else if(gamePaused && !shouldAdvanceFrame) {

                TimeSpan simTime = TimeSpan.FromMilliseconds(Constants.SimFrameTime);
                proxyGameTime.AddSimTime(simTime);
                shouldAdvanceFrame = true;
                framesToSkip = 0;
                vcrDisplay.AdvanceFrame(gameTime);
                frameAdvance = 1;
            }

            if(frameAdvance > 0 && automationAgent.PlaybackActive) {
                int maxFrame = automationAgent.PlaybackFrameCount.HasValue ? automationAgent.PlaybackFrameCount.Value - 1 : int.MaxValue;
                Console.WriteLine($"[Automation Agent] Advanced to frame {MathF.Min(automationAgent.FrameNumber + frameAdvance,maxFrame)}");
            }
        }

        private void UpdateGame(GameTime trueGameTime) {
            proxyGameTime.Update(trueGameTime);
            automationAgent.StartUpdate();

            if(automationAgent.PlaybackActive) {
                proxyGameTime.AddSimTime(automationAgent.GetFrameTime());
            }
            if(automationAgent.RecordingActive) {
                automationAgent.UpdateRecordingFrame(proxyGameTime);
            }

            KeyboardState = GetKeyboardState();
            MouseState = GetMouseState();
            GamePadState = GetGamepadState();

            _gameState.Update();

            automationAgent.EndUpdate();
        }

        private void FastForward() {
            if(fastForwarding) {
                return;
            }
            fastForwarding = true;
            int count = framesToSkip;
            framesToSkip = 0;

            int limit = automationAgent.FrameNumber + count;
            int? playbackFrameCount = automationAgent.PlaybackFrameCount;

            if(playbackFrameCount.HasValue) {
                limit = Math.Min(playbackFrameCount.Value,limit);
            }

            for(int i = automationAgent.FrameNumber;i < limit;i++) {
                UpdateGame(null); /* Automation agent supplies a game time when playback is active */
            }

            fastForwarding = false;
        }

        public void FastForward(int count) {
            if(!automationAgent.PlaybackActive) {
                return;
            }
            framesToSkip = count;
        }

        private bool HasGameState => _gameState != null;

        private void HotSwapPendingState() {
            _gameState = pendingState;
            pendingState = null;
            if(!gamePaused) {
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
            updating = true;
            watch.Start();
            if(!HasGameState) {
                if(pendingState != null) {
                    HotSwapPendingState();
                } else {
                    updateDuration = ReadWatchAndReset();
                    updating = false;
                    return;
                }
            }

            KeyboardState vanillaKeyboardState = Keyboard.GetState();
            keyWatcherSet.Update(vanillaKeyboardState);
            if(advanceFrameQueued) {
                AdvanceFrame(vanillaKeyboardState,gameTime);
                advanceFrameQueued = false;
            }

            if(!gamePaused) {
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
            if(pendingGenerator.Value == null) {
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

        private void DrawCustomCursor() {
            SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
            Vector2 mousePosition = MouseState.Position.ToVector2();
            Rectangle cursorArea = GetCursorArea();
            SpriteBatch.Draw(_customCursorTexture,mousePosition,cursorArea,Color.White,0f,cursorArea.Size.ToVector2()*0.5f,new Vector2(CursorScale),SpriteEffects.None,1f);
            SpriteBatch.End();
        }

        protected override void Draw(GameTime trueGameTime) {
            rendering = true;

            var gameState = _gameState;

            if(gameState != null && !gameState.IsLoaded) {
                /* Calm yourself, this isn't buffer into a log file - but hopefully you never see this in production */
                Console.WriteLine($"[{proxyGameTime.TotalGameTime}] WARNING: Attempt to render game state after it has been unloaded");
                return;
            }

            watch.Start();

            if(gameState != null) {
                gameState.ResetGraphicsState(GraphicsDevice);
                gameState.PreRender();
                gameState.Render();
            } else {
                GraphicsDevice.Clear(Color.Black);
                /* Notice: No game state */
            }

            vcrDisplay.Render(trueGameTime);
            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.LinearClamp);

            if(DrawDebug) {
                DrawGameTimeDebug(trueGameTime,debugWriter);
                gameState?.WriteDebug(debugWriter);
            }

            SpriteBatch.End();
            if(_customCursorTexture != null) {
                DrawCustomCursor();
            }

            renderDuration = ReadWatchAndReset();

            rendering = false;
            TryApplyPendingStateGenerator();
        }

        private void UpdateCursorDisplay() {
            if(IsActive && _customCursorTexture != null) {
                IsMouseVisible = false;
                return;
            }
            IsMouseVisible = true;
        }

        private Texture2D _customCursorTexture = null;

        public Texture2D CustomCursorTexture {
            get {
                return _customCursorTexture;
            }
            set {
                _customCursorTexture = value;
                UpdateCursorDisplay();
            }
        }

        public CursorState CursorState { get; set; } = CursorState.Default;

        public float CursorScale { get; set; } = 1f;

        private Rectangle GetCursorArea() {
            if(!CursorSources.TryGetValue(CursorState,out var value)) {
                return Rectangle.Empty;
            }
            return value;
        }

        public readonly Dictionary<CursorState,Rectangle> CursorSources = new();

        protected override void OnActivated(object sender,EventArgs args) {
            base.OnActivated(sender,args);
            UpdateCursorDisplay();
        }
        protected override void OnDeactivated(object sender,EventArgs args) {
            base.OnDeactivated(sender,args);
            UpdateCursorDisplay();
        }
    }
}
