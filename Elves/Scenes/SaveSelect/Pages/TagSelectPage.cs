using TwelveEngine;
using Microsoft.Xna.Framework;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagSelectPage:SaveSelectPage {

        public override void Close() {

            UI.Tag1.OnActivated -= TagPressed;
            UI.Tag2.OnActivated -= TagPressed;
            UI.Tag3.OnActivated -= TagPressed;

            var finger = UI.Finger;
            finger.Offset = (new(-1,-0.1f));
            finger.Scale = 1;
            var newPos = finger.Position;
            newPos.X = 0;
            finger.Position = newPos;
        }

        public override BookElement Open() {
            BookElement tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;

            foreach(var tag in UI.Tags) {
                tag.Flags = ElementFlags.UpdateAndInteract;
            }

            UI.Finger.Scale = 1;

            tag1.SetKeyFocus(null,tag2);
            tag2.SetKeyFocus(tag1,tag3);
            tag3.SetKeyFocus(tag2,null);

            UI.Tag1.OnActivated += TagPressed;
            UI.Tag2.OnActivated += TagPressed;
            UI.Tag3.OnActivated += TagPressed;

            float tagRotation = -5f;
            foreach(var tag in UI.Tags) {
                tag.Rotation = tagRotation;
            }
            return UI.SelectedTag ?? tag1;
        }

        private void TagPressed(Tag tag) {
            UI.SelectedTag = tag;
            UI.SetPage(UI.TagContextPage);
        }

        private void UpdateFinger(FloatRectangle viewport) {
            var fingerHeight = 0.375f * viewport.Height;
            var fingerSize = new Vector2(174f / 40 * fingerHeight,fingerHeight);
            UI.Finger.Size = fingerSize;

            Vector2 fingerPosition = new(2/3f,UI.Finger.Position.Y);

            if(fingerPosition.Y < 0) {
                fingerPosition.Y = UI.Tag1.Position.Y;
            }

            fingerPosition.Y = UI.SelectedElement?.Position.Y ?? fingerPosition.Y;

            var finger = UI.Finger;
            if(finger.Position != fingerPosition) {
                finger.KeyAnimation(Now);
            }

            finger.Position = fingerPosition;
            finger.Offset = (new(-1.2f,-0.1f));
        }

        public override void Update(FloatRectangle viewport) {
            SpriteElement tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;
            float twoThirdsX = viewport.Width * (2 / 3f);
            float scale = viewport.Height / tag1.SourceHeight * 0.26f;

            float horizontalStep = scale * tag1.SourceHeight * 0.11f;
            tag1.Position = new((twoThirdsX+horizontalStep)/viewport.Width,1/5f);
            tag2.Position = new(twoThirdsX/viewport.Width,2/4f);
            tag3.Position = new((twoThirdsX-horizontalStep)/viewport.Width,4/5f);

            Vector2 size = scale * tag1.SourceSize;
            foreach(var tag in UI.Tags) {
                tag.Size = size;
            }

            UpdateFinger(viewport);
        }
    }
}
