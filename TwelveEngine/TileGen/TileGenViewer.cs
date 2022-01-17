using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.GameUI;

namespace TwelveEngine.TileGen {
    public sealed class TileGenViewer:GameState {

        private const int PATTERN_SIZE = 16;

        private PatternSet patternSet;

        private Texture2D texture = null;

        public int RenderScale { get; set; } = 3;

        private ImpulseGuide impulseGuide;

        public TileGenViewer() {
            OnLoad += TileGenViewer_OnLoad;
            OnUnload += TileGenViewer_OnUnload;
            OnRender += render;
        }

        private void generatePatterns() {
            texture?.Dispose();
            texture = patternSet.GenerateTexture(Game.GraphicsDevice,16); 
        }

        private void TileGenViewer_OnLoad() {
            patternSet = Pattern.GetSet(Constants.PatternsImage,PATTERN_SIZE);
            patternSet.Settings.Seed = null;

            Input.OnAcceptDown += ImpulseHandler_OnAcceptDown;
            Input.OnCancelDown += ImpulseHandler_OnCancelDown;
            generatePatterns();

            impulseGuide = new ImpulseGuide(Game);

            impulseGuide.SetDescriptions(
                (Impulse.Accept, "Generate Tiles"),
                (Impulse.Cancel, "Delete Texture")
            );
        }

        private void ImpulseHandler_OnCancelDown() {
            if(texture == null) {
                return;
            }
            texture.Dispose();
            texture = null;
        }

        private void TileGenViewer_OnUnload() {
            Input.OnAcceptDown -= ImpulseHandler_OnAcceptDown;
            Input.OnCancelDown -= ImpulseHandler_OnCancelDown;
        }

        private void ImpulseHandler_OnAcceptDown() => generatePatterns();

        private void render(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);
            if(texture == null) {
                Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);
                impulseGuide.Render();
                Game.SpriteBatch.End();
                return;
            }

            Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            var size = texture.Bounds.Size.ToVector2() * RenderScale;
            var location = Game.GraphicsDevice.Viewport.Bounds.Center.ToVector2() - size * 0.5f;

            var destination = new Rectangle(location.ToPoint(),size.ToPoint());

            Game.SpriteBatch.Draw(texture,destination,Color.White);

            impulseGuide.Render();
            Game.SpriteBatch.End();
        }
    }
}
