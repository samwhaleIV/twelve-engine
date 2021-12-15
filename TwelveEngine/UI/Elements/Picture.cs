using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.UI.Elements {
    internal class Picture:RenderElement {

        public Picture(string imageName) => OnLoad += () => picture = GetImage(imageName);

        private Texture2D picture;
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

        public override void Render(GameTime _) {
            switch(mode) {
                default:
                case PictureMode.Contain:
                case PictureMode.Stretch:
                    Draw(picture);
                    break;
                case PictureMode.Cover:
                    Draw(picture,sourceArea);
                    break;
            }
        }

        private Rectangle sourceArea;

        private void updateCoveredArea(Rectangle area) {
            var source = new Rectangle();

            source.Width = picture.Width;
            source.Height = picture.Height;

            if(area.Width > area.Height) {
                source.Height = (int)((float)area.Height / area.Width * source.Width);
            } else {
                source.Width = (int)((float)area.Width / area.Height * source.Height);
            }

            source.X = (int)(picture.Width / 2f - source.Width / 2f);
            source.Y = (int)(picture.Height / 2f - source.Height / 2f);

            sourceArea = source;
        }

        private Rectangle calculateContainArea(Rectangle area) {
            float scale = Math.Min(area.Width / (float)picture.Width,area.Height / (float)picture.Height);

            float newWidth = picture.Width * scale, newHeight = picture.Height * scale;

            area.X += (int)(area.Width / 2f - newWidth / 2f);
            area.Y += (int)(area.Height / 2f - newHeight / 2f);

            area.Width = (int)(newWidth);
            area.Height = (int)(newHeight);

            return area;
        }

        protected override Rectangle GetScreenArea() {
            var area = base.GetScreenArea();
            switch(mode) {
                case PictureMode.Contain: {
                    area = calculateContainArea(area);
                    break;
                }
                case PictureMode.Cover: {
                    updateCoveredArea(area);
                    break;
                }
            }
            return area;
        }
    }
}
