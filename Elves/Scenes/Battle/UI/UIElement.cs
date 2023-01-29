using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using TwelveEngine.UI.Interaction;

namespace Elves.Scenes.Battle.UI {
    public class UIElement:InteractionElement<UIElement> {

        protected Texture2D Texture { get; set; } = Program.Textures.Nothing;
        public VectorRectangle ScreenArea { get; set; }

        public virtual void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(Texture == null) {
                return;
            }
            spriteBatch.Draw(Texture,ScreenArea.ToRectangle(),color ?? Color.White);
        }

        public override VectorRectangle GetScreenArea() => ScreenArea;
    }
}
