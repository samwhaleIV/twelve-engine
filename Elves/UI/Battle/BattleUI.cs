using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Elves.UI.Font;
using TwelveEngine.Shell;

namespace Elves.UI.Battle {

    public sealed class BattleUI {

        private readonly GameManager game;
        public BattleUI(GameManager game) {
            this.game = game;
            foreach(var button in actionButtons) {
                interactableElements.Add(button);
                button.OnClick += TestButtonAnimation;
            }
            interactableElements.Add(targetButton);
        }

        private TimeSpan Now => game.Time.TotalGameTime;
        private Rectangle Viewport => game.Viewport.Bounds;

        private Button activeButton = null, pressedButton = null;

        private readonly HealthBar playerHealthBar = new HealthBar(), targetHealthBar = new HealthBar();

        private readonly Tagline tagline = new Tagline();

        private ActionButton[] actionButtons = new ActionButton[] {
            new ActionButton() { Position = ButtonPosition.TopLeft },
            new ActionButton() { Position = ButtonPosition.TopRight },
            new ActionButton() { Position = ButtonPosition.BottomLeft },
            new ActionButton() { Position = ButtonPosition.BottomRight }
        };

        private readonly SpeechBox speechBox = new SpeechBox();
        private readonly TargetButton targetButton = new TargetButton();

        private readonly List<Button> interactableElements = new List<Button>();

        private static readonly Point OffscreenMousePosition = new Point(-1);

        private Point lastMousePosition = OffscreenMousePosition;

        private Button GetButtonAtPosition(int x,int y) {
            if(!Viewport.Contains(x,y)) {
                lastMousePosition = OffscreenMousePosition;
                return null;
            }
            Button newButton = null;
            foreach(var button in interactableElements) {
                if(button.Area.Contains(x,y)) {
                    newButton = button;
                    break;
                }
            }
            lastMousePosition = new Point(x,y);
            return newButton;
        }

        private void UpdateButtonFocus(int x,int y) {
            var button = GetButtonAtPosition(x,y);
            if(activeButton != null) {
                activeButton.Selected = false;
            }
            if(button == null) {
                activeButton = null;
                return;
            }
            if(pressedButton != null && button != pressedButton) {
                activeButton = null;
                return;
            }
            button.Selected = true;
            activeButton = button;
        }

        public void MouseMoved(int x,int y) {
            UpdateButtonFocus(x,y);
        }

        public void MousePress(int x,int y) {
            UpdateButtonFocus(x,y);
            if(activeButton == null) {
                return;
            }
            pressedButton = activeButton;
            activeButton.Pressed = true;
        }

        public void MouseRelease(int x,int y) {
            UpdateButtonFocus(x,y);
            if(activeButton == null || pressedButton == null || pressedButton != activeButton) {
                if(pressedButton == null) {
                    return;
                }
                pressedButton.Pressed = false;
                pressedButton = null;
                return;
            }
            var button = pressedButton;
            pressedButton = null;
            button.Click();
            button.Pressed = false;
        }

        public void UpdateUI(int scale) {
            Rectangle viewport = Viewport;
            float baseHeight = viewport.Height * 0.2f;

            float buttonScale = baseHeight / 15;
            float margin = buttonScale / 2;



            int buttonHeight = (int)(baseHeight - margin * 2f);
            int buttonWidth = buttonHeight * 3;

            int centerX = viewport.Center.X;
            int buttonCenterY = (int)(viewport.Height * 0.75f);

            foreach(var button in actionButtons) {
                button.Scale = buttonHeight / 15;
                button.Update(buttonWidth,buttonHeight,centerX,buttonCenterY,viewport.X,viewport.Width,(int)margin,Now);
            }
            /* Update for buttons that are changing positions */
            UpdateButtonFocus(lastMousePosition.X,lastMousePosition.Y);
        }

        public bool moveDirection = true;

        public void TestButtonAnimation() {
            if(moveDirection) {
                foreach(var button in actionButtons) {
                    button.MoveAway();
                }
            } else {
                foreach(var button in actionButtons) {
                    button.MoveBack();
                }
            }
            moveDirection = !moveDirection;
        }

        public void Render(int scale,Color tint,SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch,tint);
            }
            spriteBatch.End();
            Fonts.DefaultFont.Begin(spriteBatch);
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.DefaultFont,scale);
            }
            Fonts.DefaultFont.End();
        }
    }
}
