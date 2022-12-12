﻿using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Shell.Input;
using TwelveEngine.Shell.Automation;
using TwelveEngine.Shell.States;
using TwelveEngine.Shell.UI;
using TwelveEngine.Shell.Config;
using System.Text;
using System.Collections.Generic;

namespace TwelveEngine.Shell {
    public sealed partial class GameManager:Game {

        public GameManager(
            bool fullscreen = false,bool hardwareModeSwitch = false,bool verticalSync = true,bool mouseVisible = true
        ) {
            graphicsDeviceManager = new GraphicsDeviceManager(this);

            Logger.WriteBooleanSet("Context settings",new string[] {
                "Fullscreen","HardwareModeSwitch","VerticalSync","MouseVisible"
            },new bool[] {
                fullscreen, hardwareModeSwitch, verticalSync, mouseVisible
            });

            Content.RootDirectory = Constants.ContentDirectory;
            IsMouseVisible = mouseVisible;

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

            debugWriter = new DebugWriter(this);
        }

#if DEBUG
        private readonly Stopwatch watch = new Stopwatch();
        private TimeSpan updateTime, renderTime;

        private readonly FPSCounter fpsCounter = new FPSCounter();

        private void DrawGameTimeDebug(DebugWriter writer) {
            writer.ToBottomLeft();

            writer.WriteTimeMS(renderTime,"Render");
            writer.WriteTimeMS(updateTime,"Update");

            fpsCounter.Update(proxyGameTime.TotalGameTime);
            writer.WriteFPS(fpsCounter.Value);

            writer.Write(proxyGameTime.TotalGameTime);
        }
#endif

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

        /* State fields */
        private GameState _pendingGameState = null;
        private GameState _gameState = null;

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

        private GamePadState GetGamepadState() {
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

        private Func<GameState> pendingStateGenerator = null;

        private bool HasPendingState => _pendingGameState != null;

        private void LoadState(GameState state) {
            GraphicsDevice.Reset();
            state.Load(this);
        }

        public void SetState(Func<GameState> stateGenerator) {
            if(HasPendingState || pendingStateGenerator != null) {
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
                    GameState newState = _pendingGameState;
                    _pendingGameState = null;
                    _gameState = newState;
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
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        protected override void Draw(GameTime trueGameTime) {
            rendering = true;
#if DEBUG
            watch.Start();
#endif
            if(HasGameState) {
                _gameState.ResetGraphicsState(GraphicsDevice);
                _gameState.PreRender();
                _gameState.Render();
            } else {
                GraphicsDevice.Clear(Color.Black);
                /* Notice: No game state */
            }
            vcrDisplay.Render(trueGameTime);
#if DEBUG

            watch.Stop();
            renderTime = watch.Elapsed;
            watch.Reset();

            SpriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.LinearClamp);
            DrawGameTimeDebug(debugWriter);
            _gameState?.WriteDebug(debugWriter);
            SpriteBatch.End();
#endif
            rendering = false;
        }
    }
}
