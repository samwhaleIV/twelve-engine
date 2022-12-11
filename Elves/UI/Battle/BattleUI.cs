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
            }
            TopLeftButton.OnClick += TestButtonAnimation;

            playerHealthBar.Value = 1;
            targetHealthBar.Value = 1;

            TopRightButton.OnClick += () => {
                playerHealthBar.DropHealthAnimate(Now);
                playerHealthBar.Value -= 0.05f;
                if(playerHealthBar.Value < 0) {
                    playerHealthBar.Value = 1;
                }
            };

            BottomRightButton.OnClick += () => {
                targetHealthBar.DropHealthAnimate(Now);
                targetHealthBar.Value -= 0.05f;
                if(targetHealthBar.Value < 0) {
                    targetHealthBar.Value = 1;
                }
            };
            interactableElements.Add(targetButton);
        }

        private TimeSpan Now => game.Time.TotalGameTime;
        private Rectangle Viewport => game.Viewport.Bounds;

        private Button activeButton = null, pressedButton = null;

        private readonly HealthBar playerHealthBar = new HealthBar() {
            Alignment = HealthBarAlignment.Left
        };

        private readonly HealthBar targetHealthBar = new HealthBar() {
            Alignment = HealthBarAlignment.Right
        };

        private readonly Tagline tagline = new Tagline();

        private ActionButton[] actionButtons = new ActionButton[] {
            new ActionButton() { OffscreenDirection = OffscreenDirection.Left },
            new ActionButton() { OffscreenDirection = OffscreenDirection.Left },
            new ActionButton() { OffscreenDirection = OffscreenDirection.Right },
            new ActionButton() { OffscreenDirection = OffscreenDirection.Right }
        };

        private ActionButton TopLeftButton => actionButtons[0];
        private ActionButton BottomLeftButton => actionButtons[1];
        private ActionButton TopRightButton => actionButtons[2];
        private ActionButton BottomRightButton => actionButtons[3];

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
            return newButton;
        }

        private void UpdateButtonFocus(int x,int y) {
            lastMousePosition = new Point(x,y);
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
            lastMousePosition = new Point(x,y);
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


        public void Update(int scale) {

            Rectangle viewport = Viewport;
            TimeSpan now = Now;

            int centerX = viewport.Center.X;

            int margin = scale;
            int halfMargin = margin / 2;

            int buttonHeight = viewport.Height / 4 - margin;
            int buttonWidth = buttonHeight * 2;

            int buttonCenterY = (int)(viewport.Height * 0.75f) - margin * 2 + halfMargin;

            int buttonMargin = margin;

            int bottomRowY = buttonCenterY + buttonMargin;
            int topRowY = buttonCenterY - buttonMargin - buttonHeight;

            BottomLeftButton.Area = new Rectangle(centerX-buttonMargin-buttonWidth,bottomRowY,buttonWidth,buttonHeight);
            BottomRightButton.Area = new Rectangle(centerX+buttonMargin,bottomRowY,buttonWidth,buttonHeight);

            TopLeftButton.Area = new Rectangle(centerX-buttonMargin-buttonWidth,topRowY,buttonWidth,buttonHeight);
            TopRightButton.Area = new Rectangle(centerX+buttonMargin,topRowY,buttonWidth,buttonHeight);

            int healthBarY = margin * 2;

            int playerHealthBarLeft = margin * 2;
            int playerHealthBarRight = centerX - margin;

            int targetHealthBarLeft = centerX + margin;
            int targetHealthBarRight = viewport.Right - margin * 2;

            int healthBarHeight = buttonHeight / 2;

            playerHealthBar.Area = new Rectangle(playerHealthBarLeft,healthBarY,playerHealthBarRight-playerHealthBarLeft,healthBarHeight);
            targetHealthBar.Area = new Rectangle(targetHealthBarLeft,healthBarY,targetHealthBarRight-targetHealthBarLeft,healthBarHeight);

            foreach(var button in actionButtons) {
                button.Update(viewport,now);
            }

            playerHealthBar.Update(scale,Now);
            targetHealthBar.Update(scale,Now);

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

        public void Render(int scale,Color playerTint,Color targetTint,SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch,targetTint);
            }
            playerHealthBar.Draw(spriteBatch,playerTint);
            targetHealthBar.Draw(spriteBatch,targetTint);
            spriteBatch.End();
            Fonts.DefaultFont.Begin(spriteBatch);
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.DefaultFont,scale);
            }
            Fonts.DefaultFont.End();
        }
    }
}
