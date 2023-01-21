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
        }

        public override void Open(TimeSpan now) {
            Element tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;

            tag1.Flags = ElementFlags.UpdateAndInteract;
            tag2.Flags = ElementFlags.UpdateAndInteract;
            tag3.Flags = ElementFlags.UpdateAndInteract;

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
            UI.SetPage(UI.TestPage,now);
        }

        private void Tag2_OnActivated(TimeSpan now) {
            UI.SetPage(UI.TestPage,now);
        }

        private void Tag1_OnActivated(TimeSpan now) {
            UI.SetPage(UI.TestPage,now);
        }

        private const float TagRotation = 2f;

        public override void Update(VectorRectangle viewport) {
            Element tag1 = UI.Tag1, tag2 = UI.Tag2, tag3 = UI.Tag3;
            float twoThirdsX = viewport.Width * (2 / 3f);
            float scale = viewport.Height / SaveSelectUI.TagHeight * 0.26f;

            float horizontalStep = scale * SaveSelectUI.TagHeight * 0.11f;
            tag1.Position = new(twoThirdsX+horizontalStep,viewport.Height * (1/5f));
            tag2.Position = new(twoThirdsX,viewport.Height * (2/4f));
            tag3.Position = new(twoThirdsX-horizontalStep,viewport.Height * (4/5f));

            Vector2 size = new(scale * SaveSelectUI.TagWidth,scale * SaveSelectUI.TagHeight);
            UI.Tag1.Size = size;
            UI.Tag2.Size = size;
            UI.Tag3.Size = size;

            tag1.Rotation = SaveSelectUI.TagRotation;
            tag2.Rotation = SaveSelectUI.TagRotation;
            tag3.Rotation = SaveSelectUI.TagRotation;
        }
    }
}
