using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Font;
using TwelveEngine.Shell;
using TwelveEngine;
using TwelveEngine.UI;

namespace Elves.Scenes.Battle.UI {

    public sealed class BattleUI:InteractionAgent<UIElement> {

        private readonly GameManager game;
        private readonly GameState owner;

        public BattleUI(GameState owner,GameManager game) {
            this.owner = owner;
            this.game = game;

            foreach(var button in actionButtons) {
                interactableElements.Add(button);
                button.OnActivated += Button_OnActivated;
            }

            Button1 = actionButtons[0];
            Button2 = actionButtons[1];
            Button3 = actionButtons[2];
            Button4 = actionButtons[3];

            DefaultFocusElement = Button1;
        }

        private void Button_OnActivated(int ID) {
            OnActionButtonClick?.Invoke(ID);
        }

        public Action<int> OnActionButtonClick;

        private readonly Button[] actionButtons = new Button[] {
            new() { State = ButtonState.TopLeft, ID = 0 },
            new() { State = ButtonState.TopRight, ID = 1 },
            new() { State = ButtonState.BottomLeft, ID = 2 },
            new() { State = ButtonState.BottomRight, ID = 3 }
        };

        public readonly Button Button1, Button2, Button3, Button4;

        public Button GetActionButton(int ID) {
            if(ID >= actionButtons.Length || ID < 0) {
                return null;
            }
            return actionButtons[ID];
        }

        private Rectangle Viewport => game.Viewport.Bounds;

        private readonly HealthBar playerHealthBar = new() { Alignment = HealthBarAlignment.Left };
        private readonly HealthBar targetHealthBar = new() { Alignment = HealthBarAlignment.Right };

        private readonly Tagline tagline = new();
        private readonly SpeechBox speechBox = new();

        public SpeechBox SpeechBox => speechBox;
        public Tagline Tagline => tagline;
      
        private readonly List<Button> interactableElements = new();

        #region INTERACTION AGENT

        protected override bool GetLastEventWasFromMouse() => GameManager.LastInputEventWasFromMouse;
        protected override bool GetContextTransitioning() => owner.IsTransitioning;
        protected override TimeSpan GetCurrentTime() => game.Time.TotalGameTime;
        protected override IEnumerable<UIElement> GetElements() => interactableElements;
        protected override bool BackButtonPressed() => false;

        #endregion
        #region UPDATE, RENDER, & LAYOUT

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

            playerHealthBar.ScreenArea = new VectorRectangle(
                playerHealthBarLeft,healthBarY,playerHealthBarRight-playerHealthBarLeft,healthBarHeight
            );
            targetHealthBar.ScreenArea = new VectorRectangle(
                targetHealthBarLeft,healthBarY,targetHealthBarRight-targetHealthBarLeft,healthBarHeight
            );

            playerHealthBar.Update(scale,Now);
            targetHealthBar.Update(scale,Now);
        }


        public void UpdateLayout(float scale) {
            Rectangle viewport = Viewport;
            TimeSpan now = Now;

            float margin = scale;
            float halfMargin = margin * 0.5f;

            UpdateActionButtons(viewport,now,margin,halfMargin);
            UpdateHealthBars(viewport,scale,margin,halfMargin);
            speechBox.Update(now,viewport);
            tagline.Update(now,viewport,margin);
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
            int usernameScale = (int)(playerHealthBar.ScreenArea.Height * 0.5f / Fonts.RetroFont.LineHeight);
            Color usernameColor = Color.White;
            if(playerData != null && playerData.Name != null) {
                Fonts.RetroFont.Draw(
                    playerData.Name,
                    new Point((int)playerHealthBar.ScreenArea.X,(int)(playerHealthBar.ScreenArea.Bottom + playerHealthBar.ScreenArea.Top)),
                    usernameScale,usernameColor
                );
            }
            if(targetData != null && targetData.Name != null) {
                Fonts.RetroFont.DrawRight(
                    targetData.Name,
                    new Point((int)targetHealthBar.ScreenArea.Right,(int)(targetHealthBar.ScreenArea.Bottom + targetHealthBar.ScreenArea.Top)),
                    usernameScale,usernameColor
                );
            }
        }

        private void RenderActionButtonText() {
            int buttonTextScale = (int)(Button1.ScreenArea.Height / 50f);
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

            Fonts.RetroFont.Begin(spriteBatch);
            RenderActionButtonText();
            Fonts.RetroFont.End();

            RenderTagline(spriteBatch);
        }
        #endregion
    }
}
