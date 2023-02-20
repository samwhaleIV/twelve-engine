using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Font;
using TwelveEngine.Shell;
using TwelveEngine;
using TwelveEngine.UI;
using Elves.Battle;

namespace Elves.Scenes.Battle.UI {

    public sealed class BattleUI:InteractionAgent<UIElement> {

        private readonly InputGameState owner;

        private Effect healthBarEffect;
        private EffectParameter healthBarColorStartParameter, healthBarColorEndParameter;

        private static readonly Color HealthBarOffColor = new(25,25,25,255);

        public BattleUI(InputGameState owner) {
            this.owner = owner;

            foreach(var button in actionButtons) {
                interactableElements.Add(button);
                button.OnActivated += ButtonClicked;
            }

            Button1 = actionButtons[0];
            Button2 = actionButtons[1];
            Button3 = actionButtons[2];
            Button4 = actionButtons[3];

            DefaultFocusElement = Button1;

            LoadHealthBarEffect();

            owner.OnInputActivated += FocusDefault;
        }

        private void LoadHealthBarEffect() {
            healthBarEffect = owner.Content.Load<Effect>("Shaders/HealthBarEffect");
            healthBarEffect.Parameters["OffColor"].SetValue(HealthBarOffColor.ToVector4());
            healthBarColorStartParameter = healthBarEffect.Parameters["OffColorStart"];
            healthBarColorEndParameter = healthBarEffect.Parameters["OffColorEnd"];
        }

        protected override bool GetLeftMouseButtonIsCaptured() {
            return owner.Mouse.CapturingLeft;
        }

        protected override bool GetRightMouseButtonIsCaptured() {
            return owner.Mouse.CapturingRight;
        }

        protected override bool GetContextTransitioning() {
            return owner.IsTransitioning;
        }

        private void ButtonClicked(int ID) {
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

        private Rectangle Viewport => owner.Viewport.Bounds;


        private readonly HealthBar playerHealthBar = new(HealthBarAlignment.Left);
        private readonly HealthBar targetHealthBar = new(HealthBarAlignment.Right);

        private readonly Tagline tagline = new();
        private readonly SpeechBox speechBox = new();

        public SpeechBox SpeechBox => speechBox;
        public Tagline Tagline => tagline;
      
        private readonly List<Button> interactableElements = new();

        #region INTERACTION AGENT

        protected override TimeSpan GetCurrentTime() => owner.Now;

        protected override IEnumerable<UIElement> GetElements() => interactableElements;

        #endregion
        #region UPDATE, RENDER, & LAYOUT

        private float _scale;

        public void UpdateLayout(float scale) {
            _scale = scale;
            //todo... verify that scale is being properly applied through the battle renderer
            FloatRectangle viewport = new(Viewport);
            TimeSpan now = Now;

            float margin = scale * Constants.BattleUI.MarginScale;
            float halfMargin = scale;

            UpdateActionButtons(viewport,now,scale,margin,halfMargin);
            UpdateHealthBars(viewport,scale,margin);
            speechBox.Update(now,viewport,scale*Constants.BattleUI.SpeechBoxScale);
            tagline.Update(now,viewport,scale);
        }

        public void UpdateActionButtons(FloatRectangle viewport,TimeSpan now,float scale,float margin,float halfMargin) {
            Rectangle textureSource = Button1.TextureSource;
            float buttonHeight = textureSource.Height * scale * Constants.BattleUI.ButtonScale;
            float buttonWidth = (float)textureSource.Width / textureSource.Height * buttonHeight;

            float buttonCenterY = viewport.Bottom - margin - buttonHeight - halfMargin;

            var buttonRenderData = new ButtonRenderData(
                viewport,buttonWidth,buttonHeight,viewport.Center.X,buttonCenterY,halfMargin
            );

            foreach(var button in actionButtons) {
                button.Update(now,buttonRenderData);
            }
        }

        public void UpdateHealthBars(FloatRectangle viewport,float scale,float margin) {
            float width = viewport.Width * 0.5f - margin * 2;
            float YOffset = scale * Constants.BattleUI.HealthImpactScale;
            playerHealthBar.ScreenArea = new(
                viewport.Left + margin,YOffset * playerHealthBar.GetYOffset() + viewport.Top + margin,width,playerHealthBar.TextureSource.Height * scale
            );
            targetHealthBar.ScreenArea = new(
                viewport.Right - width - margin,YOffset * targetHealthBar.GetYOffset() + viewport.Top + margin,width,targetHealthBar.TextureSource.Height * scale
            );
            var now = Now;
            playerHealthBar.Update(now);
            targetHealthBar.Update(now);
        }

        private void SetHealthBarEffectRange(HealthBar healthBar) {

            (float start,float end) = healthBar.GetOffColorRange();

            FloatRectangle uvArea = healthBar.UVArea;

            start = uvArea.Left + uvArea.Width * start;
            end = uvArea.Left + uvArea.Width * end;

            healthBarColorStartParameter.SetValue(start);
            healthBarColorEndParameter.SetValue(end);
        }

        private void RenderHealthBars(SpriteBatch spriteBatch,UserData playerData,UserData targetData) {
            spriteBatch.Begin(SpriteSortMode.Immediate,null,SamplerState.PointClamp,null,null,healthBarEffect);
            if(playerData != null) {
                playerHealthBar.Value = playerData.HealthFraction;
                SetHealthBarEffectRange(playerHealthBar);
                playerHealthBar.Draw(spriteBatch,playerData.Color);
            }
            if(targetData != null) {
                targetHealthBar.Value = targetData.HealthFraction;
                SetHealthBarEffectRange(targetHealthBar);
                targetHealthBar.Draw(spriteBatch,targetData.Color);
            }
            spriteBatch.End();
        }

        private float GetNameTextScale() {
            return _scale * Constants.BattleUI.NameTextScale;
        }

        private float GetButtonTextScale() {
            return _scale * Constants.BattleUI.ButtonTextScale;
        }

        private float GetTaglineTextScale() {
            return _scale * Constants.BattleUI.TagTextScale;
        }

        private float GetSpeechBoxTextScale() {
            return _scale * Constants.BattleUI.SpeechBoxTextScale;
        }

        private float GetSpeechBoxMarginScale() {
            return _scale * Constants.BattleUI.SpeechBoxMarginScale;
        }

        private void RenderNames(UserData playerData,UserData targetData) {
            float usernameScale = GetNameTextScale();
            Color usernameColor = Color.White;
            if(playerData != null && playerData.Name != null) {
                Fonts.RetroFont.Draw(
                    playerData.Name,
                    new Vector2((int)playerHealthBar.ScreenArea.X,(int)(playerHealthBar.ScreenArea.Bottom + playerHealthBar.ScreenArea.Top)),
                    GetNameTextScale(),usernameColor
                );
            }
            if(targetData != null && targetData.Name != null) {
                Fonts.RetroFont.DrawRight(
                    targetData.Name,
                    new Vector2((int)targetHealthBar.ScreenArea.Right,(int)(targetHealthBar.ScreenArea.Bottom + targetHealthBar.ScreenArea.Top)),
                    usernameScale,usernameColor
                );
            }
        }

        private void RenderActionButtonText() {
            float buttonTextScale = GetButtonTextScale();
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.RetroFont,buttonTextScale,Color.White);
            }
        }

        private void RenderTagline(SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            tagline.Draw(spriteBatch);
            spriteBatch.End();
            Fonts.RetroFont.Begin(spriteBatch);
            tagline.DrawText(Fonts.RetroFont,GetTaglineTextScale());
            Fonts.RetroFont.End();
        }

        public void Render(SpriteBatch spriteBatch,UserData playerData,UserData targetData) {

            Fonts.RetroFont.Begin(spriteBatch);
            RenderNames(playerData,targetData);
            Fonts.RetroFont.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            speechBox.Draw(spriteBatch,targetData.Color);
            spriteBatch.End();

            Fonts.RetroFont.Begin(spriteBatch);
            speechBox.DrawText(Fonts.RetroFont,GetSpeechBoxTextScale(),GetSpeechBoxMarginScale());
            Fonts.RetroFont.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
            RenderHealthBars(spriteBatch,playerData,targetData);

            Fonts.RetroFont.Begin(spriteBatch);
            RenderActionButtonText();
            Fonts.RetroFont.End();

            RenderTagline(spriteBatch);
        }
        #endregion
    }
}
