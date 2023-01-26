using TwelveEngine.UI;
using System;
using System.Collections.Generic;
using static Elves.Constants;

namespace Elves.Scenes.SaveSelect {

    public sealed class SaveSelectUI:SpriteBook {

        public SaveSelectUI(SaveSelectScene scene) {

            BackButton = AddButton(0,123);
            PlayButton = AddButton(17,123);
            AcceptButton = AddButton(0,140);
            DeleteButton = AddButton(17,140);

            BackButton.OnActivated += BackButton_OnActivated;
            PlayButton.OnActivated += PlayButton_OnActivated;
            AcceptButton.OnActivated += AcceptButton_OnActivated;
            DeleteButton.OnActivated += DeleteButton_OnActivated;

            Tag1 = AddTag(scene,0);
            Tag2 = AddTag(scene,1);
            Tag3 = AddTag(scene,2);

            Finger = AddElement(new SpriteElement() {
                TextureSource = new(0,0,174,40),
                Offset = (new(-1,-0.1f)),
                PositionMode = CoordinateMode.Relative,
                Depth = Depth.Finger,
                SmoothStep = true,
                DefaultAnimationDuration = TimeSpan.FromMilliseconds(300)
            });

            SignHereLabel = AddElement(new SpriteElement() {
                TextureSource = new(141,50,83,20),
                Offset = (new(-0.5f,-0.5f)),
                PositionMode = CoordinateMode.Relative,
                Position = new(0.5f,1/5f),
                Depth = Depth.DrawLabel
            });

            TagSelectPage = new TagSelectPage() { Scene = scene, UI = this };
            TagContextPage = new TagContextPage() { Scene = scene, UI = this };
            TagDrawPage = new TagDrawPage() { Scene = scene, UI = this };

            SetPage(TagSelectPage);
            foreach(var element in Elements) {
                element.SkipAnimation();
            }
        }

        public readonly SaveSelectPage TagSelectPage, TagContextPage, TagDrawPage;

        public Tag SelectedTag { get; set; } = null;

        public Button BackButton, PlayButton, AcceptButton, DeleteButton;
        public SpriteElement Finger, SignHereLabel;
        public Tag Tag1, Tag2, Tag3;

        public readonly List<Button> Buttons = new();
        public readonly List<Tag> Tags = new();

        /// <summary>
        /// Adds a <c>Tag</c> element to the elements pool.
        /// </summary>
        /// <param name="scene">Required reference for getting a reference to the drawing frame.</param>
        /// <param name="ID">Save tag ID. Index starts at 0.</param>
        /// <returns></returns>
        private Tag AddTag(SaveSelectScene scene,int ID) {
            var tag = new Tag() { ID = ID, DrawingFrame = scene.DrawingFrames[ID] };
            AddElement(tag);
            Tags.Add(tag);
            return tag;
        }

        private Button AddButton(int x,int y) {
            var button = new Button(x,y) { Position = new(0.5f,5/7f) };
            AddElement(button);
            Buttons.Add(button);
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
