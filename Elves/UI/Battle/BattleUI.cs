﻿using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Elves.UI.Font;
using TwelveEngine.Shell;
using System.Text;

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
                    playerHealthBar.Value = 0;
                }
            };

            BottomRightButton.OnClick += () => {
                targetHealthBar.DropHealthAnimate(Now);
                targetHealthBar.Value -= 0.05f;
                if(targetHealthBar.Value < 0) {
                    targetHealthBar.Value = 0;
                }
            };
            interactableElements.Add(targetButton);

            TopLeftButton.Label.Append("TRY SPINNING");
            TopRightButton.Label.Append("THATS A");
            BottomLeftButton.Label.Append("NEAT");
            BottomRightButton.Label.Append("TRICK");
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

            int buttonHeight = viewport.Height / 4;
            int buttonWidth = buttonHeight * 2;

            int bottomRowY = viewport.Bottom - buttonHeight - margin;
            int topRowY = bottomRowY - margin - buttonHeight;

            int buttonXMargin = halfMargin;

            BottomLeftButton.Area = new Rectangle(centerX-buttonXMargin-buttonWidth,bottomRowY,buttonWidth,buttonHeight);
            BottomRightButton.Area = new Rectangle(centerX+buttonXMargin,bottomRowY,buttonWidth,buttonHeight);

            TopLeftButton.Area = new Rectangle(centerX-buttonXMargin-buttonWidth,topRowY,buttonWidth,buttonHeight);
            TopRightButton.Area = new Rectangle(centerX+buttonXMargin,topRowY,buttonWidth,buttonHeight);

            int healthBarY = margin;

            int playerHealthBarLeft = margin;
            int playerHealthBarRight = centerX - halfMargin;

            int targetHealthBarLeft = centerX + halfMargin;
            int targetHealthBarRight = viewport.Right - margin;

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

        public void Render(
            SpriteBatch spriteBatch,

            UserRenderData playerData,
            UserRenderData targetData
        ) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch);
            }
            playerHealthBar.Draw(spriteBatch,playerData.Color);
            targetHealthBar.Draw(spriteBatch,targetData.Color);
            spriteBatch.End();

            int usernameScale = playerHealthBar.Area.Height / 2 / Fonts.RetroFont.LineHeight;
            Color usernameColor = Color.White;

            int margin = playerHealthBar.Area.X;

            Fonts.RetroFont.Begin(spriteBatch);
            Fonts.RetroFont.Draw(
                playerData.Name,
                new Point(playerHealthBar.Area.X,playerHealthBar.Area.Bottom + margin),
                usernameScale,usernameColor
            );
            Fonts.RetroFont.DrawRight(
                targetData.Name,
                new Point(targetHealthBar.Area.Right,targetHealthBar.Area.Bottom + margin),
                usernameScale,usernameColor
            );
            int buttonTextScale = TopLeftButton.Area.Height / 6 / Fonts.RetroFont.LineHeight;
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.RetroFont,buttonTextScale,Color.White);
            }
            Fonts.RetroFont.End();
        }
    }
}
