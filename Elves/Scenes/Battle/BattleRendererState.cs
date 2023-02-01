﻿using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using Elves.Scenes.Battle.UI;
using TwelveEngine.UI;
using TwelveEngine.Effects;
using TwelveEngine.Shell;
using Elves.Battle;

namespace Elves.Scenes.Battle {
    public abstract class BattleRendererScene:Scene3D {

        private readonly string backgroundTexture;

        private void LoadBackgroundTexture() {
            background.Texture = Content.Load<Texture2D>(backgroundTexture);
        }

        public BattleRendererScene(string background) {
            backgroundTexture = background;
            OnLoad += LoadBackgroundTexture;
            Initialize();
        }

        public BattleRendererScene(Texture2D background) {
            this.background.Texture = background;
            Initialize();
        }

        public BattleRendererScene() {
            background.Texture = Program.Textures.Nothing;
            Initialize();
        }

        private BattleUI battleUI;
        public BattleUI UI => battleUI;

        private readonly ScrollingBackground background = ScrollingBackground.GetCheckered();

        public ScrollingBackground Background => background;

        private void BattleRendererState_OnPreRender() {
            background.Update(Now);
            background.Render(SpriteBatch,Viewport);
        }

        private void Initialize() {
            OnLoad += BattleScene_OnLoad;
            OnRender += BattleScene_OnRender;
            OnPreRender += BattleRendererState_OnPreRender;
            Camera.Orthographic = !Debug;
        }

        private void BattleScene_OnLoad() {
            InitializeBattleUI();
            background.Load(Content);
        }

        protected void UpdateUI() {
            battleUI.UpdateLayout((int)GetUIScale());
            CustomCursor.State = battleUI.CursorState;
        }

        protected abstract UserData GetPlayerData();
        protected abstract UserData GetTargetData();

        protected abstract void ActionButtonClicked(int ID);

        private void InitializeBattleUI() {
            battleUI = new BattleUI(this);
            battleUI.OnActionButtonClick += ActionButtonClicked;
            battleUI.BindInputEvents(this);
        }

        private void BattleScene_OnRender() {
            battleUI.Render(SpriteBatch,GetPlayerData(),GetTargetData());
        }

        protected override void UpdateGame() {
            int uiScale = (int)GetUIScale();
            battleUI.UpdateLayout(uiScale);
            UpdateInputDevices();
            UI.SendEvent(InputEvent.CreateMouseUpdate(Mouse.Position));
            battleUI.UpdateLayout(uiScale); /* Interaction can be delayed by 1 frame if we don't update the UI again */
            CustomCursor.State = UI.CursorState;
            UpdateEntities();
            UpdateCamera();
        }
    }
}
