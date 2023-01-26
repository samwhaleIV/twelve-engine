using TwelveEngine.UI;
using System;
using TwelveEngine;
using Microsoft.Xna.Framework;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagSelectPage:SaveSelectPage {

        public override void Close() {

            UI.Tag1.OnActivated -= Tag1_OnActivated;
            UI.Tag2.OnActivated -= Tag2_OnActivated;
            UI.Tag3.OnActivated -= Tag3_OnActivated;

            var finger = UI.Finger;
            finger.Offset = (new(-1,-0.1f));
            finger.Scale = 1;
            var newPos = finger.Position;
            newPos.X = 0;
            finger.Position = newPos;
        }

        public override Element Open() {
            Element tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;

            foreach(var tag in UI.Tags) {
                tag.Flags = ElementFlags.UpdateAndInteract;
            }

            UI.Finger.Scale = 1;

            tag1.SetKeyFocus(null,tag2);
            tag2.SetKeyFocus(tag1,tag3);
            tag3.SetKeyFocus(tag2,null);

            UI.Tag1.OnActivated += Tag1_OnActivated;
            UI.Tag2.OnActivated += Tag2_OnActivated;
            UI.Tag3.OnActivated += Tag3_OnActivated;

            float tagRotation = -5f;
            foreach(var tag in UI.Tags) {
                tag.Rotation = tagRotation;
            }
            return UI.SelectedTag ?? tag1;
        }

        private void Tag3_OnActivated(TimeSpan now) {
            UI.SelectedTag = UI.Tag3;
            UI.SetPage(UI.TagContextPage);
        }

        private void Tag2_OnActivated(TimeSpan now) {
            UI.SelectedTag = UI.Tag2;
            UI.SetPage(UI.TagContextPage);
        }

        private void Tag1_OnActivated(TimeSpan now) {
            UI.SelectedTag = UI.Tag1;
            UI.SetPage(UI.TagContextPage);
        }

        private void UpdateFinger(VectorRectangle viewport) {
            var fingerHeight = 0.375f * viewport.Height;
            var fingerSize = new Vector2(174f / 40 * fingerHeight,fingerHeight);
            UI.Finger.Size = fingerSize;

            Vector2 fingerPosition = new(2/3f,UI.Finger.Position.Y);

            fingerPosition.Y = UI.SelectedElement?.Position.Y ?? fingerPosition.Y;

            var finger = UI.Finger;
            if(finger.Position != fingerPosition) {
                finger.KeyAnimation(Now);
            }

            finger.Position = fingerPosition;
            finger.Offset = (new(-1.2f,-0.1f));
        }

        public override void Update(VectorRectangle viewport) {
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
