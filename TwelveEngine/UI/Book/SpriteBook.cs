﻿using TwelveEngine.Shell;

namespace TwelveEngine.UI.Book {
    public abstract class SpriteBook:Book<SpriteElement> {
        public void Render(SpriteBatch spriteBatch,float maxY) {
            spriteBatch.Begin(SpriteSortMode.FrontToBack,null,SamplerState.PointClamp);
            foreach(var element in Elements) {
                if(element.TextureSource.Size == Point.Zero || element.ComputedArea.Size == Vector2.Zero) {
                    continue;
                }
                element.Render(spriteBatch);
            }
            spriteBatch.End();
        }

        private readonly InputGameState owner;

        /// <summary>
        /// SpriteBook base constructor.
        /// </summary>
        /// <param name="owner">Used to capture a reference to <see cref="GameState.IsTransitioning"/></param>
        public SpriteBook(InputGameState owner) => this.owner = owner;

        protected override bool GetContextTransitioning() {
            return base.GetContextTransitioning() || (owner?.IsTransitioning ?? false);
        }

        protected override bool GetLeftMouseButtonIsCaptured() {
            return owner?.Mouse.CapturingLeft ?? false;
        }

        protected override bool GetRightMouseButtonIsCaptured() {
            return owner?.Mouse.CapturingRight ?? false;
        }
    }
}
