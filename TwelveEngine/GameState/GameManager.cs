using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine {
    public sealed class GameManager:Game {

        private GameState pendingGameState = null;
        private GraphicsDeviceManager graphicsDeviceManager;
        private SpriteBatch spriteBatch;

        private void initialize() {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            Content.RootDirectory = Constants.ContentRootDirectory;
            IsMouseVisible = true;
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
            gameState.Game = this;
            gameState.Load(this);
        }
        public GameState GameState {
            get {
                return this.gameState;
            }
            set {
                setGameState(value);
            }
        }

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
            spriteBatch = new SpriteBatch(GraphicsDevice);
            if(pendingGameState == null) {
                return;
            }
            setGameState(pendingGameState);
            pendingGameState = null;
        }

        protected override void Update(GameTime gameTime) {
            if(hasNullState()) {
                return;
            }
            this.gameState.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            if(hasNullState()) {
                return;
            }
            this.gameState.Draw(gameTime);
        }
    }
}
