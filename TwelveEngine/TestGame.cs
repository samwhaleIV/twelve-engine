using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    public class TestGame:Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        public TestGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize() {
            // TODO: Add your initialization logic here
            base.Initialize();
        }

        Texture2D testTexture;

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            testTexture = Content.Load<Texture2D>("hello-world");
        }

        private bool backButtonIsPressed() {
            return GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed;
        }
        private bool escapeKeyIsPressed() {
            return Keyboard.GetState().IsKeyDown(Keys.Escape);
        }

        protected override void Update(GameTime gameTime) {
            if(backButtonIsPressed() || escapeKeyIsPressed()) Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _spriteBatch.Draw(testTexture,Vector2.Zero,Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
