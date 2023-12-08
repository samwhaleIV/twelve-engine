using Microsoft.Xna.Framework;
using System;
using TwelveEngine;
using TwelveEngine.Effects;
using TwelveEngine.Input.Routing;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagDrawPage:SaveSelectPage {

        private Tag tag;
        private Button button;

        private int updateID;

        public override void Close() {
            UI.OnButtonPresed -= ButtonPressed;
            Scene.OnUpdate.Remove(updateID);
            Scene.CreateSave(UI.SelectedTag.ID);
        }

        public override BookElement Open() {
            tag = UI.SelectedTag;
            tag.Flags = ElementFlags.Update;
            var label = UI.SignHereLabel;
            label.Scale = 1;

            var button = UI.AcceptButton;
            button.IsEven = true;
            button.Flags = ElementFlags.UpdateAndInteract;
            button.Depth = Depth.Button;
            this.button = button;

            UI.OnButtonPresed += ButtonPressed;

            updateID = Scene.OnUpdate.Add(UpdateTagDrawing);

            return button;
        }

        public override void Update() {
            tag.Position = new(0.5f,1/2f);
            tag.Rotation = 0f;
            float tagHeight = Viewport.Height * (1 / 3f);
            tag.Size = new(tagHeight*tag.HeightToWidth,tagHeight);

            var label = UI.SignHereLabel;
            var labelHeight = tagHeight * (label.SourceHeight / tag.SourceHeight);
            label.Size = new(label.GetWidth(labelHeight),labelHeight);

            button.Size = new(labelHeight);
            button.Position = new(0.5f,4/5f);
        }

        private void UpdateTagDrawing() {
            MouseEventHandler mouse = Scene.MouseHandler;
            DrawingFrame drawingFrame = tag.DrawingFrame;
            if(!mouse.Capturing) {
                drawingFrame.ReleaseDraw();
                return;
            }
            FloatRectangle frameDestination = tag.ComputedArea;
            Vector2 relativePosition = (mouse.Position.ToVector2() - frameDestination.Position) / frameDestination.Size;
            drawingFrame.Draw(Scene,relativePosition);
        }

        private void ButtonPressed(ButtonImpulse impulse) {
            if(impulse != ButtonImpulse.Accept) {
                throw new InvalidOperationException("Unexpected button impulse on tag draw page.");
            }
            UI.SetPage(UI.TagContextPage);
        }
    }
}
