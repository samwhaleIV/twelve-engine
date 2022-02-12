using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell.States;
using TwelveEngine.Shell.UI;

namespace TwelveEngine.TileGen {
    public sealed class TileGenViewer:InputGameState {

        private const int PATTERN_SIZE = 16;

        private PatternSet patternSet;

        private Texture2D texture = null;

        public int RenderScale { get; set; } = 3;

        public TileGenViewer() {
            OnLoad += TileGenViewer_OnLoad;
            OnRender += render;

            Input.OnAcceptDown += InputHandler_OnAcceptDown;
            Input.OnCancelDown += InputHandler_OnCancelDown;
        }

        private void generatePatterns() {
            texture?.Dispose();
            texture = patternSet.GenerateTexture(Game.GraphicsDevice,16); 
        }

        private void TileGenViewer_OnLoad() {
            patternSet = Pattern.GetSet(Constants.PatternsImage,PATTERN_SIZE);
            patternSet.Settings.Seed = null;

            generatePatterns();

            InputGuide.SetDescriptions(
                (Impulse.Accept, "Generate Tiles"),
                (Impulse.Cancel, "Delete Texture")
            );
        }

        private void InputHandler_OnCancelDown() {
            if(texture == null) {
                return;
            }
            texture.Dispose();
            texture = null;
        }

        private void InputHandler_OnAcceptDown() => generatePatterns();

        private void render(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);
            if(texture == null) {
                Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
                InputGuide.Render();
                Game.SpriteBatch.End();
                return;
            }

            Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            var size = texture.Bounds.Size.ToVector2() * RenderScale;
            var location = Game.Viewport.Bounds.Center.ToVector2() - size * 0.5f;

            var destination = new Rectangle(location.ToPoint(),size.ToPoint());

            Game.SpriteBatch.Draw(texture,destination,Color.White);

            InputGuide.Render();
            Game.SpriteBatch.End();
        }
    }
}
