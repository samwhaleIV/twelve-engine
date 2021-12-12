using System;
using System.Diagnostics;
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

            keyWatcherSet = new KeyWatcherSet(
                new KeyWatcher(Constants.PlaybackKey,automationAgent.TogglePlayback),
                new KeyWatcher(Constants.RecordingKey,automationAgent.ToggleRecording),

                new KeyWatcher(Constants.AdvanceFrame,advanceFrame),
                new KeyWatcher(Constants.PauseGame,togglePaused),

                new KeyWatcher(Constants.SaveState,saveSerialState),
                new KeyWatcher(Constants.LoadState,loadSerialState)
            );
        }

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

        /* Loading, automation, and time maniuplation state */
        private bool initialized = false;
        private bool loading = false;
        private bool fastForwarding = false;
        private bool shouldAdvanceFrame = false;
        private int framesToSkip = 0;
        private bool gamePaused = false;

        /* Runtime fixed variables (These should not be updated after initialization) */
        private SpriteBatch spriteBatch;
        private SpriteFont spriteFont;
        private GraphicsDeviceManager graphicsDeviceManager;

        /* GameManager, "God class", extensions */
        private readonly VCRDisplay vcrDisplay;
        private readonly KeyWatcherSet keyWatcherSet;
        private readonly AutomationAgent automationAgent = new AutomationAgent();
        private readonly KeyboardHandler keyboardHandler = new KeyboardHandler();
        private readonly MouseHandler mouseHandler = new MouseHandler();
        private readonly ProxyGameTime proxyGameTime = new ProxyGameTime();
        private readonly TimeoutManager timeout = new TimeoutManager();
        private readonly KeyBinds keyBinds = new KeyBinds();

        /* Public access */
        public bool Loading => loading;
        public KeyboardState KeyboardState => keyboardState;
        public MouseState MouseState => mouseState;
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public SpriteBatch SpriteBatch => spriteBatch;
        public AutomationAgent AutomationAgent => automationAgent;
        public KeyboardHandler KeyboardHandler => keyboardHandler;
        public MouseHandler MouseHandler => mouseHandler;
        public KeyBinds KeyBinds => keyBinds;
        public SpriteFont DefaultFont => spriteFont;

        public bool IsKeyDown(KeyBind type) {
            return keyboardState.IsKeyDown(keyBinds.Get(type));
        }
        public bool CancelTimeout(int ID) {
            return timeout.Remove(ID);
        }
        public int SetTimeout(Action action,TimeSpan timeout) {
            return this.timeout.Add(action,timeout,proxyGameTime.TotalGameTime);
        }
        public bool Paused {
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

        private KeyboardState getKeyboardState() {
            return automationAgent.FilterKeyboardState(Keyboard.GetState());
        }
        private MouseState getMouseState() {
            return automationAgent.FilterMouseState(Mouse.GetState());
        }
        private bool hasGameState() {
            return gameState != null;
        }
        private bool gameIsReady() {
            return !loading && hasGameState();
        }

        private void saveSerialState() {
            if(!hasGameState()) {
                return;
            }
            var frame = new SerialFrame();
            gameState.Export(frame);
            savedState = frame;
        }
        private void loadSerialState() {
            if(!hasGameState() || savedState == null) {
                return;
            }
            savedState.StartReadback();
            gameState.Import(savedState);
        }

        private void unloadGameState() {
            if(!hasGameState()) {
                return;
            }
            var oldGameState = gameState;
            oldGameState.Unload();
        }

        private void loadGameState(GameState gameState) {
            unloadGameState();
            if(gameState == null) {
                return;
            }
            gameState.SetGameReference(this);
            gameState.Load();
            this.gameState = gameState;
        }

        public void SetState(GameState gameState) {
            if(!initialized) {
                pendingGameState = gameState;
                return;
            }
            if(loading) {
                return;
            }
            loading = true;
            proxyGameTime.Freeze();
            loadGameState(gameState);
            proxyGameTime.Unfreeze();
            loading = false;
        }

        protected override void Initialize() {
            initialized = true;
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("default-font");
            vcrDisplay.Load();
            if(pendingGameState == null) {
                return;
            }
            SetState(pendingGameState);
            pendingGameState = null;
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

            keyboardHandler.Update(keyboardState);
            mouseHandler.Update(mouseState);

            gameState.Update(proxyGameTime);
            timeout.Update(proxyGameTime.TotalGameTime);

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
            if(!gameIsReady()) {
#if DEBUG
                watch.Stop();
                updateTime = watch.Elapsed;
                watch.Reset();
#endif
                return;
            }

            keyWatcherSet.Process(Keyboard.GetState(),gameTime);
            if(gamePaused) {
                if(shouldAdvanceFrame) {
                    updateGame(gameTime);
                    shouldAdvanceFrame = false;
                } else if(framesToSkip > 0) {
                    fastForward();
                }
            } else {
                updateGame(gameTime);
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
            if(gameIsReady()) {
                gameState.Draw(proxyGameTime);
            } else {
                GraphicsDevice.Clear(Color.Black);
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
