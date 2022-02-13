using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Input;
using TwelveEngine.Shell.Automation;
using TwelveEngine.Serial;
using TwelveEngine.Shell.States;
using TwelveEngine.Shell.UI;
using TwelveEngine.Shell.Config;

namespace TwelveEngine.Shell {
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

            keyWatcherSet = new HotkeySet(GetDebugControls(keyBindSet));

            debugWriter = new DebugWriter(this);
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

        private (Keys key, Action action)[] GetDebugControls(KeyBindSet keyBindSet) => new (Keys, Action)[]{
            (keyBindSet.Playback, automationAgent.TogglePlayback),
            (keyBindSet.Recording, automationAgent.ToggleRecording),

            (keyBindSet.PauseGame, TogglePaused),
            (keyBindSet.AdvanceFrame, QueueAdvanceFrame),

            (keyBindSet.SaveState, SaveSerialState),
            (keyBindSet.LoadState, LoadSerialState)
        };

        private void AutomationAgent_PlaybackStopped() {
            IsFixedTimeStep = false;
        }

        private void AutomationAgent_PlaybackStarted() {
            TargetElapsedTime = automationAgent.GetAveragePlaybackFrameTime();
            IsFixedTimeStep = true;
        }

        /* State fields */
        private SerialFrame SavedState { get; set; }
        private bool HasSavedState => SavedState != null;

        private GameState PendingGameState { get; set; }
        private GameState GameState { get; set; }

        internal SpriteBatchSettings SpriteBatchSettings {
            get {
                if(HasPendingState) {
                    return PendingGameState.SpriteBatchSettings;
                }
                if(HasGameState) {
                    return GameState.SpriteBatchSettings;
                }
                return null;
            }
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

        private readonly AutomationAgent automationAgent = new AutomationAgent();
        private readonly ProxyGameTime proxyGameTime = new ProxyGameTime();

        private readonly DebugWriter debugWriter;

        /* Public access */
        public GraphicsDeviceManager GraphicsDeviceManager => graphicsDeviceManager;
        public AutomationAgent AutomationAgent => automationAgent;
        public KeyBinds KeyBinds => keyBinds;
        public GameTime Time => proxyGameTime;

        public SmartSpriteBatch SpriteBatch { get; private set; }
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

        private GamePadState GetGamepadState() {
            var state = GamePad.GetState(Constants.Config.GamePadIndex,GamePadDeadZone.None);
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

        private void SaveSerialState() {
            if(!HasGameState) {
                return;
            }
            var frame = new SerialFrame();
            GameState.Export(frame);
            SavedState = frame;
        }

        private void LoadSerialState() {
            if(!HasGameState || !HasSavedState) {
                return;
            }
            SavedState.StartReadback();
            GameState.Import(SavedState);
        }

        private Func<GameState> pendingStateGenerator = null;

        private bool HasPendingState => PendingGameState != null;

        public void SetState(Func<GameState> stateGenerator) {
            if(HasPendingState || pendingStateGenerator != null) {
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

            GameState oldState = GameState, newGameState;
            GameState = null;

            oldState?.Unload();
            newGameState = stateGenerator.Invoke();
            newGameState?.Load(this);

            PendingGameState = newGameState;
        }
        
        public void SetState(GameState state) => SetState(() => state);

        public void SetState<TState>() where TState : GameState, new() => SetState(new TState());

        public void SetState<TState, TData>(TData data) where TState : DataGameState<TData>, new() {
            GameState load() {
                var state = new TState();
                state.SetData(data);
                return state;
            }
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
            string[] cpuTextures = Constants.Config.CPUTextures;
            if(cpuTextures.Length > 0) {
                CPUTexture.LoadDictionary(Content,cpuTextures);
            }
            SpriteBatch = new SmartSpriteBatch(this);
            RenderTargets = new RenderTargetStack(GraphicsDevice);
            DebugFont = Content.Load<SpriteFont>(Constants.DebugFont);
            vcrDisplay.Load();
            initialized = true;
            OnLoad?.Invoke(this);
        }

        protected override void UnloadContent() {
            GameState?.Unload();
            GameState = null;

            PendingGameState?.Unload();
            PendingGameState = null;

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

                Debug.WriteLine($"Advanced to frame {automationAgent.FrameNumber + 1}");
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

            GameState.Update(proxyGameTime);

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

        private bool HasGameState => GameState != null;

        protected override void Update(GameTime gameTime) {
            updating = true;
#if DEBUG
            watch.Start();
#endif
            if(!HasGameState) {
#if DEBUG
                watch.Stop();
                updateTime = watch.Elapsed;
                watch.Reset();
#endif
                if(HasPendingState) {
                    GameState newState = PendingGameState;
                    PendingGameState = null;
                    GameState = newState;
                    Update(gameTime);
                }

                updating = false;
                return;
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
#if DEBUG
            watch.Stop();
            updateTime = watch.Elapsed;
            watch.Reset();
#endif
            updating = false;
            if(pendingStateGenerator != null) {
                Func<GameState> generator = pendingStateGenerator;
                pendingStateGenerator = null;
                SetState(generator);
            }
        }

        protected override void Draw(GameTime trueGameTime) {
            rendering = true;
#if DEBUG
            watch.Start();
#endif
            if(HasGameState) {
                GameState.PreRender(proxyGameTime);
                GameState.Render(proxyGameTime);
            } else {
                GraphicsDevice.Clear(Color.Black);
            }
            vcrDisplay.Render(trueGameTime);
#if DEBUG

            watch.Stop();
            renderTime = watch.Elapsed;
            watch.Reset();

            SpriteBatch.SamplerState = SamplerState.LinearClamp;
            SpriteBatch.Begin();
            DrawGameTimeDebug(debugWriter);
            GameState?.WriteDebug(debugWriter);
            SpriteBatch.End();
#endif
            rendering = false;
        }
    }
}
