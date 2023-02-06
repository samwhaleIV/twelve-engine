using Microsoft.Xna.Framework;
using System;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI.Pages {
    public sealed class DefaultPage:CarouselPage {

        private const string MENU_TEXT = "Save Select";
        private const string SETTINGS_TEXT = "Settings";
        private const string PLAY_TEXT = "Fight";
        private static readonly Color TextColor = new(38,38,38,255);
        private const float TextScale = 0.25f;

        public override void Close() {
            UI.OnButtonActivated -= UI_OnButtonActivated;
            Scene.OnRender.Remove(renderTextHandle);
        }

        private int renderTextHandle;

        public override BookElement Open() {

            UI.OnButtonActivated += UI_OnButtonActivated;

            UI.LeftButton.Flags = ElementFlags.UpdateAndInteract;
            UI.RightButton.Flags = ElementFlags.UpdateAndInteract;
            UI.MenuButton.Flags = ElementFlags.UpdateAndInteract;
            UI.SettingsButton.Flags = ElementFlags.UpdateAndInteract;
            UI.PlayButton.Flags = ElementFlags.UpdateAndInteract;

            UI.LeftButton.Scale = 1;
            UI.RightButton.Scale = 1;
            UI.MenuButton.Scale = 1;
            UI.SettingsButton.Scale = 1;
            UI.PlayButton.Scale = 1;

            renderTextHandle = Scene.OnRender.Add(RenderText,EventPriority.Last);

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

        private static void DrawButtonText(Button button,string text,float scale) {
            Fonts.RetroFont.DrawCentered(text,button.GetComputedCenter(),scale*button.ComputedScale,TextColor);
        }

        private void RenderText() {
            Fonts.RetroFont.Begin(Scene.SpriteBatch);
            float textScale = MathF.Round(Scene.GetUIScale() * TextScale);
            DrawButtonText(UI.MenuButton,MENU_TEXT,textScale);
            DrawButtonText(UI.SettingsButton,SETTINGS_TEXT,textScale);
            DrawButtonText(UI.PlayButton,PLAY_TEXT,textScale);
            Fonts.RetroFont.End();
        }

        public override void Update(FloatRectangle viewport) {

            float directionButtonHeight = viewport.Height * 0.15f;
            Vector2 directionButtonSize = new(UI.LeftButton.GetWidth(directionButtonHeight),directionButtonHeight);

            float cornerButtonHeight = UI.MenuButton.SourceHeight / UI.LeftButton.SourceHeight * directionButtonHeight;
            Vector2 cornerButtonSize = new(UI.MenuButton.GetWidth(cornerButtonHeight),cornerButtonHeight);

            float playButtonHeight = UI.PlayButton.SourceHeight / UI.LeftButton.SourceHeight * directionButtonHeight;
            UI.PlayButton.Size = new(UI.PlayButton.GetWidth(playButtonHeight),playButtonHeight);
            
            UI.MenuButton.Size = cornerButtonSize;
            UI.SettingsButton.Size = cornerButtonSize;

            float marginCenterDistance = Scene.GetMarginCenter();

            float navigationYCenter = 1 - (marginCenterDistance / viewport.Height);

            Vector2 cornerButtonMargin = new(64 / viewport.AspectRatio,0);
            UI.MenuButton.Position = (new Vector2(0,0.5f) * viewport.Size) + cornerButtonMargin;
            UI.SettingsButton.Position = (new Vector2(1,0.5f) * viewport.Size) - cornerButtonMargin;

            UI.LeftButton.Position = new(0.5f - 0.32f / viewport.AspectRatio,navigationYCenter);
            UI.LeftButton.Size = directionButtonSize;

            UI.RightButton.Position = new(0.5f + 0.32f / viewport.AspectRatio,navigationYCenter);
            UI.RightButton.Size = directionButtonSize;

            UI.PlayButton.Position = new(0.5f,navigationYCenter);

            //Color buttonColor = Scene.GetTintColor();
            //buttonColor = Color.Lerp(buttonColor,Color.Black,0.25f);
            //UI.LeftButton.Color = buttonColor;
            //UI.RightButton.Color = buttonColor;
            //UI.PlayButton.Color = buttonColor;
            //UI.MenuButton.Color = buttonColor;
            //UI.SettingsButton.Color = buttonColor;
        }
    }
}
