using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI {
    internal readonly struct RenderCache {

        public const SpriteSortMode SortMode = SpriteSortMode.BackToFront;

        public readonly RenderElement[] Elements, Interact, Scroll;

        public readonly RenderFrame[] Frames;

        public RenderCache(
            RenderElement[] elements,
            RenderElement[] interact,
            RenderElement[] scroll,
            RenderFrame[] frames
        ) {
            Elements = elements;
            Interact = interact;
            Scroll = scroll;
            Frames = frames;
        }

        public void UpdateElements(GameTime gameTime) {
            foreach(var element in Elements) {
                element.Update(gameTime);
            }
        }

        public void RenderElements(SpriteBatch spriteBatch,GameTime gameTime) {
            spriteBatch.Begin(SortMode,null,SamplerState.PointClamp);
            foreach(var element in Elements) {
                element.Render(gameTime);
            }
            spriteBatch.End();
        }

        public void PreRenderElements(GameTime gameTime) {
            foreach(var frame in Frames) {
                frame.PreRender(gameTime);
            }
        }

        public void Load(GameManager game) {
            foreach(var element in Elements) {
                element.Load(game);
            }
        }

        public void Unload() {
            foreach(var element in Elements) {
                element.Unload();
            }
        }
    }
}
