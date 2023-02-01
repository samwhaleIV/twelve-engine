using TwelveEngine.Shell;

namespace TwelveEngine.UI.Book {
    public abstract class SpriteBook:Book<SpriteElement> {
        public void Render(SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.FrontToBack,null,SamplerState.PointClamp);
            foreach(var element in Elements) {
                if(!element.TextureSource.HasValue || element.ComputedArea.Destination.Size == Vector2.Zero) {
                    continue;
                }
                element.Render(spriteBatch);
            }
            spriteBatch.End();
        }

        private readonly GameState owner;

        /// <summary>
        /// SpriteBook base constructor.
        /// </summary>
        /// <param name="owner">Used to capture a reference to <see cref="GameState.IsTransitioning"/></param>
        public SpriteBook(GameState owner) => this.owner = owner;

        protected override bool GetContextTransitioning() {
            return base.GetContextTransitioning() || (owner?.IsTransitioning ?? false);
        }
    }
}
