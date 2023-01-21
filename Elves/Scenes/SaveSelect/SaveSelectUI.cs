using Elves.UI.SpriteUI;
using System;
using TwelveEngine;
using Elves.UI;

namespace Elves.Scenes.SaveSelect {

    public sealed class SaveSelectUI:SpriteBook {

        public SaveSelectUI(SaveSelectScene scene) {
            Scene = scene;

            BackButton = AddButton(0,123);
            PlayButton = AddButton(17,123);
            AcceptButton = AddButton(0,140);
            DeleteButton = AddButton(17,140);

            Tag1 = AddTag();
            Tag2 = AddTag();
            Tag3 = AddTag();

            Finger = AddElement(new SpriteElement() {
                TextureSource = new(0,0,174,40),
                Offset = (new(-1,-0.1f)),
                PositionModeY = CoordinateMode.Relative,
                PositionModeX = CoordinateMode.Relative,
                Depth = 0.75f,
                SmoothStep = true,
                DefaultAnimationDuration = TimeSpan.FromSeconds(0.2f)
            });

            TagSelectPage = new TagSelectPage(this);
            TestPage = new TestPage1(this);

            SetPage(TagSelectPage,TimeSpan.Zero);
            foreach(var element in Elements) {
                element.SkipAnimation();
            }
        }

        public const int TagWidth = 128, TagHeight = 32;
        public const float TagRotation = -5f;

        public readonly SaveSelectUIPage TagSelectPage, TestPage;

        public SaveSelectScene Scene { get; private set; }

        public SpriteElement BackButton, PlayButton, AcceptButton, DeleteButton, Tag1, Tag2, Tag3, Finger;
        private SpriteElement AddTag() => AddElement(new Tag());
        private SpriteElement AddButton(int x,int y) => AddElement(new SpriteElement() {
            TextureSource = new(x,y,16,16),
            Offset = new(-0.5f,-0.5f)
        });

        public override SpriteElement AddElement(SpriteElement element) {
            base.AddElement(element);
            element.Texture = Program.Textures.SaveSelect;
            return element;
        }
    }
}
