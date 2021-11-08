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
            Window.AllowUserResizing = true;
            Window.AllowAltF4 = true;

            JSEngine.RunTest();
            Window.Title = Runtime.TestString;

            base.Initialize();
        }

        Texture2D testTexture;

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
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

        private Viewport GetViewport() {
            return GraphicsDevice.Viewport;
        }
        private Vector2 GetScreenCenter() {
            var viewport = GetViewport();
            Vector2 screenCenter = new Vector2(viewport.Width / 2,viewport.Height / 2);
            return screenCenter;
        }

        private void DrawCentered(SpriteBatch spriteBatch,Texture2D texture,Vector2 origin) {
            Vector2 destination = new Vector2(
                origin.X - texture.Width / 2,
                origin.Y - texture.Height / 2
            );
            spriteBatch.Draw(texture,destination,Color.White);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.LightGray);

            var screenCenter = GetScreenCenter();

            _spriteBatch.Begin();

            DrawCentered(_spriteBatch,testTexture,screenCenter);
            
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
