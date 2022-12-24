using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Input;
using TwelveEngine.Shell.Automation;
using TwelveEngine.Shell.Config;
using System.Collections.Generic;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.Shell {
    public sealed partial class GameManager:Game {

        private bool drawDebug = false;

        public GameManager(
            bool fullscreen = false,bool hardwareModeSwitch = false,bool verticalSync = true,bool drawDebug = false
        ) {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            this.drawDebug = drawDebug;

            Logger.WriteBooleanSet("Context settings",new string[] {
                "Fullscreen","HardwareModeSwitch","VerticalSync"
            },new bool[] {
                fullscreen, hardwareModeSwitch, verticalSync
            });

            Content.RootDirectory = Constants.ContentDirectory;

            graphicsDeviceManager.SynchronizeWithVerticalRetrace = verticalSync;

            graphicsDeviceManager.IsFullScreen = fullscreen;
            graphicsDeviceManager.HardwareModeSwitch = hardwareModeSwitch;

            IsFixedTimeStep = false;

            vcrDisplay = new VCRDisplay(this,automationAgent);

            automationAgent.PlaybackStarted += proxyGameTime.Freeze;
            automationAgent.PlaybackStopped += proxyGameTime.Unfreeze;

            automationAgent.PlaybackStarted += AutomationAgent_PlaybackStarted;
            automationAgent.PlaybackStopped += AutomationAgent_PlaybackStopped;

            keyBinds = KeyBinds.Load(out KeyBindSet keyBindSet);

            keyWatcherSet = new HotkeySet(GetDebugControls(keyBindSet));

            if(drawDebug) {
                debugWriter = new DebugWriter(this);
            }
        }

        private readonly Stopwatch watch = new();
        private TimeSpan updateTime, renderTime;

        private readonly FPSCounter fpsCounter = new();

        private void DrawGameTimeDebug(DebugWriter writer) {
            writer.ToBottomLeft();

            writer.WriteTimeMS(renderTime,"Render");
            writer.WriteTimeMS(updateTime,"Update");

            fpsCounter.Update(proxyGameTime.TotalGameTime);
            writer.WriteFPS(fpsCounter.Value);

            writer.Write(proxyGameTime.TotalGameTime);
        }

        private (Keys key, Action action)[] GetDebugControls(KeyBindSet keyBindSet) => new (Keys, Action)[]{
            (keyBindSet.Playback, automationAgent.TogglePlayback),
            (keyBindSet.Recording, automationAgent.ToggleRecording),

            (keyBindSet.PauseGame, TogglePaused),
            (keyBindSet.AdvanceFrame, QueueAdvanceFrame)
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
        private readonly KeyBinds keyBinds;

        private readonly AutomationAgent automationAgent = new();
        private readonly ProxyGameTime proxyGameTime = new();
        private readonly DebugWriter debugWriter;

        /* Public access */
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public AutomationAgent AutomationAgent => automationAgent;
        public KeyBinds KeyBinds => keyBinds;
        public GameTime Time => proxyGameTime;

        public SpriteBatch SpriteBatch { get; private set; }
        public SpriteFont DebugFont { get; private set; }
        private RenderTargetStack RenderTargets { get; set; }

        public KeyboardState KeyboardState { get; private set; }
        public MouseState MouseState { get; private set; }
        public GamePadState GamePadState { get; private set; }

        public void SetRenderTarget(RenderTarget2D renderTarget) => RenderTargets.Push(renderTarget);
        public void RestoreRenderTarget() => RenderTargets.Pop();
        public Viewport Viewport => RenderTargets.GetViewport();

        public bool IsPaused {
            get => gamePaused;
            set => SetPaused(value);
        }

        private void SetPaused(bool paused) {
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

        private void TogglePaused() => SetPaused(!gamePaused);

        private static GamePadState GetGamepadState() {
            var state = GamePad.GetState(Constants.Config.GamePadIndex,GamePadDeadZone.Circular);
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
        private GameState _pendingGameState = null;
        private GameState _gameState = null;
        private Func<GameState> pendingStateGenerator = null;

        private void LoadState(GameState state) {
            GraphicsDevice.Reset();
            state.Load(this);
        }

        public void SetState(Func<GameState> stateGenerator) {
            if(_pendingGameState != null || pendingStateGenerator != null) {
                /* Recursive loading! Preload all your assets to your heart's content! */
                pendingStateGenerator = null;
                _pendingGameState?.Unload();
                _pendingGameState = stateGenerator.Invoke();
                LoadState(_pendingGameState);
                return;
            }
            if(!initialized) {
                throw new InvalidOperationException("Cannot change GameState until GameManager has loaded all of its own content.");
            }
            if(rendering) {
                throw new InvalidOperationException("Cannot change a GameState during a Draw operation.");
            }
            if(updating) {
                pendingStateGenerator = stateGenerator;
                return;
            }

            GameState oldState = _gameState;
            _gameState = null;

            oldState?.Unload();
            _pendingGameState = stateGenerator.Invoke();
            LoadState(_pendingGameState);
            string stateName = _pendingGameState.Name;
            Logger.WriteLine($"[{proxyGameTime.TotalGameTime}] Loaded game state - {(string.IsNullOrWhiteSpace(stateName) ? "<No Name>" : stateName)}");
        }
        
        public void SetState(GameState state) => SetState(() => state);

        public void SetState<TState>() where TState : GameState, new() => SetState(new TState());

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
            Texture2D emptyTexture = new Texture2D(GraphicsDevice,size,size);
            Color[] pixels = new Color[pixelCount];
            for(int i = 0;i<pixelCount;i++) {
                pixels[i] = Color.White;
            }
            emptyTexture.SetData(pixels);
            EmptyTexture = emptyTexture;
        }

        protected override void LoadContent() {
            string[] cpuTextures = Constants.Config.CPUTextures;
            if(cpuTextures.Length > 0) {
                CPUTexture.LoadDictionary(Content,cpuTextures);
            }
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

            _pendingGameState?.Unload();
            _pendingGameState = null;

            SpriteBatch?.Dispose();
            SpriteBatch = null;

            Content.Dispose();
            graphicsDeviceManager.Dispose();
        }

        private bool advanceFrameQueued = false;
        private void QueueAdvanceFrame() => advanceFrameQueued = true;

        private void AdvanceFrame(KeyboardState keyboardState,GameTime gameTime) {
            if(automationAgent.PlaybackActive && keyboardState.IsKeyDown(Keys.LeftShift)) {

                shouldAdvanceFrame = false;
                framesToSkip = Constants.ShiftFrameSkip;
                vcrDisplay.AdvanceFramesMany(gameTime);

            } else if(gamePaused && !shouldAdvanceFrame) {

                TimeSpan simTime = TimeSpan.FromMilliseconds(Constants.SimFrameTime);
                proxyGameTime.AddSimTime(simTime);
                shouldAdvanceFrame = true;
                vcrDisplay.AdvanceFrame(gameTime);

                Console.WriteLine($"[Automation Agent] Advanced to frame {automationAgent.FrameNumber + 1}");
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
            if(framesToSkip >= 1) {
                FastForward();
            }
        }

        private void FastForward() {
            if(fastForwarding) return;
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

        protected override void Update(GameTime gameTime) {
            updating = true;
            watch.Start();
            if(!HasGameState) {
                if(_pendingGameState != null) {
                    _gameState = _pendingGameState;
                    _pendingGameState = null;
                } else {
                    watch.Stop();
                    updateTime = watch.Elapsed;
                    watch.Reset();
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
            watch.Stop();
            updateTime = watch.Elapsed;
            watch.Reset();
            updating = false;
        }

        private void TryApplyPendingStateGenerator() {
            if(pendingStateGenerator == null) {
                return;
            }
            watch.Start();
            Func<GameState> generator = pendingStateGenerator;
            pendingStateGenerator = null;
            SetState(generator);
            GC.Collect(GC.MaxGeneration,GCCollectionMode.Forced,true);
            watch.Stop();
            Logger.WriteLine($"GC and state swap, elapsed time: {watch.Elapsed}");
            watch.Reset();
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

            if(drawDebug) {
                DrawGameTimeDebug(debugWriter);
                gameState?.WriteDebug(debugWriter);
            }

            SpriteBatch.End();
            if(_customCursorTexture != null) {
                DrawCustomCursor();
            }

            watch.Stop();
            updateTime = watch.Elapsed;
            watch.Reset();

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
