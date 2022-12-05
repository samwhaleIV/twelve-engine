using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell;

namespace Elves.UI {
    public sealed class NineGrid:UIElement {

        public NineGrid() => Texture = UITextures.Panel;

        public NineGrid(Texture2D texture) => Texture = texture;

        public Rectangle Source { get; set; } = new Rectangle(0,85,16,16);
        public int Corner { get; set; } = 4;

        public int Scale { get; set; } = 1;

        public override void Draw(SpriteBatch spriteBatch) {
            if(Texture == null) {
                return;
            }
            int cornerScaled = Corner * Scale;
            int cornerScaledDouble = cornerScaled * 2;
            int cornerDouble = Corner * 2;
            spriteBatch.Draw(Texture, /* Top left */
                new Rectangle(Area.X,Area.Y,cornerScaled,cornerScaled),
                new Rectangle(Source.X,Source.Y,Corner,Corner),
            Color.White);
            spriteBatch.Draw(Texture, /* Top right */
                new Rectangle(Area.X+Area.Width-cornerScaled,Area.Y,cornerScaled,cornerScaled),
                new Rectangle(Source.X+Source.Width-Corner,Source.Y,Corner,Corner),
            Color.White);
            spriteBatch.Draw(Texture, /* Bottom left */
                new Rectangle(Area.X,Area.Y+Area.Height-cornerScaled,cornerScaled,cornerScaled),
                new Rectangle(Source.X,Source.Y+Source.Height-Corner,Corner,Corner),
            Color.White);
            spriteBatch.Draw(Texture, /* Bottom right */
                new Rectangle(Area.X+Area.Width-cornerScaled,Area.Y+Area.Height-cornerScaled,cornerScaled,cornerScaled),
                new Rectangle(Source.X+Source.Width-Corner,Source.Y+Source.Height-Corner,Corner,Corner),
            Color.White);
            spriteBatch.Draw(Texture, /* Left */
                new Rectangle(Area.X,Area.Y+cornerScaled,cornerScaled,Area.Height-cornerScaledDouble),
                new Rectangle(Source.X,Source.Y+Corner,Corner,Source.Height-cornerDouble),
            Color.White);
            spriteBatch.Draw(Texture, /* Right */
                new Rectangle(Area.X+Area.Width-cornerScaled,Area.Y+cornerScaled,cornerScaled,Area.Height-cornerScaledDouble),
                new Rectangle(Source.X+Source.Width-Corner,Source.Y+Corner,Corner,Source.Height-cornerDouble),
            Color.White);
            spriteBatch.Draw(Texture, /* Top */
                new Rectangle(Area.X+cornerScaled,Area.Y,Area.Width-cornerScaledDouble,cornerScaled),
                new Rectangle(Source.X+Corner,Source.Y,Source.Width-cornerDouble,Corner),
            Color.White);
            spriteBatch.Draw(Texture, /* Bottom */
                new Rectangle(Area.X+cornerScaled,Area.Y+Area.Height-cornerScaled,Area.Width-cornerScaledDouble,cornerScaled),
                new Rectangle(Source.X+Corner,Source.Y+Source.Height-Corner,Source.Width-cornerDouble,Corner),
            Color.White);
            spriteBatch.Draw(Texture, /* Center */
                new Rectangle(Area.X+cornerScaled,Area.Y+cornerScaled,Area.Width-cornerScaledDouble,Area.Height-cornerScaledDouble),
                new Rectangle(Source.X+Corner,Source.Y+Corner,Source.Width-cornerDouble,Source.Height-cornerDouble),
            Color.White);
        }
    }
}
