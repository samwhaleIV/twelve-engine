using Elves.UI;
using System;
using TwelveEngine;
using Microsoft.Xna.Framework;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagSelectPage:SaveSelectUIPage {

        public TagSelectPage(SaveSelectUI owner) : base(owner) {
            DefaultFocusElement = owner.Tag1;
        }

        public override void Close() {
            UI.Tag1.OnActivated -= Tag1_OnActivated;
            UI.Tag2.OnActivated -= Tag2_OnActivated;
            UI.Tag3.OnActivated -= Tag3_OnActivated;
            UI.Finger.Scale = 1;
            var newPos = UI.Finger.Position;
            newPos.X = 0;
            UI.Finger.Position = newPos;
        }

        public override void Open() {
            Element tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;

            tag1.Flags = ElementFlags.UpdateAndInteract;
            tag2.Flags = ElementFlags.UpdateAndInteract;
            tag3.Flags = ElementFlags.UpdateAndInteract;

            UI.Finger.Scale = 1;

            tag1.Rotation = TagRotation;
            tag2.Rotation = TagRotation;
            tag3.Rotation = TagRotation;

            tag1.SetKeyFocus(null,tag2);
            tag2.SetKeyFocus(tag1,tag3);
            tag3.SetKeyFocus(tag2,null);

            UI.Tag1.OnActivated += Tag1_OnActivated;
            UI.Tag2.OnActivated += Tag2_OnActivated;
            UI.Tag3.OnActivated += Tag3_OnActivated;
        }

        private void Tag3_OnActivated(TimeSpan now) {
            UI.SelectedTag = UI.Tag3;
            UI.SetPage(UI.TestPage,now);
            DefaultFocusElement = UI.Tag3;
        }

        private void Tag2_OnActivated(TimeSpan now) {
            UI.SelectedTag = UI.Tag2;
            UI.SetPage(UI.TestPage,now);
            DefaultFocusElement = UI.Tag2;
        }

        private void Tag1_OnActivated(TimeSpan now) {
            UI.SelectedTag = UI.Tag1;
            UI.SetPage(UI.TestPage,now);
            DefaultFocusElement = UI.Tag1;
        }

        private const float TagRotation = 2f;

        private void UpdateFinger(VectorRectangle viewport) {
            var fingerHeight = 0.375f * viewport.Height;
            var fingerSize = new Vector2(174f / 40 * fingerHeight,fingerHeight);
            UI.Finger.Size = fingerSize;

            Vector2 fingerPosition = new(2.5f/5f,UI.Finger.Position.Y);

            fingerPosition.Y = UI.SelectedElement?.Position.Y ?? fingerPosition.Y;

            if(UI.Finger.Position != fingerPosition) {
                UI.Finger.KeyAnimation(Now);
            }

            UI.Finger.Position = fingerPosition;
        }

        public override void Update(VectorRectangle viewport) {
            Element tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;
            float twoThirdsX = viewport.Width * (2 / 3f);
            float scale = viewport.Height / SaveSelectUI.TagHeight * 0.26f;

            float horizontalStep = scale * SaveSelectUI.TagHeight * 0.11f;
            tag1.Position = new((twoThirdsX+horizontalStep)/viewport.Width,1/5f);
            tag2.Position = new(twoThirdsX/viewport.Width,2/4f);
            tag3.Position = new((twoThirdsX-horizontalStep)/viewport.Width,4/5f);

            Vector2 size = new(scale * SaveSelectUI.TagWidth,scale * SaveSelectUI.TagHeight);
            UI.Tag1.Size = size;
            UI.Tag2.Size = size;
            UI.Tag3.Size = size;

            tag1.Rotation = SaveSelectUI.TagRotation;
            tag2.Rotation = SaveSelectUI.TagRotation;
            tag3.Rotation = SaveSelectUI.TagRotation;

            UpdateFinger(viewport);
        }
    }
}
