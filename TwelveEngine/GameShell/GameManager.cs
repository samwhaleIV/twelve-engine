using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Config;
using TwelveEngine.Input;
using TwelveEngine.GameShell;
using TwelveEngine.GameShell.Automation;
using TwelveEngine.Serial;
using TwelveEngine.GameUI;

namespace TwelveEngine {
    public sealed partial class GameManager:Game {

        public GameManager() {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Content.RootDirectory = Constants.ContentDirectory;
            IsMouseVisible = true;

            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;

            vcrDisplay = new VCRDisplay(this,automationAgent);

            automationAgent.PlaybackStarted += proxyGameTime.Freeze;
            automationAgent.PlaybackStopped += proxyGameTime.Unfreeze;

            automationAgent.PlaybackStarted += AutomationAgent_PlaybackStarted;
            automationAgent.PlaybackStopped += AutomationAgent_PlaybackStopped;

            keyBinds = KeyBinds.Load(out KeyBindSet keyBindSet);

            impulseHandler = new ImpulseHandler(keyBinds);
            keyWatcherSet = new KeyWatcherSet(getDebugControls(keyBindSet));

            debugWriter = new DebugWriter(this);
#if DEBUG
            OnWriteDebug += DrawGameTimeDebug;
#endif
        }

#if DEBUG
        private readonly Stopwatch watch = new Stopwatch();
        private TimeSpan updateTime, renderTime;

        private double GetFps() {
            var totalSeconds = proxyGameTime.ElapsedGameTime.TotalSeconds;
            if(totalSeconds == 0d) {
                return 0d;
            }
            return 1d / totalSeconds;
        }

        private void DrawGameTimeDebug(DebugWriter writer) {
            writer.ToBottomLeft();

            writer.WriteTimeMS(renderTime,"Render");
            writer.WriteTimeMS(updateTime,"Update");

            writer.WriteFPS(GetFps());
            writer.Write(proxyGameTime.TotalGameTime);
        }
#endif

        private (Keys key, Action action)[] getDebugControls(KeyBindSet keyBindSet) => new (Keys, Action)[]{
            (keyBindSet.Playback, automationAgent.TogglePlayback),
            (keyBindSet.Recording, automationAgent.ToggleRecording),

            (keyBindSet.PauseGame, togglePaused),
            (keyBindSet.AdvanceFrame, queueAdvanceFrame),

            (keyBindSet.SaveState, saveSerialState),
            (keyBindSet.LoadState, loadSerialState)
        };

        private void AutomationAgent_PlaybackStopped() {
            IsFixedTimeStep = false;
        }

        private void AutomationAgent_PlaybackStarted() {
            TargetElapsedTime = automationAgent.GetAveragePlaybackFrameTime();
            IsFixedTimeStep = true;
        }

        /* State fields */
        private GameState gameState = null;
        private SerialFrame savedState = null;

        /* Loading, automation, and time maniuplation state */
        private bool initialized = false, gamePaused = false, updating = false, rendering = false;

        private bool fastForwarding = false, shouldAdvanceFrame = false;
        private int framesToSkip = 0;

        /* God class extensions */
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        private readonly VCRDisplay vcrDisplay;
        private readonly KeyWatcherSet keyWatcherSet;
        private readonly ImpulseHandler impulseHandler;
        private readonly KeyBinds keyBinds;

        private readonly AutomationAgent automationAgent = new AutomationAgent();
        private readonly MouseHandler mouseHandler = new MouseHandler();
        private readonly ProxyGameTime proxyGameTime = new ProxyGameTime();

        private readonly DebugWriter debugWriter;
        public event Action<DebugWriter> OnWriteDebug;

        /* Public access */
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public AutomationAgent AutomationAgent => automationAgent;
        public ImpulseHandler ImpulseHandler => impulseHandler;
        public MouseHandler MouseHandler => mouseHandler;
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
            set => setPaused(value);
        }

        private void setPaused(bool paused) {
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

        private void togglePaused() => setPaused(!gamePaused);

        private GamePadState getGamePadState() {
            var state = GamePad.GetState(Constants.Config.GamePadIndex,GamePadDeadZone.None);
            return state;
        }

        private KeyboardState getKeyboardState() {
            var state = Keyboard.GetState();
            return automationAgent.FilterKeyboardState(state);
        }

        private MouseState getMouseState() {
            var state = Mouse.GetState();
            return automationAgent.FilterMouseState(state);
        }

        private bool hasState() {
            return gameState != null;
        }

        private void saveSerialState() {
            if(!hasState()) {
                return;
            }
            var frame = new SerialFrame();
            gameState.Export(frame);
            savedState = frame;
        }
        private void loadSerialState() {
            if(!hasState() || savedState == null) {
                return;
            }
            savedState.StartReadback();
            gameState.Import(savedState);
        }

        private GameState pendingGameState = null;
        private Func<GameState> pendingStateGenerator = null;

        public void SetState(Func<GameState> stateGenerator) {
            if(pendingGameState != null || pendingStateGenerator != null) {
                throw new InvalidOperationException("A GameState has already been queued. Cannot override the previous SetState; Load only one GameState at a time.");
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

            GameState oldState = gameState, newGameState;
            gameState = null;

            if(oldState != null) {
                oldState.Unload();
            }
            newGameState = stateGenerator.Invoke();
            if(newGameState != null) {
                newGameState.Load(this);
            }

            pendingGameState = newGameState;
        }
        
        public void SetState(GameState state) => SetState(() => state);

        public void SetState<TState>() where TState : GameState, new() => SetState(new TState());

        public void SetState<TState,TData>(TData data) where TState : DataGameState<TData>, new() {
            Func<GameState> load = () => {
                var state = new TState();
                state.SetData(data);
                return state;
            };
            SetState(load);
        }

        public void SetState<TState>(string data) where TState : DataGameState<string>, new() => SetState<TState,string>(data);

        protected override void Initialize() {
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        public event Action<GameManager> OnLoad;

        protected override void LoadContent() {
            var cpuTextures = Constants.Config.CPUTextures;
            if(cpuTextures.Length > 0) {
                CPUTexture.LoadDictionary(Content,cpuTextures);
            }
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            RenderTargets = new RenderTargetStack(GraphicsDevice);
            DebugFont = Content.Load<SpriteFont>(Constants.DebugFont);
            vcrDisplay.Load();
            initialized = true;
            OnLoad?.Invoke(this);
        }

        protected override void UnloadContent() {
            if(gameState != null) {
                gameState.Unload();
                gameState = null;
            }
            if(pendingGameState != null) {
                pendingGameState.Unload();
                pendingGameState = null;
            }
            Content.Dispose();
            if(SpriteBatch != null) {
                SpriteBatch.Dispose();
                SpriteBatch = null;
            }
            graphicsDeviceManager.Dispose();
        }

        private bool advanceFrameQueued = false;
        private void queueAdvanceFrame() => advanceFrameQueued = true;

        private void advanceFrame(KeyboardState keyboardState,GameTime gameTime) {
            if(automationAgent.PlaybackActive && keyboardState.IsKeyDown(Keys.LeftShift)) {

                shouldAdvanceFrame = false;
                framesToSkip = Constants.ShiftFrameSkip;
                vcrDisplay.AdvanceFramesMany(gameTime);

            } else if(gamePaused && !shouldAdvanceFrame) {

                var simTime = TimeSpan.FromMilliseconds(Constants.SimFrameTime);
                proxyGameTime.AddSimTime(simTime);
                shouldAdvanceFrame = true;
                vcrDisplay.AdvanceFrame(gameTime);

                Debug.WriteLine($"Advanced to frame {automationAgent.FrameNumber + 1}");
            }
        }

        private void updateGame(GameTime trueGameTime) {
            proxyGameTime.Update(trueGameTime);
            automationAgent.StartUpdate();

            if(automationAgent.PlaybackActive) {
                proxyGameTime.AddSimTime(automationAgent.GetFrameTime());
            }
            if(automationAgent.RecordingActive) {
                automationAgent.UpdateRecordingFrame(proxyGameTime);
            }

            KeyboardState = getKeyboardState();
            MouseState = getMouseState();
            GamePadState = getGamePadState();

            mouseHandler.Update(MouseState);
            impulseHandler.Update(KeyboardState,GamePadState);

            gameState.Update(proxyGameTime);

            automationAgent.EndUpdate();
            if(framesToSkip > 0) {
                fastForward();
            }
        }

        private void fastForward() {
            if(fastForwarding) return;
            fastForwarding = true;
            var count = framesToSkip;
            framesToSkip = 0;

            var limit = automationAgent.FrameNumber + count;
            var playbackFrameCount = automationAgent.PlaybackFrameCount;

            if(playbackFrameCount.HasValue) {
                limit = Math.Min(playbackFrameCount.Value,limit);
            }

            for(var i = automationAgent.FrameNumber;i < limit;i++) {
                updateGame(null); /* Automation agent supplies a game time when playback is active */
            }

            fastForwarding = false;
        }

        public void FastForward(int count) {
            if(!automationAgent.PlaybackActive) { /* Confused? See above. */
                return;
            }
            framesToSkip = count;
        }

        protected override void Update(GameTime gameTime) {
            updating = true;
#if DEBUG
            watch.Start();
#endif
            if(!hasState()) {
#if DEBUG
                watch.Stop();
                updateTime = watch.Elapsed;
                watch.Reset();
#endif
                if(pendingGameState != null) {
                    var newState = pendingGameState;
                    pendingGameState = null;
                    gameState = newState;
                    Update(gameTime);
                }

                updating = false;
                return;
            }
            var vanillaKeyboardState = Keyboard.GetState();
            keyWatcherSet.Update(vanillaKeyboardState);
            if(advanceFrameQueued) {
                advanceFrame(vanillaKeyboardState,gameTime);
                advanceFrameQueued = false;
            }

            if(!gamePaused) {
                updateGame(gameTime);             
            } else if(shouldAdvanceFrame) {
                updateGame(gameTime);
                shouldAdvanceFrame = false;
            } else if(framesToSkip > 0) {
                fastForward();
            }
#if DEBUG
            watch.Stop();
            updateTime = watch.Elapsed;
            watch.Reset();
#endif
            updating = false;
            if(pendingStateGenerator != null) {
                var generator = pendingStateGenerator;
                pendingStateGenerator = null;
                SetState(generator);
            }
        }

        protected override void Draw(GameTime trueGameTime) {
            rendering = true;
#if DEBUG
            watch.Start();
#endif
            if(hasState()) {
                gameState.PreRender(proxyGameTime);
                gameState.Render(proxyGameTime);
            } else {
                GraphicsDevice.Clear(Color.Black);
            }
            vcrDisplay.Render(trueGameTime);
#if DEBUG
            watch.Stop();
            renderTime = watch.Elapsed;
            watch.Reset();

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.AnisotropicClamp,DepthStencilState.None);
            OnWriteDebug?.Invoke(debugWriter);
            SpriteBatch.End();
#endif

            rendering = false;
        }
    }
}
