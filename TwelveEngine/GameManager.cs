using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    public sealed class GameManager:Game {
        private readonly GraphicsDeviceManager graphics;
        private SpriteBatch spriteBatch;

        public GameManager() {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;
            base.Initialize();
        }

        Texture2D testTexture;

        protected override void LoadContent() {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            testTexture = Content.Load<Texture2D>("hello-world");
        }

        private bool BackButtonIsPressed() {
            return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
        }
        private bool EscapeKeyIsPressed() {
            return Keyboard.GetState().IsKeyDown(Keys.Escape);
        }

        protected override void Update(GameTime gameTime) {
            if(BackButtonIsPressed() || EscapeKeyIsPressed()) Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.LightGray);

            spriteBatch.Begin();
            Graphics.DrawCentered(spriteBatch,testTexture,this);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
