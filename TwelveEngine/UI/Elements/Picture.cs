using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    internal class Picture:RenderElement {

        private readonly string imageName;

        private PictureMode mode = PictureMode.Stretch;
        public PictureMode Mode {
            get => mode;
            set {
                if(mode == value) {
                    return;
                }
                mode = value;
                UpdateLayout();
            }
        }

        public Picture(string imageName) {
            this.imageName = imageName;
            OnLoad += Picture_OnLoad;
        }

        private Texture2D picture;

        private void Picture_OnLoad() {
            picture = Game.Content.Load<Texture2D>(imageName);
        }

        public override void Render(GameTime gameTime) {
            switch(mode) {
                default:
                case PictureMode.Contain:
                case PictureMode.Stretch:
                    renderDefault();
                    break;
                case PictureMode.Cover:
                    renderCovered();
                    break;
            }
        }

        private void renderDefault() {
            Game.SpriteBatch.Draw(picture,ScreenArea,GetRenderColor());
        }

        private void renderCovered() {
            Game.SpriteBatch.Draw(picture,ScreenArea,sourceArea,GetRenderColor());
        }

        private Rectangle sourceArea;

        private void updateCoveredArea(Rectangle area) {
            var source = new Rectangle();

            source.Width = picture.Width;
            source.Height = picture.Height;

            if(area.Width > area.Height) {
                source.Height = (int)Math.Floor((float)area.Height / area.Width * source.Width);
            } else {
                source.Width = (int)Math.Floor((float)area.Width / area.Height * source.Height);
            }

            source.X = (int)Math.Floor(picture.Width / 2f - source.Width / 2f);
            source.Y = (int)Math.Floor(picture.Height / 2f - source.Height / 2f);

            sourceArea = source;
        }

        protected override Rectangle GetScreenArea() {
            var area = base.GetScreenArea();

            switch(mode) {
                case PictureMode.Contain:
                    break;
                case PictureMode.Cover:
                    updateCoveredArea(area);
                    goto default;
                default:
                    return area;
            }

            float scale = Math.Min(area.Width / (float)picture.Width,area.Height / (float)picture.Height);

            float newWidth = picture.Width * scale, newHeight = picture.Height * scale;

            area.X += (int)Math.Floor(area.Width / 2f - newWidth / 2f);
            area.Y += (int)Math.Floor(area.Height / 2f - newHeight / 2f);

            area.Width = (int)Math.Ceiling(newWidth);
            area.Height = (int)Math.Ceiling(newHeight);

            return area;
        }
    }
}
