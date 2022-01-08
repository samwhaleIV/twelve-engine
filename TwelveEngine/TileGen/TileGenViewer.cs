using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.TileGen {
    public sealed class TileGenViewer:GameState {

        private const int PATTERN_SIZE = 16;

        private PatternSet patternSet;

        private Texture2D texture = null;

        public int RenderScale { get; set; } = 4;

        public TileGenViewer() {
            OnLoad += TileGenViewer_OnLoad;
            OnUnload += TileGenViewer_OnUnload;
        }

        private void generatePatterns() {
            texture?.Dispose();
            texture = patternSet.GenerateTexture(Game.GraphicsDevice,16); 
        }

        private void TileGenViewer_OnLoad() {
            patternSet = Pattern.GetSet(Constants.PatternsImage,PATTERN_SIZE);
            patternSet.Settings.Seed = null;

            Game.ImpulseHandler.OnAcceptDown += ImpulseHandler_OnAcceptDown;
            generatePatterns();
        }
        private void TileGenViewer_OnUnload() {
            Game.ImpulseHandler.OnAcceptDown -= ImpulseHandler_OnAcceptDown;
        }

        private void ImpulseHandler_OnAcceptDown() => generatePatterns();

        public override void Update(GameTime gameTime) {}

        public override void Render(GameTime gameTime) {
            Game.GraphicsDevice.Clear(Color.Black);
            if(texture == null) {
                return;
            }

            Game.SpriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp);

            var size = texture.Bounds.Size.ToVector2() * RenderScale;
            var location = Game.GraphicsDevice.Viewport.Bounds.Center.ToVector2() - size * 0.5f;

            var destination = new Rectangle(location.ToPoint(),size.ToPoint());

            Game.SpriteBatch.Draw(texture,destination,Color.White);

            Game.SpriteBatch.End();
        }
    }
}
