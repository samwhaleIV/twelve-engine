using Elves.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagContextPage:SaveSelectUIPage {
        public TagContextPage(SaveSelectUI ui) : base(ui) {}

        private Tag tag;

        private readonly List<Button> buttonsList = new();

        private void SetButtons(TimeSpan now,params Button[] buttons) {
            foreach(var button in buttonsList) {
                button.KeyAnimation(now);
                button.Scale = 0;
                button.Flags = ElementFlags.None;
                button.Position = new(0.5f);
                button.ClearKeyFocus();
                button.Depth = SaveSelectDepth.OldButton;
            }
            buttonsList.Clear();
            bool isEven = false;

            Element lastButton = null;
            foreach(var button in buttons) {
                if(lastButton is not null) {
                    lastButton.NextElement = button;
                }
                button.PreviousElement = lastButton;
                lastButton = button;
                button.KeyAnimation(now);
                button.Scale = 1;
                button.Flags = ElementFlags.UpdateAndInteract;
                button.IsEven = isEven;
                isEven = !isEven;
                buttonsList.Add(button);
                button.Depth = SaveSelectDepth.Button;
            }
        }

        public override void Open() {
            tag = UI.SelectedTag;
            tag.Flags = ElementFlags.CanUpdate;

            UI.OnButtonPresed += UI_OnButtonPresed;

            tag.Depth = SaveSelectDepth.FocusedTag;

            UI.Scene.BackgroundZoomIn(Now,TransitionDuration);

            switch(tag.Display) {
                case TagDisplay.Custom:
                    break;
                case TagDisplay.Empty:
                    tag.Display = TagDisplay.Create;
                    SetButtons(Now,UI.BackButton,UI.AcceptButton);
                    DefaultFocusElement = UI.BackButton;
                    break;
                case TagDisplay.Delete:
                    break;
                case TagDisplay.Create:
                    break;
            }
        }

        private void UI_OnButtonPresed(TimeSpan now,ButtonImpulse impulse) {
            switch(tag.Display) {
                case TagDisplay.Custom:
                    break;
                case TagDisplay.Delete:
                    break;
                case TagDisplay.Create:
                    switch(impulse) {
                        case ButtonImpulse.Back:
                            UI.SetPage(UI.TagSelectPage,now);
                            break;
                        case ButtonImpulse.Play:
                            throw new NotImplementedException();
                            //set to tag creation/draw page
                            break;
                    }
                    break;
            }
        }

        public override void Close() {
            tag.Depth = SaveSelectDepth.Tag;
            UI.OnButtonPresed -= UI_OnButtonPresed;
            if(tag.Display == TagDisplay.Create) {
                tag.Display = TagDisplay.Empty;
            }
            UI.Scene.BackgroundZoomOut(Now,TransitionDuration);
        }

        private void UpdateButtons(VectorRectangle viewport) {

            int buttonCount = buttonsList.Count;

            float height = tag.Size.Y;
            float centerX = viewport.Width * 0.5f + height * 0.5f;
            float buttonY = viewport.Height * (5 / 7f);
            float centerOffset = height * 0.05f;
            float totalWidth = height * buttonCount + centerOffset * (buttonCount - 1);
            float xStart = centerX - totalWidth * 0.5f;

            for(int i = 0;i<buttonCount;i++) {
                var button = buttonsList[i];
                button.Position = new Vector2(xStart + (centerOffset + height) * i,buttonY) / viewport.Size;
                button.Size = new(height);
            }
        }

        public override void Update(VectorRectangle viewport) {
            tag.Position = new(0.5f,2/7f);
            tag.Rotation = 0f;
            float height = viewport.Height * 0.33f;
            tag.Size = new(height*(SaveSelectUI.TagWidth/SaveSelectUI.TagHeight),height);

            UpdateButtons(viewport);
        }
    }
}
