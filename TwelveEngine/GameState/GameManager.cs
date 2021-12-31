using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Automation;
using TwelveEngine.Input;

namespace TwelveEngine {
    public sealed partial class GameManager:Game {

        public GameManager() {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = Constants.ContentRootDirectory;
            IsMouseVisible = true;

            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;

            vcrDisplay = new VCRDisplay(this,automationAgent);

            automationAgent.PlaybackStarted += proxyGameTime.Freeze;
            automationAgent.PlaybackStopped += proxyGameTime.Unfreeze;

            automationAgent.PlaybackStarted += AutomationAgent_PlaybackStarted;
            automationAgent.PlaybackStopped += AutomationAgent_PlaybackStopped;

            keyWatcherSet = new KeyWatcherSet(getDebugControls());

            /* TODO: Load KeyBinds from a file */

            impulseHandler = new ImpulseHandler(keyBindSet);
        }

        private (Keys key, Action action)[] getDebugControls() => new (Keys key, Action action)[]{

            (KeyBinds.Playback, automationAgent.TogglePlayback),
            (KeyBinds.Recording, automationAgent.ToggleRecording),

            (KeyBinds.PauseGame, togglePaused),
            (KeyBinds.AdvanceFrame, queueAdvanceFrame),

            (KeyBinds.SaveState, saveSerialState),
            (KeyBinds.LoadState, loadSerialState)
        };

        private void AutomationAgent_PlaybackStopped() {
            IsFixedTimeStep = false;
        }

        private void AutomationAgent_PlaybackStarted() {
            TargetElapsedTime = automationAgent.GetAveragePlaybackFrameTime();
            IsFixedTimeStep = true;
        }

        /* Runtime state variables */
        private GameState gameState = null;
        private GameState pendingGameState = null;
        private SerialFrame savedState = null;

        private KeyboardState keyboardState;
        private MouseState mouseState;
        private GamePadState gamePadState;

        /* Loading, automation, and time maniuplation state */
        private bool initialized = false;
        private bool fastForwarding = false;
        private bool shouldAdvanceFrame = false;
        private int framesToSkip = 0;
        private bool gamePaused = false;
        private bool loadingState = false;

        /* Runtime fixed variables (These should not be updated after initialization) */
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;

        /* GameManager, "God class", extensions */
        private readonly GraphicsDeviceManager graphicsDeviceManager;
        private readonly VCRDisplay vcrDisplay;
        private readonly KeyWatcherSet keyWatcherSet;
        private readonly ImpulseHandler impulseHandler;
        private readonly AutomationAgent automationAgent = new AutomationAgent();
        private readonly KeyBindSet keyBindSet = new KeyBindSet();
        private readonly MouseHandler mouseHandler = new MouseHandler();
        private readonly ProxyGameTime proxyGameTime = new ProxyGameTime();
        private readonly TimeoutManager timeout = new TimeoutManager();

        /* Public access */
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public SpriteBatch SpriteBatch => spriteBatch;
        public AutomationAgent AutomationAgent => automationAgent;
        public ImpulseHandler ImpulseHandler => impulseHandler;
        public MouseHandler MouseHandler => mouseHandler;
        public KeyBindSet KeyBindSet => keyBindSet;
        public SpriteFont DefaultFont => spriteFont;

        public KeyboardState KeyboardState => keyboardState;
        public MouseState MouseState => mouseState;
        public GamePadState GamePadState => gamePadState;

        internal bool IsLoadingState => loadingState;

        public bool CancelTimeout(int ID) {
            return timeout.Remove(ID);
        }
        public int SetTimeout(Action action,TimeSpan timeout) {
            return this.timeout.Add(action,timeout,proxyGameTime.TotalGameTime);
        }
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
            var state = GamePad.GetState(Constants.GamePadIndex);
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
            return !loadingState && gameState != null;
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

        private void unloadState() {
            if(!hasState()) {
                return;
            }
            var oldGameState = gameState;
            oldGameState.Unload();
        }

        private void swapAndLoadState(GameState gameState) {
            unloadState();
            this.gameState = null;
            if(gameState == null) {
                return;
            }
            gameState.SetReferences(this);
            gameState.Load();
        }

        private Task loadingTask(Action loadOperation) {
#pragma warning disable CS0162 // Unreachable code detected
            if(Constants.DoFakeLoadingTime) {
                return Task.WhenAll(Task.Run(loadOperation),Task.Delay(Constants.FakeLoadingTime));
            } else {
                return Task.Run(loadOperation);
            }
#pragma warning restore CS0162 // Unreachable code detected
        }

        public async void SetState(GameState gameState) {
            if(loadingState) {
                return;
            }
            if(!initialized) {
                pendingGameState = gameState;
                return;
            }
            loadingState = true;

            void loadOperation() => swapAndLoadState(gameState);
            await loadingTask(loadOperation);

            this.gameState = gameState;
            loadingState = false;
        }

        public async void SetState(Func<GameState> stateGenerator) {
            if(loadingState) {
                return;
            }
            if(!initialized) {
                pendingGameState = stateGenerator.Invoke();
                return;
            }

            loadingState = true;
            GameState newGameState = null;

            async void loadState() {
                GameState gameState = await Task.Run(stateGenerator);
                await Task.Run(() => swapAndLoadState(gameState));
                newGameState = gameState;
            }
            await loadingTask(loadState);

            this.gameState = newGameState;
            loadingState = false;
        }

        protected override void Initialize() {
            initialized = true;
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        protected override void LoadContent() {
            if(Constants.DoLoadOffThreadTextures) {
                OffThreadTexture.LoadDictionary(Content);
            }
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("default-font");
            vcrDisplay.Load();
            if(pendingGameState == null) {
                return;
            }
            SetState(pendingGameState);
            pendingGameState = null;
        }

        private bool advanceFrameQueued = false;
        private void queueAdvanceFrame() {
            advanceFrameQueued = true;
        }

        private void advanceFrame(KeyboardState keyboardState,GameTime gameTime) {
            if(automationAgent.PlaybackActive && keyboardState.IsKeyDown(Keys.LeftShift)) {
                shouldAdvanceFrame = false;
                framesToSkip = Constants.ShiftFastForwardFrames;
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

            keyboardState = getKeyboardState();
            mouseState = getMouseState();
            gamePadState = getGamePadState();

            impulseHandler.Update(keyboardState,gamePadState);
            mouseHandler.Update(mouseState);

            timeout.Update(proxyGameTime.TotalGameTime);
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
#if DEBUG
            watch.Start();
#endif
            if(!hasState()) {
#if DEBUG
                watch.Stop();
                updateTime = watch.Elapsed;
                watch.Reset();
#endif
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
        }

        protected override void Draw(GameTime trueGameTime) {
#if DEBUG
            watch.Start();
#endif
            if(hasState()) {
                gameState.PreRender(proxyGameTime);
                gameState.Render(proxyGameTime);
            } else {
                GraphicsDevice.Clear(Color.Black); /* TODO: Retain the back buffer during a loading sequence */
            }
            vcrDisplay.Render(trueGameTime);
#if DEBUG
            watch.Stop();
            renderTime = watch.Elapsed;
            watch.Reset();
            renderGameTime();
#endif
        }
    }
}
