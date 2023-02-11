using Elves.Settings;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Immutable;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI {
    public sealed class CarouselUI:SpriteBook {

        private static readonly Rectangle LeftButtonSource = new(65,5,20,20);
        private static readonly Rectangle RightButtonSource = new(88,5,20,20);

        private static readonly Rectangle PlayButtonSource = new(59,28,36,20);
        private static readonly Rectangle BackButtonSource = new(5,29,47,20);

        private static readonly Rectangle SettingsButtonSource = new(4,5,55,20);

        private readonly CarouselScene3D scene;

        private readonly Texture2D ButtonTextureFile = Program.Textures.Carousel;

        public CarouselUI(CarouselScene3D scene):base(scene) {
            this.scene = scene;
            OnPageTransitionStart += CarouselUI_OnPageTransitionStart;
            Initialize();
        }

        private void CarouselUI_OnPageTransitionStart() {
            foreach(var button in Buttons) {
                button.ClearKeyFocus();
                button.CanInteract = false;
                button.CanUpdate = false;
            }
        }

        public Button BackButton { get; private set; }
        public Button SettingsButton { get; private set; }
        public Button LeftButton { get; private set; }
        public Button RightButton { get; private set; }
        public Button PlayButton { get; private set; }

        public SelectionArrow SelectionArrow { get; private set; }

        public ImmutableList<Button> Buttons { get; private set; }

        private Button CreateButton(Rectangle textureSource,ButtonAction action) {
            var button = new Button(textureSource) {
                Position = new Vector2(0,0),
                Action = action,
                Texture = ButtonTextureFile
            };
            button.OnActivated += ButtonActivated;
            AddElement(button);
            return button;
        }

        public DefaultPage DefaultPage { get; private set; }
        public SettingsPage<Button> SettingsPage { get; private set; }

        public void SetDefaultFocusTree() {
            BackButton.FocusSet = new() {
                Right = SettingsButton,
                Down = LeftButton
            };
            SettingsButton.FocusSet = new() {
                Left = BackButton,
                Down = RightButton
            };
            LeftButton.FocusSet = new() {
                Left = BackButton,
                Up = BackButton,
                Right = PlayButton
            };
            RightButton.FocusSet = new() {
                Right = SettingsButton,
                Up = SettingsButton,
                Left = PlayButton
            };
            PlayButton.FocusSet = new() {
                Left = LeftButton,
                Right = RightButton
            };
        }

        private void Initialize() {

            BackButton = CreateButton(BackButtonSource,ButtonAction.Back);
            BackButton.Depth = Constants.Depth.MiddleCloser;
            SettingsButton = CreateButton(SettingsButtonSource,ButtonAction.Settings);

            LeftButton = CreateButton(LeftButtonSource,ButtonAction.Left);
            RightButton = CreateButton(RightButtonSource,ButtonAction.Right);
            PlayButton = CreateButton(PlayButtonSource,ButtonAction.Play);

            DefaultPage = new DefaultPage() { Scene = scene, UI = this };

            SettingsPage = new SettingsPage<Button>(BackButton);
            SettingsPage.OnBack += SettingsPage_OnBack;
            SettingsPage.OnUpdate += UpdateSelectionArrow;

            foreach(var element in SettingsPage.GetElements()) {
                AddElement(element);
            }

            SelectionArrow = AddElement<SelectionArrow>();
            Buttons = ImmutableList.Create(BackButton,SettingsButton,LeftButton,RightButton,PlayButton);
        }

        private void SettingsPage_OnBack() {
            DefaultPage.OpeningFromSettingsPage = true;
            SetPage(DefaultPage);
        }

        public event Action<ButtonAction> OnButtonActivated;

        private void ButtonActivated(ButtonAction action) => OnButtonActivated?.Invoke(action);

        protected override TimeSpan GetCurrentTime() => scene.Now;
        protected override FloatRectangle GetViewport() => new(scene.Viewport);

        public void UpdateSelectionArrow() {
            if(SelectedElement is null) {
                SelectionArrow.Update(SelectedElement,scene.UIScale,Now);
                return;
            }
            var selectedElementCenter = SelectedElement.ComputedArea.Center / GetViewport().Size;

            if(Page == SettingsPage) {
                if(SelectedElement == BackButton) {
                    SelectionArrow.Direction = Direction.Up;
                } else {
                    SelectionArrow.Direction = Direction.Left;
                }
            } else {
                SelectionArrow.Direction = selectedElementCenter switch {
                    { X: < 1 / 3f } => Direction.Left,
                    { X: > 2 / 3f } => Direction.Right,
                    { Y: > 1 / 2f } => Direction.Down,
                    _ => Direction.Up
                };
            }

            SelectionArrow.Update(SelectedElement,scene.UIScale,Now);
        }
    }
}
