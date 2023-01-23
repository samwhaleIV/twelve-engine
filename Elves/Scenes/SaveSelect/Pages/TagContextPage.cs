using TwelveEngine.UI;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagContextPage:SaveSelectPage {

        private Tag tag;

        private readonly List<Button> buttonList = new();
        private readonly HashSet<Button> disposedButtons = new();

        private void SetButtons(TimeSpan now,params Button[] buttons) {
            var buttonShiftTime = UI.TransitionDuration;

            foreach(var button in buttonList) {
                button.Flags = ElementFlags.None;
                button.Depth = Depth.OldButton;
                button.ClearKeyFocus();
                disposedButtons.Add(button);
            }

            buttonList.Clear();
            bool isEven = false;
            Element lastButton = null;
            foreach(var button in buttons) {
                button.KeyAnimation(now,buttonShiftTime);
                if(disposedButtons.Contains(button)) {
                    disposedButtons.Remove(button);
                }
                if(lastButton is not null) {
                    lastButton.NextElement = button;
                }
                button.PreviousElement = lastButton;
                lastButton = button;
                button.Scale = 1;
                button.Flags = ElementFlags.UpdateAndInteract;
                button.IsEven = isEven;
                isEven = !isEven;
                button.Depth = Depth.Button;
                button.PauseInputForAnimation();
                buttonList.Add(button);
            }
            foreach(var button in disposedButtons) {
                button.KeyAnimation(now,buttonShiftTime);
                button.Scale = 0;
            }
            disposedButtons.Clear();
        }

        public override void Open() {
            tag = UI.SelectedTag;
            tag.Flags = ElementFlags.CanUpdate;

            UI.OnButtonPresed += UI_OnButtonPresed;

            tag.Depth = Depth.FocusedTag;

            Scene.BackgroundZoomIn();

            switch(tag.Display) {
                case TagDisplay.Custom:
                    SetButtons(Now,UI.BackButton,UI.PlayButton,UI.DeleteButton);
                    DefaultFocusElement = UI.BackButton;
                    break;
                case TagDisplay.Empty:
                    tag.Display = TagDisplay.Create;
                    SetButtons(Now,UI.BackButton,UI.AcceptButton);
                    DefaultFocusElement = UI.BackButton;
                    break;
                default:
                    throw new InvalidOperationException($"Invalid tag opening state \"{tag.Display}\".");
            }
        }

        private void UI_OnButtonPresed(TimeSpan now,ButtonImpulse impulse) {
            switch(tag.Display) {
                case TagDisplay.Custom:
                    switch(impulse) {
                        case ButtonImpulse.Back:
                            UI.SetPage(now,UI.TagSelectPage);
                            break;
                        case ButtonImpulse.Play:
                            return;
                            throw new NotImplementedException();
                        case ButtonImpulse.Delete:
                            tag.Display = TagDisplay.Delete;
                            tag.Blip(now);
                            SetButtons(now,UI.BackButton,UI.AcceptButton);
                            UI.ResetInteractionState(UI.BackButton);
                            break;
                    }
                    break;
                case TagDisplay.Delete:
                    switch(impulse) {
                        case ButtonImpulse.Back:
                            tag.Display = TagDisplay.Custom;
                            tag.Blip(now);
                            SetButtons(Now,UI.BackButton,UI.PlayButton,UI.DeleteButton);
                            UI.ResetInteractionState(UI.BackButton);
                            break;
                        case ButtonImpulse.Accept:
                            tag.Display = TagDisplay.Empty;
                            Scene.DeleteSave(tag.ID);
                            UI.SetPage(now,UI.TagSelectPage);
                            break;
                    }
                    break;
                case TagDisplay.Create:
                    switch(impulse) {
                        case ButtonImpulse.Back:
                            UI.SetPage(now,UI.TagSelectPage);
                            break;
                        case ButtonImpulse.Accept:
                            tag.Display = TagDisplay.Custom;
                            UI.SetPage(now,UI.TagDrawPage);
                            break;
                    }
                    break;
            }
        }

        public override void Close() {
            tag.Depth = Depth.Tag;
            UI.OnButtonPresed -= UI_OnButtonPresed;
            if(tag.Display == TagDisplay.Create) {
                tag.Display = TagDisplay.Empty;
            }
            Scene.BackgroundZoomOut();
            foreach(var button in UI.Buttons) {
                button.Depth = Depth.OldButton;
            }
        }

        private void UpdateButtons(VectorRectangle viewport) {

            int buttonCount = buttonList.Count;

            float height = tag.Size.Y;
            float centerX = viewport.Width * 0.5f + height * 0.5f;
            float buttonY = viewport.Height * (5 / 7f);
            float centerOffset = height * 0.05f;
            float totalWidth = height * buttonCount + centerOffset * (buttonCount - 1);
            float xStart = centerX - totalWidth * 0.5f;

            for(int i = 0;i<buttonCount;i++) {
                var button = buttonList[i];
                button.Position = new Vector2(xStart + (centerOffset + height) * i,buttonY) / viewport.Size;
                button.Size = new(height);
            }
        }

        public override void Update(VectorRectangle viewport) {
            tag.Position = new(0.5f,2/7f);
            tag.Rotation = 0f;
            float height = viewport.Height * (1 / 3f);
            tag.Size = new(tag.GetWidth(height),height);

            UpdateButtons(viewport);
        }
    }
}
