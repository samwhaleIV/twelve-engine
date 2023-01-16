using Elves.UI.Battle;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using TwelveEngine;

namespace Elves.Battle {
    public abstract class BattleRendererState:WorldBase {

        private string backgroundTexture;

        private void LoadBackgroundTexture() {
            background.Texture = Game.Content.Load<Texture2D>(backgroundTexture);
        }

        public BattleRendererState(string background) {
            backgroundTexture = background;
            OnLoad += LoadBackgroundTexture;
            Initialize();
        }

        public BattleRendererState(Texture2D background) {
            this.background.Texture = background;
            Initialize();
        }

        public BattleRendererState() {
            background.Texture = Textures.Nothing;
            Initialize();
        }

        private BattleUI battleUI;
        public BattleUI UI => battleUI;

        private readonly ScrollingBackground background = ScrollingBackground.GetCheckered();

        public ScrollingBackground Background => background;

        private void BattleRendererState_OnPreRender() {
            background.Render(Game.SpriteBatch,Now,Game.Viewport);
        }

        private void Initialize() {
            OnLoad += BattleScene_OnLoad;
            OnRender += BattleScene_OnRender;
            OnUnload += BattleRendererState_OnUnload;
            OnPreRender += BattleRendererState_OnPreRender;
            Camera.Orthographic = !Debug3D;
        }

        private void BattleScene_OnLoad() {
            InitializeBattleUI();
            background.Load(Game.Content);
        }

        private void BattleRendererState_OnUnload() {
            background.Unload();
        }

        protected void UpdateUI() {
            battleUI.Update((int)GetUIScale());
            Game.CursorState = battleUI.GetCursorState();
        }

        protected abstract UserData GetPlayerData();
        protected abstract UserData GetTargetData();

        protected abstract void ActionButtonClicked(int ID);

        private void InitializeBattleUI() {
            battleUI = new BattleUI(Game);

            Mouse.OnPress += battleUI.MousePress;
            Mouse.OnRelease += battleUI.MouseRelease;
            Mouse.OnMove += battleUI.MouseMoved;

            battleUI.OnActionButtonClick += ActionButtonClicked;
        }

        private void BattleScene_OnRender() {
            battleUI.Render(Game.SpriteBatch,GetPlayerData(),GetTargetData());
        }
    }
}
