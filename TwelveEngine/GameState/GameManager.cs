using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Automation;
using TwelveEngine.Input;

namespace TwelveEngine {
    public sealed class GameManager:Game {

        private GameState gameState = null;
        private GameState pendingGameState = null;

        private bool initialized = false;
        private bool loading = false;
        public bool Loading => loading;

        private KeyboardState keyboardState;
        public KeyboardState KeyboardState => keyboardState;

        public MouseState MouseState => automationAgent.GetMouseState();

        private GraphicsDeviceManager graphicsDeviceManager;
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;

        private SpriteBatch spriteBatch;
        public SpriteBatch SpriteBatch => spriteBatch;

        private bool fastForwarding = false;
        private bool shouldAdvanceFrame = false;
        private int framesToSkip = 0;

        private VCRDisplay vcrDisplay;
        private readonly AutomationAgent automationAgent = new AutomationAgent();

        public int Frame => automationAgent.Frame;
        public bool RecordingActive => automationAgent.RecordingActive;
        public bool PlaybackActive => automationAgent.PlaybackActive;
        public string PlaybackFile => automationAgent.PlaybackFile;

        private bool gamePaused = false; /* Value must start as false */

        private readonly KeyboardHandler keyboardHandler = new KeyboardHandler();
        public KeyboardHandler KeyboardHandler => keyboardHandler;

        private KeyBinds keyBinds = new KeyBinds();
        public KeyBinds KeyBinds => keyBinds;

        public bool IsKeyDown(KeyBind type) => keyboardState.IsKeyDown(keyBinds.Get(type));

        private DateTime pauseTimeStart;
        private readonly ProxyGameTime proxyGameTime = new ProxyGameTime();

        private SpriteFont spriteFont;

        public bool Paused {
            get => gamePaused;
            set {
                if(gamePaused == value) {
                    return;
                }
                if(!gamePaused) {
                    pauseTimeStart = DateTime.Now;
                    proxyGameTime.Freeze();
                } else {
                    proxyGameTime.AddPauseTime(DateTime.Now - pauseTimeStart);
                    proxyGameTime.Unfreeze();
                }
                gamePaused = value;
            }
        }

        public void TogglePaused() => Paused = !gamePaused;

        private KeyWatcherSet keyWatcherSet;

        private void initialize() {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = Constants.ContentRootDirectory;
            IsMouseVisible = true;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
            vcrDisplay = new VCRDisplay(this,automationAgent);
            automationAgent.PlaybackStopped += proxyGameTime.UnlockTime;

            keyWatcherSet = new KeyWatcherSet(
                new KeyWatcher(Constants.PlaybackKey,togglePlayback),
                new KeyWatcher(Constants.RecordingKey,toggleRecording),
                new KeyWatcher(Constants.AdvanceFrame,advanceFrame),
                new KeyWatcher(Constants.PauseGame,TogglePaused),
                new KeyWatcher(Constants.SaveState,saveSerialState),
                new KeyWatcher(Constants.LoadState,loadSerialState)
            );
        }

        public GameManager() => initialize();

        public GameManager(GameState gameState) {
            initialize();
            pendingGameState = gameState;
        }

        private byte[] savedState = null;
        private void saveSerialState() { /* This could be made async, but the game could change
                                    * during the saving process, creating an unstable save state */
            if(!hasGameState()) {
                return;
            }
            var frame = new SerialFrame();
            gameState.Export(frame);
            savedState = frame.Export();
        }
        private void loadSerialState() {
            if(!hasGameState() || savedState == null) {
                return;
            }
            var frame = new SerialFrame(savedState);
            gameState.Import(frame);
        }

        private bool hasGameState() => gameState != null;

        private void unloadGameState() {
            if(!hasGameState()) {
                return;
            }
            var oldGameState = gameState;
            oldGameState.Unload();
            oldGameState.Game = null;
        }

        private void loadGameState(GameState gameState) {
            unloadGameState();
            if(gameState == null) {
                return;
            }
            gameState.Game = this;
            gameState.Load();
            this.gameState = gameState;
        }

        public async void SetGameState(GameState gameState) {
            if(!initialized) {
                pendingGameState = gameState;
                return;
            }
            if(loading) {
                return;
            }
            loading = true;
            var startTime = DateTime.Now;
            await Task.WhenAll(
                Task.Run(() => loadGameState(gameState)),
                Task.Delay(Constants.MinimumLoadTime)
            );
            proxyGameTime.AddPauseTime(DateTime.Now - startTime);
            loading = false;
        }

        protected override void Initialize() {
            initialized = true;
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        protected override void LoadContent() {
            Game2D.CollisionTypes.LoadTypes(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            spriteFont = Content.Load<SpriteFont>("default-font");
            vcrDisplay.Load();
            if(pendingGameState == null) {
                return;
            }
            SetGameState(pendingGameState);
            pendingGameState = null;
        }

        private bool gameIsReady() => !loading && hasGameState();

        private void stopRecording() {
            Task.Run(async () => {
                var path = IO.PrepareOutputPath();
                await automationAgent.StopRecording(path);
                Debug.WriteLine($"Recording saved to '{path}'.");
            });
        }

        private void toggleRecording() {
            if(!automationAgent.PlaybackActive) {
                if(automationAgent.RecordingActive) {
                    stopRecording();
                } else {
                    automationAgent.StartRecording();
                }
            }
        }

        private void startPlayback() {
            var file = IO.GetPlaybackFile();
            if(string.IsNullOrWhiteSpace(file)) {
                return;
            }
            Task.Run(async () => {
                await automationAgent.StartPlayback(file);
                Debug.WriteLine($"Playing input file '{file}'");
            });
        }

        private void togglePlayback() {
            if(!automationAgent.RecordingActive) {
                if(automationAgent.PlaybackLoading) {
                    return;
                }
                if(automationAgent.PlaybackActive) {
                    automationAgent.StopPlayback();
                } else {
                    startPlayback();
                    proxyGameTime.LockTime();
                }
            }
        }

        private void advanceFrame(KeyboardState keyboardState,GameTime gameTime) {
            if(automationAgent.PlaybackActive && keyboardState.IsKeyDown(Keys.LeftShift)) {
                shouldAdvanceFrame = false;
                framesToSkip = Constants.ShiftFastForwardFrames;
                vcrDisplay.AdvanceFramesMany(gameTime);
            } else if(gamePaused && !shouldAdvanceFrame) {
                var simTime = TimeSpan.FromMilliseconds(Constants.SimFrameTime);
                pauseTimeStart += simTime;
                proxyGameTime.AddSimframe(simTime);
                shouldAdvanceFrame = true;
                vcrDisplay.AdvanceFrame(gameTime);
                Debug.WriteLine($"Advanced to frame {automationAgent.Frame + 1}");
            }
        }

        private void updateGame(GameTime trueGameTime) {
            proxyGameTime.Update(trueGameTime);
            automationAgent.StartUpdate();
            if(automationAgent.PlaybackActive) {
                proxyGameTime.SetPlaybackTime(automationAgent.GetFrameTime());
            }
            if(automationAgent.RecordingActive) {
                automationAgent.UpdateRecordingFrame(proxyGameTime);
            }
            keyboardState = automationAgent.GetKeyboardState();
            keyboardHandler.Update(keyboardState);
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

            var limit = automationAgent.Frame + count;
            var playbackFrameCount = automationAgent.PlaybackFrameCount;

            if(playbackFrameCount.HasValue) {
                limit = Math.Min(playbackFrameCount.Value,limit);
            }

            for(var i = automationAgent.Frame;i < limit;i++) {
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
            if(!gameIsReady()) {
                return;
            }
            keyWatcherSet.Process(Keyboard.GetState(),gameTime);
            if(gamePaused) {
                if(shouldAdvanceFrame) {
                    updateGame(gameTime);
                    shouldAdvanceFrame = false;
                    return;
                }
                if(framesToSkip > 0) {
                    fastForward();
                }
                return;
            }
            updateGame(gameTime);
        }

        private void renderGameTime() {
            var position = new Vector2();
            position.X = 2;
            position.Y = GraphicsDevice.Viewport.Height - 21;
            SpriteBatch.Begin();
            spriteBatch.DrawString(spriteFont,proxyGameTime.TotalGameTime.ToString(),position,Color.White);
            spriteBatch.End();
        }

        protected override void Draw(GameTime trueGameTime) {
            if(gameIsReady()) {
                this.gameState.Draw(proxyGameTime);
            } else {
                GraphicsDevice.Clear(Color.Black);
            }
            vcrDisplay.Render(trueGameTime);
#if DEBUG
            renderGameTime();
#endif
        }
    }
}
