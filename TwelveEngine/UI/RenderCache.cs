using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI {
    internal readonly struct RenderCache {

        public const SpriteSortMode SortMode = SpriteSortMode.BackToFront;

        public readonly RenderElement[] ElementsCache, InteractCache, ScrollCache;

        public readonly RenderFrame[] Frames;

        public RenderCache(
            RenderElement[] elements,
            RenderElement[] interact,
            RenderElement[] scroll,
            RenderFrame[] frames
        ) {
            ElementsCache = elements;
            InteractCache = interact;
            ScrollCache = scroll;
            Frames = frames;
        }

        public void Update(GameTime gameTime) {
            foreach(var element in ElementsCache) {
                element.Update(gameTime);
            }
        }

        public void Render(SpriteBatch spriteBatch,GameTime gameTime) {
            spriteBatch.Begin(SortMode,null,SamplerState.PointClamp);
            foreach(var element in ElementsCache) {
                element.Render(gameTime);
            }
            spriteBatch.End();
        }

        public void PreRender(GameTime gameTime) {
            foreach(var frame in Frames) {
                frame.PreRender(gameTime);
            }
        }

        public void Load(GameManager game) {
            foreach(var element in ElementsCache) {
                element.Load(game);
            }
        }

        public void Unload() {
            foreach(var element in ElementsCache) {
                element.Unload();
            }
        }
    }
}
