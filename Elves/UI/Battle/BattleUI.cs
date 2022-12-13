using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Elves.UI.Font;
using TwelveEngine.Shell;
using TwelveEngine;

namespace Elves.UI.Battle {

    public sealed class BattleUI {

        private readonly GameManager game;
        public BattleUI(GameManager game) {
            this.game = game;

            playerHealthBar.Value = 1;
            targetHealthBar.Value = 1;

            foreach(var button in actionButtons) {
                interactableElements.Add(button);
                button.OnClick += ActionButtonClicked;
            }

            ActionButton1.Label.Append("TRY SPINNING");
            ActionButton2.Label.Append("THATS A");
            ActionButton3.Label.Append("NEAT");
            ActionButton4.Label.Append("TRICK");
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

        private readonly HealthBar playerHealthBar = new HealthBar() { Alignment = HealthBarAlignment.Left };
        private readonly HealthBar targetHealthBar = new HealthBar() { Alignment = HealthBarAlignment.Right };

        public Action<int> OnActionButtonClick;

        private void ActionButtonClicked(int ID) {
            OnActionButtonClick?.Invoke(ID);
        }

        //todo...
        //private readonly TargetButton targetButton = new TargetButton();
        //private readonly Tagline tagline = new Tagline();
        private readonly SpeechBox speechBox = new SpeechBox();

        public SpeechBox SpeechBox => speechBox;
      
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

        public void Update(float scale) {
            Rectangle viewport = Viewport;
            TimeSpan now = Now;

            float margin = scale;
            float halfMargin = margin * 0.5f;

            UpdateActionButtons(viewport,now,margin,halfMargin);
            UpdateHealthBars(viewport,scale,margin,halfMargin);
            speechBox.Update(now,viewport,margin);

            /* Update for buttons that are changing positions */
            UpdateButtonFocus(lastMousePosition.X,lastMousePosition.Y);
        }

        private void RenderActionButtons(SpriteBatch spriteBatch) {
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch);
            }
        }

        private void RenderHealthBars(SpriteBatch spriteBatch,UserRenderData playerData,UserRenderData targetData) {
            playerHealthBar.Draw(spriteBatch,playerData.Color);
            targetHealthBar.Draw(spriteBatch,targetData.Color);
        }

        private void RenderUsernames(UserRenderData playerData,UserRenderData targetData) {
            int usernameScale = (int)(playerHealthBar.Area.Height * 0.5f / Fonts.RetroFont.LineHeight);
            Color usernameColor = Color.White;

            Fonts.RetroFont.Draw(
                playerData.Name,
                new Point((int)playerHealthBar.Area.X,(int)(playerHealthBar.Area.Bottom + playerHealthBar.Area.Top)),
                usernameScale,usernameColor
            );
            Fonts.RetroFont.DrawRight(
                targetData.Name,
                new Point((int)targetHealthBar.Area.Right,(int)(targetHealthBar.Area.Bottom + targetHealthBar.Area.Top)),
                usernameScale,usernameColor
            );
        }

        private void RenderActionButtonText() {
            int buttonTextScale = (int)(ActionButton1.Area.Height / 6 / Fonts.RetroFont.LineHeight);
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.RetroFont,buttonTextScale,Color.White);
            }
        }

        public void Render(SpriteBatch spriteBatch,UserRenderData playerData,UserRenderData targetData) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);

            RenderActionButtons(spriteBatch);
            RenderHealthBars(spriteBatch,playerData,targetData);

            //todo... other elements
            spriteBatch.End();

            Fonts.RetroFont.Begin(spriteBatch);
            RenderUsernames(playerData,targetData);
            RenderActionButtonText();
            Fonts.RetroFont.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            speechBox.Draw(spriteBatch);
            spriteBatch.End();

            Fonts.DefaultFont.Begin(spriteBatch);
            speechBox.DrawText(Fonts.DefaultFont);
            Fonts.DefaultFont.End();
        }
    }
}
