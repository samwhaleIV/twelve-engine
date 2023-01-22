using TwelveEngine.UI;
using System;

namespace Elves.Scenes.SaveSelect {

    public sealed class SaveSelectUI:SpriteBook {

        public SaveSelectUI(SaveSelectScene scene) {

            BackButton = AddButton(0,123);
            PlayButton = AddButton(17,123);
            AcceptButton = AddButton(0,140);
            DeleteButton = AddButton(17,140);

            BackButton.OnActivated += BackButton_OnActivated;
            PlayButton.OnActivated +=PlayButton_OnActivated;
            AcceptButton.OnActivated += AcceptButton_OnActivated;
            DeleteButton.OnActivated += DeleteButton_OnActivated;

            Tag1 = AddTag(0);
            Tag2 = AddTag(1);
            Tag3 = AddTag(2);

            Finger = AddElement(new SpriteElement() {
                TextureSource = new(0,0,174,40),
                Offset = (new(-1,-0.1f)),
                PositionModeY = CoordinateMode.Relative,
                PositionModeX = CoordinateMode.Relative,
                Depth = SaveSelectDepth.Finger,
                SmoothStep = true,
                DefaultAnimationDuration = TimeSpan.FromMilliseconds(300)
            });

            TagSelectPage = new TagSelectPage() { Scene = scene, UI = this };
            TestPage = new TagContextPage() { Scene = scene, UI = this };

            SetPage(TagSelectPage,TimeSpan.Zero);
            foreach(var element in Elements) {
                element.SkipAnimation();
            }
        }

        public const int TagWidth = 128, TagHeight = 32;
        public const float TagRotation = -5f;

        public readonly SaveSelectUIPage TagSelectPage, TestPage;

        public Tag SelectedTag { get; set; } = null;

        public Button BackButton, PlayButton, AcceptButton, DeleteButton;
        public SpriteElement Finger;
        public Tag Tag1, Tag2, Tag3;

        private Tag AddTag(int ID) {
            var tag = new Tag() { ID = ID };
            AddElement(tag);
            return tag;
        }

        private Button AddButton(int x,int y) {
            var button = new Button(x,y);
            AddElement(button);
            return button;
        }

        public override SpriteElement AddElement(SpriteElement element) {
            base.AddElement(element);
            element.Texture = Program.Textures.SaveSelect;
            return element;
        }

        public event Action<TimeSpan,ButtonImpulse> OnButtonPresed;

        private void ButtonPressed(TimeSpan now,ButtonImpulse impulse) => OnButtonPresed?.Invoke(now,impulse);

        private void DeleteButton_OnActivated(TimeSpan now) => ButtonPressed(now,ButtonImpulse.Delete);
        private void AcceptButton_OnActivated(TimeSpan now) => ButtonPressed(now,ButtonImpulse.Accept);
        private void PlayButton_OnActivated(TimeSpan now) => ButtonPressed(now,ButtonImpulse.Play);
        private void BackButton_OnActivated(TimeSpan now) => ButtonPressed(now,ButtonImpulse.Back);
    }
}
