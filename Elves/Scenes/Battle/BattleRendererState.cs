using Microsoft.Xna.Framework.Graphics;
using TwelveEngine;
using Elves.Scenes.Battle.UI;
using TwelveEngine.Input;
using TwelveEngine.UI.Interaction;

namespace Elves.Scenes.Battle {
    public abstract class BattleRendererState:Scene3D {

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
            battleUI.UpdateLayout((int)GetUIScale());
            Game.CursorState = battleUI.CursorState;
        }

        protected abstract UserData GetPlayerData();
        protected abstract UserData GetTargetData();

        protected abstract void ActionButtonClicked(int ID);

        private void InitializeBattleUI() {
            battleUI = new BattleUI(Game);
            battleUI.OnActionButtonClick += ActionButtonClicked;

            Input.OnAcceptDown += Input_OnAcceptDown;
            Input.OnCancelDown += Input_OnCancelDown;
            Mouse.OnPress += Mouse_OnPress;
            Input.OnDirectionDown += Input_OnDirectionDown;
            Input.OnAcceptUp += Input_OnAcceptUp;
            Mouse.OnRelease += Mouse_OnRelease;
        }

        private void Mouse_OnRelease() => UI?.SendEvent(InputEvent.MouseReleased);
        private void Input_OnAcceptUp() => UI?.SendEvent(InputEvent.AcceptReleased);
        private void Input_OnDirectionDown(Direction direction) => UI?.SendEvent(InputEvent.CreateDirectionImpulse(direction));
        private void Mouse_OnPress() => UI?.SendEvent(InputEvent.MousePressed);
        private void Input_OnCancelDown() => UI?.SendEvent(InputEvent.BackButtonActivated);
        private void Input_OnAcceptDown() => UI?.SendEvent(InputEvent.AcceptPressed);

        private void BattleScene_OnRender() {
            battleUI.Render(Game.SpriteBatch,GetPlayerData(),GetTargetData());
        }

        protected override void UpdateGame() {
            int uiScale = (int)GetUIScale();
            battleUI.UpdateLayout(uiScale);
            UpdateInputDevices();
            UI.SendEvent(InputEvent.CreateMouseUpdate(Mouse.Position));
            battleUI.UpdateLayout(uiScale); /* Interaction can be delayed by 1 frame if we don't update the UI again */
            Game.CursorState = UI.CursorState;
            UpdateEntities();
            UpdateCamera();
        }
    }
}
