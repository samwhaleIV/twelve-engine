﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace TwelveEngine {
    public sealed class GameManager:Game {

        private GameState pendingGameState = null;
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private void initialize() {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = Constants.ContentRootDirectory;
            IsMouseVisible = true;
            graphicsDeviceManager.SynchronizeWithVerticalRetrace = true;
            IsFixedTimeStep = false;
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
        public AutomationAgent AutomationAgent => automationAgent;

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
            if(pendingGameState == null) {
                return;
            }
            setGameState(pendingGameState);
            pendingGameState = null;
        }

        private bool gameIsReady() {
            return !(loading || hasNullState());
        }

        private bool recordingKeyPressed = false;
        private void handleRecording(KeyboardState keyboardState) {
            var keyActive = keyboardState.IsKeyDown(Constants.RecordingKey);
            if(!keyActive) {
                recordingKeyPressed = false;
                return;
            }
            if(recordingKeyPressed) return;
            recordingKeyPressed = true;

            if(!automationAgent.PlaybackActive) {
                if(automationAgent.RecordingActive) {
                    var folder = Constants.PlaybackFolder;
                    var path = $"{folder}\\{DateTime.Now.ToFileTimeUtc()}.{Constants.PlaybackFileExtension}";
                    if(!Directory.Exists(folder)) {
                        Directory.CreateDirectory(folder);
                    }
                    automationAgent.StopRecording(path);
                    Debug.WriteLine($"Recording saved to '{path}'.");
                } else {
                    automationAgent.StartRecording();
                    Debug.WriteLine("Recording started...");
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

        private bool playbackKeyPressed = false;
        private void handlePlayback(KeyboardState keyboardState) {
            var keyActive = keyboardState.IsKeyDown(Constants.PlaybackKey);
            if(!keyActive) {
                playbackKeyPressed = false;
                return;
            }
            if(playbackKeyPressed) return;
            playbackKeyPressed = true;

            if(!automationAgent.RecordingActive) {
                if(automationAgent.PlaybackActive) {
                    automationAgent.StopPlayback();
                    Debug.WriteLine("Playback stopped manually (more frames exist). Warning: Time skipping may have caused instability.");
                } else {
                    var file = getPlaybackFile();
                    if(string.IsNullOrWhiteSpace(file)) {
                        Debug.WriteLine("No recent playback file found.");
                        return;
                    }
                    var playbackFile = getPlaybackFile();
                    automationAgent.StartPlayback(playbackFile);
                    Debug.WriteLine($"Playing input file '{playbackFile}'.");
                }
            }
        }

        private GameTime gameTimeMask;
        protected override void Update(GameTime gameTime) {
            if(!gameIsReady()) return;

            var trueState = Keyboard.GetState();
            handleRecording(trueState);
            handlePlayback(trueState);

            automationAgent.StartFrame();
            gameTimeMask = automationAgent.GetGameTime(gameTime);
            this.gameState.Update(gameTimeMask);
            automationAgent.EndFrame();
        }

        protected override void Draw(GameTime _) {
            if(!gameIsReady()) return;
            this.gameState.Draw(gameTimeMask);
        }
    }
}
