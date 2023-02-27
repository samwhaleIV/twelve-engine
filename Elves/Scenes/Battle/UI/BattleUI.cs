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

        public MiniGameScreen MiniGameScreen { get; private init; } = new() {
            Width = Constants.Battle.MiniGameWidth,
            Height = Constants.Battle.MiniGameHeight,
            Texture = Program.Textures.MiniGameTablet
        };

        public bool GetCanPress() {
            return !MiniGameScreen.IsActive;
        }

        public BattleUI(InputGameState owner) {
            this.owner = owner;

            foreach(var button in actionButtons) {
                _interactableElements.Add(button);
                button.OnActivated += ButtonClicked;
                button.CanPress = GetCanPress;
            }

            /* I hate this but I also love it at the same time. */
            (Button1,Button2,Button3,Button4) = (actionButtons[0],actionButtons[1],actionButtons[2],actionButtons[3]);

            DefaultFocusElement = Button1;

            LoadHealthBarEffect();

            owner.OnInputActivated += FocusDefault;

            MiniGameScreen.Load(owner.GraphicsDevice);
            owner.OnUnload.Add(MiniGameScreen.Unload);
            owner.OnPreRender.Add(PreRenderMiniGame,EventPriority.First);
        }

        private void PreRenderMiniGame() {
            MiniGameScreen.PreRender(owner.GraphicsDevice,owner.SpriteBatch,owner.RenderTarget);
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


        private readonly HealthBar playerHealthBar = new(HealthBarAlignment.Left), targetHealthBar = new(HealthBarAlignment.Right);

        public SpeechBox SpeechBox { get; private init; } = new();
        public Tagline Tagline { get; private init; } = new();
      
        private readonly List<Button> _interactableElements = new();

        #region INTERACTION AGENT

        protected override TimeSpan GetCurrentTime() => owner.Now;

        protected override IEnumerable<UIElement> GetElements() => _interactableElements;

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
            SpeechBox.Update(now,viewport,scale*Constants.BattleUI.SpeechBoxScale);
            Tagline.Update(now,viewport,scale);

            MiniGameScreen.UpdateLayout(now,viewport,scale * Constants.BattleUI.MiniGameScale);
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

        private void RenderNames(UserData playerData,UserData targetData) {
            float usernameScale = _scale * Constants.BattleUI.NameTextScale;
            Color usernameColor = Color.White;
            if(playerData != null && playerData.Name != null) {
                Fonts.Retro.Draw(
                    playerData.Name,
                    new Vector2((int)playerHealthBar.ScreenArea.X,(int)(playerHealthBar.ScreenArea.Bottom + playerHealthBar.ScreenArea.Top)),
                    usernameScale,usernameColor
                );
            }
            if(targetData != null && targetData.Name != null) {
                Fonts.Retro.DrawRight(
                    targetData.Name,
                    new Vector2((int)targetHealthBar.ScreenArea.Right,(int)(targetHealthBar.ScreenArea.Bottom + targetHealthBar.ScreenArea.Top)),
                    usernameScale,usernameColor
                );
            }
        }

        private void RenderActionButtonText() {
            float buttonTextScale = _scale * Constants.BattleUI.ButtonTextScale;
            foreach(var button in actionButtons) {
                button.DrawText(Fonts.Retro,buttonTextScale,Color.White);
            }
        }

        private void RenderTagline(SpriteBatch spriteBatch) {
            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            Tagline.Draw(spriteBatch);
            spriteBatch.End();
            Fonts.Retro.Begin(spriteBatch);
            float tagTextScale = _scale * Constants.BattleUI.TagTextScale;
            Tagline.DrawText(Fonts.Retro,tagTextScale);
            Fonts.Retro.End();
        }

        public void Render(SpriteBatch spriteBatch,UserData playerData,UserData targetData) {

            Fonts.Retro.Begin(spriteBatch);
            RenderNames(playerData,targetData);
            Fonts.Retro.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            SpeechBox.Draw(spriteBatch,targetData.Color);
            spriteBatch.End();

            Fonts.Retro.Begin(spriteBatch);
            float speechBoxMargin = _scale * Constants.BattleUI.SpeechBoxMarginScale;
            SpeechBox.DrawText(Fonts.Retro,_scale * Constants.BattleUI.SpeechBoxTextScale,speechBoxMargin);
            Fonts.Retro.End();

            spriteBatch.Begin(SpriteSortMode.Deferred,null,SamplerState.PointClamp);
            foreach(var button in actionButtons) {
                button.Draw(spriteBatch);
            }
            spriteBatch.End();
            RenderHealthBars(spriteBatch,playerData,targetData);

            Fonts.Retro.Begin(spriteBatch);
            RenderActionButtonText();
            Fonts.Retro.End();

            MiniGameScreen.Render(spriteBatch);

            RenderTagline(spriteBatch);
        }
        #endregion
    }
}
