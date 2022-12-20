using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Elves.UI.Font;
using TwelveEngine.Shell;
using TwelveEngine;
using Elves.Battle;

namespace Elves.UI.Battle {

    public sealed class BattleUI {

        private readonly GameManager game;
        public BattleUI(GameManager game) {
            this.game = game;
            foreach(var button in actionButtons) {
                interactableElements.Add(button);
                button.OnClick += ActionButtonClicked;
            }
        }

        public const int ACTION_BUTTON_1 = 0;
        public const int ACTION_BUTTON_2 = 1;
        public const int ACTION_BUTTON_3 = 2;
        public const int ACTION_BUTTON_4 = 3;

        private readonly ActionButton[] actionButtons = new ActionButton[] {
            new ActionButton() { State = ButtonState.TopLeft, ID = ACTION_BUTTON_1 },
            new ActionButton() { State = ButtonState.TopRight, ID = ACTION_BUTTON_2 },
            new ActionButton() { State = ButtonState.BottomLeft, ID = ACTION_BUTTON_3 },
            new ActionButton() { State = ButtonState.BottomRight, ID = ACTION_BUTTON_4 }
        };

        public ActionButton GetActionButton(int ID) {
            if(ID >= actionButtons.Length || ID < 0) {
                return null;
            }
            return actionButtons[ID];
        }

        public ActionButton ActionButton1 => actionButtons[ACTION_BUTTON_1];
        public ActionButton ActionButton2 => actionButtons[ACTION_BUTTON_2];
        public ActionButton ActionButton3 => actionButtons[ACTION_BUTTON_3];
        public ActionButton ActionButton4 => actionButtons[ACTION_BUTTON_4];

        private TimeSpan Now => game.Time.TotalGameTime;
        private Rectangle Viewport => game.Viewport.Bounds;

        private Button activeButton = null, pressedButton = null;

        private readonly HealthBar playerHealthBar = new() { Alignment = HealthBarAlignment.Left };
        private readonly HealthBar targetHealthBar = new() { Alignment = HealthBarAlignment.Right };

        public Action<int> OnActionButtonClick;

        private void ActionButtonClicked(int ID) {
            OnActionButtonClick?.Invoke(ID);
        }

        //todo...
        //private readonly TargetButton targetButton = new TargetButton();
        private readonly Tagline tagline = new();
        private readonly SpeechBox speechBox = new();

        public SpeechBox SpeechBox => speechBox;
        public Tagline Tagline => tagline;
      
        private readonly List<Button> interactableElements = new();
        private static readonly Point OffscreenMousePosition = new(-1);
        private Point lastMousePosition = OffscreenMousePosition;

        private Button GetButtonAtPosition(Point position) {
            if(!Viewport.Contains(position)) {
                lastMousePosition = OffscreenMousePosition;
                return null;
            }
            Button newButton = null;
            for(int i = interactableElements.Count-1;i>=0;i--) {
                var button = interactableElements[i];
                if(button.Area.Contains(position)) {
                    newButton = button;
                    break;
                }
            }
            return newButton;
        }

        private void UpdateButtonFocus(Point position) {
            lastMousePosition = position;
            var button = GetButtonAtPosition(position);
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

        public void MouseMoved(Point position) {
            UpdateButtonFocus(position);
        }

        public void MousePress(Point position) {
            UpdateButtonFocus(position);
            if(activeButton == null) {
                return;
            }
            pressedButton = activeButton;
            activeButton.Pressed = true;
        }

        public void MouseRelease(Point position) {
            lastMousePosition = position;
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

        public void UpdateActionButtons(Rectangle viewport,TimeSpan now,float margin,float halfMargin) {
            float buttonHeight = viewport.Height * 0.25f;
            float buttonWidth = buttonHeight * 2;

            float buttonCenterY = viewport.Bottom - margin - buttonHeight - halfMargin;

            var buttonRenderData = new ButtonRenderData(
                viewport,buttonWidth,buttonHeight,viewport.Center.X,buttonCenterY,halfMargin
            );

            foreach(var button in actionButtons) {
                button.Update(now,buttonRenderData);
            }
        }

        public void UpdateHealthBars(Rectangle viewport,float scale,float margin,float halfMargin) {
            float healthBarY = margin;

            float centerX = viewport.Center.X;

            float playerHealthBarLeft = margin;
            float playerHealthBarRight = centerX - halfMargin;

            float targetHealthBarLeft = centerX + halfMargin;
            float targetHealthBarRight = viewport.Right - margin;

            float healthBarHeight = viewport.Height * 0.125f; /* Equal to half of action button height */

            playerHealthBar.Area = new VectorRectangle(
                playerHealthBarLeft,healthBarY,playerHealthBarRight-playerHealthBarLeft,healthBarHeight
            );
            targetHealthBar.Area = new VectorRectangle(
                targetHealthBarLeft,healthBarY,targetHealthBarRight-targetHealthBarLeft,healthBarHeight
            );

            playerHealthBar.Update(scale,Now);
            targetHealthBar.Update(scale,Now);
        }

        public CursorState GetCursorState() {
            if(pressedButton != null && activeButton == pressedButton) {
                return CursorState.Pressed;
            }
            if(activeButton != null && activeButton.IsEnabled) {
                return CursorState.Interact;
            }
            return CursorState.Default;
        }

        public void Update(float scale) {
            Rectangle viewport = Viewport;
            TimeSpan now = Now;

            float margin = scale;
            float halfMargin = margin * 0.5f;

            UpdateActionButtons(viewport,now,margin,halfMargin);
            UpdateHealthBars(viewport,scale,margin,halfMargin);
            speechBox.Update(now,viewport);
            tagline.Update(now,viewport,margin);

            /* Update for buttons that are changing positions */
            UpdateButtonFocus(lastMousePosition);
        }

        private void RenderActionButtons(SpriteBatch spriteBatch) {
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch);
            }
        }

        private void RenderHealthBars(SpriteBatch spriteBatch,UserData playerData,UserData targetData) {
            if(playerData != null) {
                playerHealthBar.Value = playerData.HealthFraction;
                playerHealthBar.Draw(spriteBatch,playerData.Color);

            }
            if(targetData != null) {
                targetHealthBar.Value = targetData.HealthFraction;
                targetHealthBar.Draw(spriteBatch,targetData.Color);
            }
        }

        private void RenderUsernames(UserData playerData,UserData targetData) {
            int usernameScale = (int)(playerHealthBar.Area.Height * 0.5f / Fonts.RetroFont.LineHeight);
            Color usernameColor = Color.White;
            if(playerData != null && playerData.Name != null) {
                Fonts.RetroFont.Draw(
                    playerData.Name,
                    new Point((int)playerHealthBar.Area.X,(int)(playerHealthBar.Area.Bottom + playerHealthBar.Area.Top)),
                    usernameScale,usernameColor
                );
            }
            if(targetData != null && targetData.Name != null) {
                Fonts.RetroFont.DrawRight(
                    targetData.Name,
                    new Point((int)targetHealthBar.Area.Right,(int)(targetHealthBar.Area.Bottom + targetHealthBar.Area.Top)),
                    usernameScale,usernameColor
                );
            }
        }

        private void RenderActionButtonText() {
            int buttonTextScale = (int)(ActionButton1.Area.Height / 50f);
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.RetroFont,buttonTextScale,Color.White);
            }
        }

        private void RenderTagline(SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            tagline.Draw(spriteBatch);
            spriteBatch.End();
            Fonts.RetroFont.Begin(spriteBatch);
            tagline.DrawText(Fonts.RetroFont);
            Fonts.RetroFont.End();
        }

        public void Render(SpriteBatch spriteBatch,UserData playerData,UserData targetData) {

            Fonts.RetroFont.Begin(spriteBatch);
            RenderUsernames(playerData,targetData);
            Fonts.RetroFont.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            speechBox.Draw(spriteBatch,targetData.Color);
            spriteBatch.End();

            Fonts.RetroFont.Begin(spriteBatch);
            speechBox.DrawText(Fonts.RetroFont);
            Fonts.RetroFont.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            RenderActionButtons(spriteBatch);
            RenderHealthBars(spriteBatch,playerData,targetData);
            spriteBatch.End();

            RenderTagline(spriteBatch);

            Fonts.RetroFont.Begin(spriteBatch);
            RenderActionButtonText();
            Fonts.RetroFont.End();
        }
    }
}
