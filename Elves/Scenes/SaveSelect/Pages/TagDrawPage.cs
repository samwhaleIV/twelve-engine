using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine;
using TwelveEngine.UI;
using TwelveEngine.Input;

namespace Elves.Scenes.SaveSelect {
    public sealed class TagDrawPage:SaveSelectPage {

        private Tag tag;
        private Button button;

        public override void Close() {
            UI.OnButtonPresed -= UI_OnButtonPresed;
            Scene.OnUpdate -= Scene_OnUpdate;
        }

        public override Element Open() {
            tag = UI.SelectedTag;
            tag.Flags = ElementFlags.Update;
            var label = UI.SignHereLabel;
            label.Scale = 1;

            var button = UI.AcceptButton;
            button.IsEven = true;
            button.Flags = ElementFlags.UpdateAndInteract;
            button.Depth = Depth.Button;
            this.button = button;

            UI.OnButtonPresed += UI_OnButtonPresed;
            Scene.OnUpdate += Scene_OnUpdate;

            return button;
        }

        public override void Update(VectorRectangle viewport) {
            tag.Position = new(0.5f,1/2f);
            tag.Rotation = 0f;
            float tagHeight = viewport.Height * (1 / 3f);
            tag.Size = new(tagHeight*tag.HeightToWidth,tagHeight);

            var label = UI.SignHereLabel;
            var labelHeight = tagHeight * (label.SourceHeight / tag.SourceHeight);
            label.Size = new(label.GetWidth(labelHeight),labelHeight);

            button.Size = new(labelHeight);
            button.Position = new(0.5f,4/5f);
        }

        private void Scene_OnUpdate() {
            MouseHandler mouse = Scene.Mouse;
            DrawingFrame drawingFrame = tag.DrawingFrame;
            if(!mouse.Capturing) {
                drawingFrame.ReleaseDraw();
                return;
            }
            VectorRectangle frameDestination = tag.ComputedArea.Destination;
            Vector2 relativePosition = (mouse.Position.ToVector2() - frameDestination.Position) / frameDestination.Size;
            drawingFrame.Draw(Scene.Game,relativePosition);
        }

        private void UI_OnButtonPresed(TimeSpan now,ButtonImpulse impulse) {
            if(impulse != ButtonImpulse.Accept) {
                throw new InvalidOperationException("Unexpected button impulse for tag draw page.");
            }
            UI.SetPage(UI.TagContextPage);
        }
    }
}
