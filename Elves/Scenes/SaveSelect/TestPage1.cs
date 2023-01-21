using Elves.UI;
using System;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class TestPage1:SaveSelectUIPage {
        public TestPage1(SaveSelectUI ui) : base(ui) { }

        public override void Open(TimeSpan now) {
            DefaultFocusElement = UI.Tag1;
            UI.Tag1.Flags = ElementFlags.UpdateAndInteract;
            UI.Tag1.OnActivated += Tag1_OnActivated;
            UI.Tag1.Depth = 1f;
        }

        public override void Close() {
            UI.Tag1.Depth = 0.5f;
            UI.Tag1.OnActivated -= Tag1_OnActivated;
        }

        private void Tag1_OnActivated(TimeSpan now) {
            UI.SetPage(UI.TagSelectPage,now);
        }

        public override void Update(TimeSpan now,VectorRectangle viewport) {
            UI.Tag1.Position = new(0.5f);
            UI.Tag1.Rotation = 0f;
            float height = viewport.Height * 0.25f;
            UI.Tag1.Size = new(height*(SaveSelectUI.TagWidth/SaveSelectUI.TagHeight),height);
        }
    }
}
