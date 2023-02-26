using Microsoft.Xna.Framework;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI {
    public sealed class DefaultPage:BookPage<SpriteElement> {

        public CarouselUI UI { get; set; }
        public CarouselScene3D Scene { get; set; }

        public override void Close() {
            UI.OnButtonActivated -= UI_OnButtonActivated;
            Scene.OnRender.Remove(_renderTextHandle);
            foreach(var button in UI.Buttons) {
                button.KeyFrame(Now,TransitionDuration);
                button.Scale = 0;
                button.Flags = ElementFlags.None;
            }
        }

        private int _renderTextHandle = -1;
        private Vector2 _elfNameCenter = Vector2.Zero;
        private float _elfNameScale = 0;

        public bool OpeningFromSettingsPage { get; set; } = false;

        public override BookElement Open() {
            UI.OnButtonActivated += UI_OnButtonActivated;

            foreach(var button in UI.Buttons) {
                button.KeyFrame(Now,TransitionDuration);

                button.Scale = 1;
                button.Flags = ElementFlags.UpdateAndInteract;

                button.PositionModeX = CoordinateMode.Absolute;
                button.PositionModeY = CoordinateMode.Relative;

                button.SizeMode = CoordinateMode.Absolute;
            }

            UI.BackButton.Offset = new(0,-0.5f);
            UI.SettingsButton.Offset = new(-1,-0.5f);
            UI.RightButton.Offset = new(0,-0.5f);
            UI.LeftButton.Offset = new(-1,-0.5f);
            UI.PlayButton.PositionModeX = CoordinateMode.Relative;

            UI.SetDefaultFocusTree();

            _renderTextHandle = Scene.OnRender.Add(RenderElfNameText,EventPriority.SecondToLast);

            BookElement defaultFocusElement;
            if(OpeningFromSettingsPage) {
                defaultFocusElement = UI.SettingsButton;
            } else {
                UI.SelectionArrow.PositionMode = CoordinateMode.Relative;
                UI.SelectionArrow.Position = new(0.5f,0.5f);
                defaultFocusElement = UI.PlayButton;
            }
            return defaultFocusElement;
        }

        private void UI_OnButtonActivated(ButtonAction buttonAction) {
            switch(buttonAction) {
                case ButtonAction.Left: Scene.RotateCarousel(MoveDirection.Left); return;
                case ButtonAction.Right: Scene.RotateCarousel(MoveDirection.Right); return;
                case ButtonAction.Play: Scene.StartBattle(); return;
                case ButtonAction.Settings: UI.SetPage(UI.SettingsPage); return;
                case ButtonAction.Back: Scene.BackToMenu(); return;
            }
        }

        public override void Update() {
            float pixelSize = Scene.UIScale;
            float topY = 1/8f, bottomY = 7/8f;

            float buttonEdgeMargin = pixelSize * 2;

            _elfNameScale = pixelSize * Constants.UI.CarouselElfNameScale;
            _elfNameCenter = new Vector2(0.5f,topY) * Viewport.Size;

            foreach(Button button in UI.Buttons) {
                button.SizeByPixels(pixelSize);
            }

            float directionButtonOffset = pixelSize * Constants.UI.CarouselDirectionButtonOffset;

            UI.LeftButton.Position = new(Viewport.Center.X - directionButtonOffset,bottomY);
            UI.RightButton.Position = new(Viewport.Center.X + directionButtonOffset,bottomY);
            UI.BackButton.Position = new(buttonEdgeMargin,0.5f);
            UI.SettingsButton.Position = new(Viewport.Width-buttonEdgeMargin,0.5f);
            UI.PlayButton.Position = new(0.5f,bottomY);

            UI.UpdateSelectionArrow();
        }

        private void RenderElfNameText() {
            var font = Fonts.RetroOutlined;
            font.Begin(Scene.SpriteBatch);
            font.DrawCentered(Scene.CenterItemName,_elfNameCenter,_elfNameScale,Color.White);
            font.End();
        }

    }
}
