using Elves.UI;
using Elves.UI.Battle;

namespace Elves.Battle {
    public abstract class BattleRendererState:OrthoBackgroundState {

        private BattleUI battleUI;
        public BattleUI UI => battleUI;

        public BattleRendererState(string backgroundImage) : base(backgroundImage) {
            OnLoad += BattleScene_OnLoad;
            ScrollingBackground = true;
            OnRender += BattleScene_OnRender;
        }

        protected void UpdateUI() {
            battleUI.Update((int)GetUIScale());
            Game.CursorState = battleUI.GetCursorState();
        }

        private readonly User _player = new User();
        private readonly User _target = new User();

        public User Player => _player;
        public User Target => _target;

        protected abstract void ActionButtonClicked(int ID);

        private void InitializeBattleUI() {
            battleUI = new BattleUI(Game);

            Mouse.OnPress += battleUI.MousePress;
            Mouse.OnRelease += battleUI.MouseRelease;
            Mouse.OnMove += battleUI.MouseMoved;

            battleUI.OnActionButtonClick += ActionButtonClicked;
        }

        private void BattleScene_OnLoad() {
            InitializeBattleUI();
        }

        private void BattleScene_OnRender() {
            battleUI.Render(Game.SpriteBatch,_player,_target);
        }
    }
}
