using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI.Pages {
    public sealed class DefaultPage:CarouselPage {

        public override void Close() {
            UI.OnButtonActivated -= UI_OnButtonActivated;
            Scene.OnRender.Remove(renderTextHandle);
        }

        private int renderTextHandle;

        public override BookElement Open() {

            UI.OnButtonActivated += UI_OnButtonActivated;

            UI.LeftButton.Flags = ElementFlags.UpdateAndInteract;
            UI.RightButton.Flags = ElementFlags.UpdateAndInteract;
            UI.BackButton.Flags = ElementFlags.UpdateAndInteract;
            UI.SettingsButton.Flags = ElementFlags.UpdateAndInteract;
            UI.PlayButton.Flags = ElementFlags.UpdateAndInteract;

            UI.LeftButton.Scale = 1;
            UI.RightButton.Scale = 1;
            UI.BackButton.Scale = 1;
            UI.SettingsButton.Scale = 1;
            UI.PlayButton.Scale = 1;

            renderTextHandle = Scene.OnRender.Add(Render,EventPriority.Last);

            return UI.PlayButton;
        }

        private void UI_OnButtonActivated(ButtonAction buttonAction) {
            switch(buttonAction) {
                case ButtonAction.Left:
                    Scene.RotateCarousel(MoveDirection.Left);
                    break;
                case ButtonAction.Right:
                    Scene.RotateCarousel(MoveDirection.Right);
                    break;
                case ButtonAction.Play:
                    Scene.StartBattle();
                    break;
                case ButtonAction.Settings:
                    //TODO
                    return;
                case ButtonAction.Menu:
                    Scene.BackToMenu();
                    break;
            }
        }

        private Vector2 elfNameCenter;
        private float elfNameScale;

        private void Render() {
            var font = Fonts.RetroFontOutlined;
            font.Begin(Scene.SpriteBatch);
            font.DrawCentered(Scene.CenterItemName,elfNameCenter,elfNameScale,Color.White);
            font.End();
        }

        public override void Update(FloatRectangle viewport) {
            float pixelSize = Scene.PixelScale;
            float topY = 1/8f, bottomY = 7/8f;

            float centerOffset = 1 / 4f / viewport.AspectRatio;

            float leftX = 0.5f - centerOffset;
            float rightX = 0.5f + centerOffset;

            float buttonEdgeMargin = pixelSize * 2;

            elfNameScale = pixelSize;
            elfNameCenter = new Vector2(0.5f,topY) * viewport.Size;

            foreach(Button button in UI.Buttons) {
                button.SizeByPixels(pixelSize);
            }

            UI.LeftButton.Position = new(leftX,bottomY);
            UI.RightButton.Position = new(rightX,bottomY);

            UI.PlayButton.Position = new(0.5f,bottomY);

            var backButton = UI.BackButton;
                backButton.PositionModeX = CoordinateMode.Absolute;
                backButton.PositionModeY = CoordinateMode.Relative;
                backButton.Offset = new(0,-0.5f);
                backButton.Position = new(buttonEdgeMargin,0.5f);

            var settingsButton = UI.SettingsButton;
                settingsButton.PositionModeX = CoordinateMode.Absolute;
                settingsButton.PositionModeY = CoordinateMode.Relative;
                settingsButton.Offset = new(-1,-0.5f);
                settingsButton.Position = new(viewport.Width-buttonEdgeMargin,0.5f);
        }
    }
}
