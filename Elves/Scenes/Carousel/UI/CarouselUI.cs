using Elves.Scenes.Carousel.UI.Pages;
using Microsoft.Xna.Framework;
using System;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI {
    public sealed class CarouselUI:SpriteBook {

        private readonly CarouselScene3D scene;

        public CarouselUI(CarouselScene3D scene):base(scene) {
            this.scene = scene;
            Initialize();
        }

        public Button MenuButton { get; private set; }
        public Button SettingsButton { get; private set; }
        public Button LeftButton { get; private set; }
        public Button RightButton { get; private set; }
        public Button PlayButton { get; private set; }

        private Button AddButton(Rectangle textureSource,ButtonAction action) {
            var button = new Button(textureSource) {
                Position = new Vector2(0,0),
                Action = action,
                Texture = Program.Textures.Panel
            };
            AddElement(button);
            return button;
        }

        private static readonly Rectangle LeftButtonSource = new(16,80,16,10);
        private static readonly Rectangle RightButtonSource = new(0,80,16,10);

        private static readonly Rectangle PlayButtonSoruce = new(64,80,22,11);
        private static readonly Rectangle MenuButtonSource = new(32,80,32,9);
        private static readonly Rectangle SettingsButtonSource = new(32,80,32,9);

        public CarouselPage DefaultPage { get; private set; }
        public CarouselPage SettingsPage { get; private set; }

        private void Initialize() {
            MenuButton = AddButton(MenuButtonSource,ButtonAction.Menu);
            SettingsButton = AddButton(SettingsButtonSource,ButtonAction.Settings);

            MenuButton.PositionMode = CoordinateMode.Absolute;
            SettingsButton.PositionMode = CoordinateMode.Absolute;
            MenuButton.Offset = new Vector2(0,-0.5f);
            SettingsButton.Offset = new Vector2(-1,-0.5f);

            LeftButton = AddButton(LeftButtonSource,ButtonAction.Left);
            RightButton = AddButton(RightButtonSource,ButtonAction.Right);
            PlayButton = AddButton(PlayButtonSoruce,ButtonAction.Play);
            //todo make elements...
            DefaultPage = new DefaultPage() { Scene = scene, UI = this };
            SettingsPage = new SettingsPage() { Scene = scene, UI = this };

            MenuButton.OnActivated += ButtonActivated;
            SettingsButton.OnActivated += ButtonActivated;
            LeftButton.OnActivated += ButtonActivated;
            RightButton.OnActivated += ButtonActivated;
            PlayButton.OnActivated += ButtonActivated;

            MenuButton.FocusSet = new() {
                Right = SettingsButton,
                Down = LeftButton
            };
            SettingsButton.FocusSet = new() {
                Left = MenuButton,
                Down = RightButton
            };
            LeftButton.FocusSet = new() {
                Left = MenuButton,
                Up = MenuButton,
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

            SetPage(DefaultPage);
        }

        public event Action<ButtonAction> OnButtonActivated;

        private void ButtonActivated(ButtonAction action) => OnButtonActivated?.Invoke(action);
    }
}
