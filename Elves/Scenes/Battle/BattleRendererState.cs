using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using Elves.Scenes.Battle.UI;

namespace Elves.Scenes.Battle {
    public abstract class BattleRendererState:Scene {

        private readonly string backgroundTexture;

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
            background.Texture = Program.Textures.Nothing;
            Initialize();
        }

        private BattleUI battleUI;
        public BattleUI UI => battleUI;

        private readonly ScrollingBackground background = ScrollingBackground.GetCheckered();

        public ScrollingBackground Background => background;

        private void BattleRendererState_OnPreRender() {
            background.Update(Now);
            background.Render(Game.SpriteBatch,Game.Viewport);
        }

        private void Initialize() {
            OnLoad += BattleScene_OnLoad;
            OnRender += BattleScene_OnRender;
            OnPreRender += BattleRendererState_OnPreRender;
            Camera.Orthographic = !Debug;
        }

        private void BattleScene_OnLoad() {
            InitializeBattleUI();
            background.Load(Game.Content);
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

            Mouse.OnPress +=Mouse_OnPress;
            Mouse.OnRelease +=Mouse_OnRelease;
            Mouse.OnMove +=Mouse_OnMove;

            battleUI.OnActionButtonClick += ActionButtonClicked;
        }

        private void Mouse_OnMove() {
            battleUI.MouseMoved(Mouse.Position);
        }

        private void Mouse_OnRelease() {
            battleUI.MouseRelease(Mouse.Position);
        }

        private void Mouse_OnPress() {
            battleUI.MousePress(Mouse.Position);
        }

        private void BattleScene_OnRender() {
            battleUI.Render(Game.SpriteBatch,GetPlayerData(),GetTargetData());
        }
    }
}
