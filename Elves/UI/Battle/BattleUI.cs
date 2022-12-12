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
        //private readonly SpeechBox speechBox = new SpeechBox();
      
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

        public void UpdateActionButtons(Rectangle viewport,TimeSpan now,int margin,int halfMargin) {
            int buttonHeight = viewport.Height / 4;
            int buttonWidth = buttonHeight * 2;

            int buttonCenterY = viewport.Bottom - margin - buttonHeight - halfMargin;

            var buttonRenderData = new ButtonRenderData(
                viewport,buttonWidth,buttonHeight,viewport.Center.X,buttonCenterY,halfMargin
            );

            foreach(var button in actionButtons) {
                button.Update(now,buttonRenderData);
            }
        }

        public void UpdateHealthBars(Rectangle viewport,int scale,int margin,int halfMargin) {
            int healthBarY = margin;

            int centerX = viewport.Center.X;

            int playerHealthBarLeft = margin;
            int playerHealthBarRight = centerX - halfMargin;

            int targetHealthBarLeft = centerX + halfMargin;
            int targetHealthBarRight = viewport.Right - margin;

            int healthBarHeight = viewport.Height / 8; /* Equal to half of action button height */

            playerHealthBar.Area = new Rectangle(playerHealthBarLeft,healthBarY,playerHealthBarRight-playerHealthBarLeft,healthBarHeight);
            targetHealthBar.Area = new Rectangle(targetHealthBarLeft,healthBarY,targetHealthBarRight-targetHealthBarLeft,healthBarHeight);

            playerHealthBar.Update(scale,Now);
            targetHealthBar.Update(scale,Now);
        }

        public void Update(int scale) {
            Rectangle viewport = Viewport;
            TimeSpan now = Now;

            int margin = scale;
            int halfMargin = margin / 2;

            UpdateActionButtons(viewport,now,margin,halfMargin);
            UpdateHealthBars(viewport,scale,margin,halfMargin);

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
            int usernameScale = playerHealthBar.Area.Height / 2 / Fonts.RetroFont.LineHeight;
            Color usernameColor = Color.White;

            Fonts.RetroFont.Draw(
                playerData.Name,
                new Point(playerHealthBar.Area.X,playerHealthBar.Area.Bottom + playerHealthBar.Area.Top),
                usernameScale,usernameColor
            );
            Fonts.RetroFont.DrawRight(
                targetData.Name,
                new Point(targetHealthBar.Area.Right,targetHealthBar.Area.Bottom + targetHealthBar.Area.Top),
                usernameScale,usernameColor
            );
        }

        private void RenderActionButtonText() {
            int buttonTextScale = ActionButton1.Area.Height / 6 / Fonts.RetroFont.LineHeight;
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
        }
    }
}
