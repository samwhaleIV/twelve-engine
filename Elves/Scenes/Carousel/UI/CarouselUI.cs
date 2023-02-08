using Elves.Scenes.Carousel.UI.Pages;
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
        private static readonly Rectangle BackButtonSource = new(5,29,46,20);

        private static readonly Rectangle SettingsButtonSource = new(4,5,58,20);

        private readonly CarouselScene3D scene;

        private readonly Texture2D ButtonTextureFile = Program.Textures.Carousel;

        public CarouselUI(CarouselScene3D scene):base(scene) {
            this.scene = scene;
            Initialize();
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

        public CarouselPage DefaultPage { get; private set; }
        public CarouselPage SettingsPage { get; private set; }

        private void Initialize() {

            BackButton = CreateButton(BackButtonSource,ButtonAction.Menu);
            SettingsButton = CreateButton(SettingsButtonSource,ButtonAction.Settings);

            LeftButton = CreateButton(LeftButtonSource,ButtonAction.Left);
            RightButton = CreateButton(RightButtonSource,ButtonAction.Right);
            PlayButton = CreateButton(PlayButtonSource,ButtonAction.Play);

            DefaultPage = new DefaultPage() { Scene = scene, UI = this };
            SettingsPage = new SettingsPage() { Scene = scene, UI = this };

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

            SelectionArrow = AddElement<SelectionArrow>();

            Buttons = ImmutableList.Create(BackButton,SettingsButton,LeftButton,RightButton,PlayButton);
        }

        public event Action<ButtonAction> OnButtonActivated;

        private void ButtonActivated(ButtonAction action) => OnButtonActivated?.Invoke(action);

        protected override TimeSpan GetCurrentTime() => scene.Now;
        protected override FloatRectangle GetViewport() => new(scene.Viewport);
    }
}
