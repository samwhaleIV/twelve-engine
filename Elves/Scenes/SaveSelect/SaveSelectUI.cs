using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {

    public sealed class SaveSelectUI:SpriteBook {

        private readonly SaveSelectScene scene;

        public SaveSelectUI(SaveSelectScene scene):base(scene) {
            this.scene = scene;

            BackButton = AddButton(0,123,ButtonImpulse.Back);
            PlayButton = AddButton(17,123,ButtonImpulse.Play);
            AcceptButton = AddButton(0,140,ButtonImpulse.Accept);
            DeleteButton = AddButton(17,140,ButtonImpulse.Delete);

            BackButton.OnActivated += ButtonPressed;
            PlayButton.OnActivated += ButtonPressed;
            AcceptButton.OnActivated += ButtonPressed;
            DeleteButton.OnActivated += ButtonPressed;

            Tag1 = AddTag(scene,0);
            Tag2 = AddTag(scene,1);
            Tag3 = AddTag(scene,2);

            Finger = AddElement(new SpriteElement() {
                TextureSource = new(0,0,174,40),
                Offset = (new(-1,-0.1f)),
                PositionMode = CoordinateMode.Relative,
                Position = new(0,-1),
                Depth = Depth.Finger,
                SmoothStepAnimation = true,
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

            SetFirstPage(TagSelectPage);
            scene.OnInputActivated += FocusDefault;
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
            var tag = new Tag(SaveSelectScene.HasSaveFile(ID)) { ID = ID, DrawingFrame = scene.DrawingFrames[ID] };
            AddElement(tag);
            Tags.Add(tag);
            return tag;
        }

        private Button AddButton(int x,int y,ButtonImpulse impulse) {
            var button = new Button(x,y) { Position = new(0.5f,5/7f), Impulse = impulse };
            AddElement(button);
            Buttons.Add(button);
            return button;
        }

        public override SpriteElement AddElement(SpriteElement element) {
            base.AddElement(element);
            element.Texture = Program.Textures.SaveSelect;
            return element;
        }

        public event Action<ButtonImpulse> OnButtonPresed;

        private void ButtonPressed(ButtonImpulse impulse) => OnButtonPresed?.Invoke(impulse);

        protected override TimeSpan GetCurrentTime() {
            return scene.Now;
        }

        protected override FloatRectangle GetViewport() {
            return new(scene.Viewport);
        }
    }
}
