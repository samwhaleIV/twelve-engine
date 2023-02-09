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
            Scene.OnRender.Remove(renderTextHandle);
        }

        private int renderTextHandle;

        public override BookElement Open() {

            UI.OnButtonActivated += UI_OnButtonActivated;

            foreach(var button in UI.Buttons) {
                button.Scale = 1;
                button.Flags = ElementFlags.UpdateAndInteract;
            }

            UI.SelectionArrow.CanUpdate = true;
            UI.SelectionArrow.Scale = 1;

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

        private void UpdateSelectionArrow(float pixelSize) {
            var selectedElement = UI.SelectedElement;
            var selectionArrow = UI.SelectionArrow;
            if(selectedElement is null) {
                selectionArrow.Update(selectedElement,pixelSize,Now);
                return;
            }
            Direction direction = Direction.None;
            if(
                selectedElement == UI.LeftButton ||
                selectedElement == UI.RightButton ||
                selectedElement == UI.PlayButton
            ) {
                direction = Direction.Down;
            } else if(
                selectedElement == UI.BackButton
            ) {
                direction = Direction.Left;
            } else if(
                selectedElement == UI.SettingsButton
            ) {
                direction = Direction.Right;
            }
            selectionArrow.Direction = direction;
            selectionArrow.Update(selectedElement,pixelSize,Now);
        }

        public override void Update(FloatRectangle viewport) {
            float pixelSize = Scene.UIScale;
            float topY = 1/8f, bottomY = 7/8f;

            float buttonEdgeMargin = pixelSize * 2;

            elfNameScale = pixelSize * Constants.UI.CarouselElfNameScale;
            elfNameCenter = new Vector2(0.5f,topY) * viewport.Size;

            foreach(Button button in UI.Buttons) {
                button.SizeByPixels(pixelSize);
            }

            float directionButtonOffset = pixelSize * Constants.UI.CarouselDirectionButtonOffset;

            var leftButton = UI.LeftButton;
                leftButton.PositionModeX = CoordinateMode.Absolute;
                leftButton.PositionModeY = CoordinateMode.Relative;
                leftButton.Offset = new(-1,-0.5f);
                leftButton.Position = new(viewport.Center.X - directionButtonOffset,bottomY);

            var rightButton = UI.RightButton;
                rightButton.PositionModeX = CoordinateMode.Absolute;
                rightButton.PositionModeY = CoordinateMode.Relative;
                rightButton.Offset = new(0,-0.5f);
                rightButton.Position = new(viewport.Center.X + directionButtonOffset,bottomY);

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

            UpdateSelectionArrow(pixelSize);
        }
    }
}
