using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TwelveEngine {
    public sealed class GameManager:Game {

        private GameState pendingGameState = null;
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private VCRDisplay vcrDisplay;

        private void initialize() {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = Constants.ContentRootDirectory;
            IsMouseVisible = true;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
            vcrDisplay = new VCRDisplay(this,automationAgent);
        }

        public GameManager() {
            initialize();
        }

        public GameManager(GameState gameState) {
            initialize();
            pendingGameState = gameState;
        }

        private GameState gameState = null;
        private bool hasNullState() {
            return gameState == null;
        }

        private bool loading = false;
        public bool Loading => loading;

        private void setGameState(GameState gameState) {
            if(!initialized) {
                pendingGameState = gameState;
                return;
            }
            if(!hasNullState()) {
                this.gameState.Game = null;
                this.gameState.Unload();
            }
            this.gameState = gameState;
            if(hasNullState()) {
                return;
            }
            loading = true;
            gameState.Game = this;
            gameState.Load(this);
            loading = false;
        }
        public GameState GameState {
            get {
                return this.gameState;
            }
            set {
                setGameState(value);
            }
        }

        private readonly AutomationAgent automationAgent = new AutomationAgent();

        public int Frame => automationAgent.Frame;
        public bool RecordingActive => automationAgent.RecordingActive;
        public bool PlaybackActive => automationAgent.PlaybackActive;
        public string PlaybackFile => automationAgent.PlaybackFile;

        public KeyboardState KeyboardState => automationAgent.GetKeyboardState();
        public MouseState MouseState => automationAgent.GetMouseState();

        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public SpriteBatch SpriteBatch => spriteBatch;

        private bool initialized = false;

        protected override void Initialize() {
            initialized = true;
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        protected override void LoadContent() {
            Game2D.CollisionTypes.LoadTypes(Content);
            spriteBatch = new SpriteBatch(GraphicsDevice);
            vcrDisplay.Load();
            if(pendingGameState == null) {
                return;
            }
            setGameState(pendingGameState);
            pendingGameState = null;
        }

        private bool gameIsReady() {
            return !(loading || hasNullState());
        }

        private void stopRecording() {
            var folder = Constants.PlaybackFolder;
            var path = $"{folder}\\{DateTime.Now.ToFileTimeUtc()}.{Constants.PlaybackFileExtension}";
            if(!Directory.Exists(folder)) {
                Directory.CreateDirectory(folder);
            }
            Task.Run(async () => {
                await automationAgent.StopRecording(path);
                Debug.WriteLine($"Recording saved to '{path}'.");
            });
        }
        private void startRecording() {
            automationAgent.StartRecording();
        }

        private bool recordingKeyPressed = false;
        private void handleRecording(KeyboardState keyboardState) {
            bool keyActive = keyboardState.IsKeyDown(Constants.RecordingKey);
            if(!keyActive) {
                recordingKeyPressed = false;
                return;
            }
            if(recordingKeyPressed) return;
            recordingKeyPressed = true;

            if(!automationAgent.PlaybackActive) {
                if(automationAgent.RecordingActive) {
                    stopRecording();
                } else {
                    startRecording();
                }
            }
        }

        private string getPlaybackFile() {
            var preferredFile = Constants.PreferredPlaybackFile;
            if(!string.IsNullOrWhiteSpace(preferredFile)) {
                return preferredFile;
            }

            var defaultFile = Constants.DefaultPlaybackFile;
            if(File.Exists(defaultFile)) {
                return defaultFile;
            }

            var file = Directory.EnumerateFiles(
                Directory.GetCurrentDirectory(),$"{Constants.PlaybackFolder}\\*.{Constants.PlaybackFileExtension}"
            ).OrderByDescending(name => name).FirstOrDefault();

            return file;
        }

        private void startPlayback() {
            var file = getPlaybackFile();
            if(string.IsNullOrWhiteSpace(file)) {
                return;
            }
            var playbackFile = getPlaybackFile();
            Task.Run(async () => {
                await automationAgent.StartPlayback(playbackFile);
                Debug.WriteLine($"Playing input file '{playbackFile}'");
            });
        }
        private void stopPlayback() {
            automationAgent.StopPlayback();
        }

        private bool playbackKeyPressed = false;
        private void handlePlayback(KeyboardState keyboardState) {
            bool keyActive = keyboardState.IsKeyDown(Constants.PlaybackKey);
            if(!keyActive) {
                playbackKeyPressed = false;
                return;
            }
            if(playbackKeyPressed) return;
            playbackKeyPressed = true;

            if(!automationAgent.RecordingActive) {
                if(automationAgent.PlaybackLoading) {
                    return;
                }
                if(automationAgent.PlaybackActive) {
                    stopPlayback();
                } else {
                    startPlayback();
                }
            }
        }

        private void advanceFrame(GameTime gameTime) {
            shouldAdvanceFrame = true;
            vcrDisplay.AdvanceFrame(gameTime);
            Debug.WriteLine($"Advanced to frame {automationAgent.Frame + 1}");
        }

        private bool pauseKeyDown = false;
        private bool advanceKeyDown = false;
        private void handlePauseControls(GameTime gameTime,KeyboardState keyboardState) {
            bool pauseKey = keyboardState.IsKeyDown(Constants.PauseGame);
            bool advanceKey = keyboardState.IsKeyDown(Constants.AdvanceFrame);

            if(!pauseKey) {
                pauseKeyDown = false;
            } else if(!pauseKeyDown) {
                pauseKeyDown = true;
                TogglePaused();
            }

            if(!advanceKey) {
                advanceKeyDown = false;
            } else if(!advanceKeyDown) {
                advanceKeyDown = true;
                if(gamePaused && !shouldAdvanceFrame) {
                    advanceFrame(gameTime);
                }
            }
        }

        private GameTime pauseTimeMask;

        private bool gamePaused = false; /* Value must start as false */
        public bool Paused {
            get {
                return gamePaused;
            }
            set {
                if(gamePaused == value) {
                    return;
                }
                if(!gamePaused) {
                    var mask = new GameTime();
                    mask.ElapsedGameTime = lastElapsedTime;
                    mask.TotalGameTime = lastTotalTime;
                    pauseTimeMask = mask;
                } else {
                    pauseTimeMask = null;
                }
                gamePaused = value;
            }
        }
        public void Pause() {
            Paused = true;
        }
        public void Unpause() {
            Paused = false;
        }
        public void TogglePaused() {
            Paused = !Paused;
        }

        private bool shouldAdvanceFrame = false;

        private int framesToSkip = 0;

        private void updateGame(GameTime gameTime) {
            automationAgent.StartFrame();
            gameTimeMask = automationAgent.GetGameTime(gameTime);
            this.gameState.Update(gameTimeMask);
            automationAgent.EndFrame();
            if(framesToSkip > 0) {
                var count = framesToSkip;
                framesToSkip = 0;
                for(var i = 0;i < count;i++) {
                    updateGame(null); /* Automation agent supplies a game time when playback is active */
                }
                Debug.WriteLine($"Fast forward {count} frames of playback");
            }
        }

        public void SkipFrames(int count) {
            if(!automationAgent.PlaybackActive) { /* Confused? See above. */
                return;
            }
            framesToSkip = count;
        }

        private GameTime gameTimeMask;
        protected override void Update(GameTime gameTime) {
            if(!gameIsReady()) {
                return;
            }

            var trueState = Keyboard.GetState();
            handleRecording(trueState);
            handlePlayback(trueState);
            handlePauseControls(gameTime,trueState);

            if(gamePaused) {
                if(!shouldAdvanceFrame) {
                    return;
                }
                updateGame(gameTime);
                shouldAdvanceFrame = false;
                return;
            }
            updateGame(gameTime);
        }

        private TimeSpan lastElapsedTime;
        private TimeSpan lastTotalTime;

        private void renderVCR(GameTime gameTime) {
            SpriteBatch.Begin(SpriteSortMode.Deferred,BlendState.NonPremultiplied,SamplerState.PointClamp);
            vcrDisplay.Render(gameTime);
            SpriteBatch.End();
        }

        protected override void Draw(GameTime gameTime) {
            if(!gameIsReady()) {
                GraphicsDevice.Clear(Color.Black);
                renderVCR(gameTime);
                return;
            }

            if(!gamePaused || automationAgent.PlaybackActive) {
                this.gameState.Draw(gameTimeMask);
            } else {
                this.gameState.Draw(pauseTimeMask);
            }

            lastElapsedTime = gameTime.ElapsedGameTime;
            lastTotalTime = gameTime.TotalGameTime;

            renderVCR(gameTime);
        }
    }
}
