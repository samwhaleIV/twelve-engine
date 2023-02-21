using Elves.Scenes.Battle.UI;
using TwelveEngine.Effects;
using TwelveEngine.Shell;
using Elves.Battle;

namespace Elves.Scenes.Battle {
    public abstract class BattleRendererScene:Scene3D {

        public BattleRendererScene() {
            Initialize();
        }

        private BattleUI battleUI;
        public BattleUI UI => battleUI;

        public ScrollingBackground Background { get; init; }

        private void RenderBackground() {
            if(Background is null) {
                return;
            }
            Background.Update(Now);
            Background.Render(SpriteBatch,Viewport);
        }

        private void Initialize() {
            UIScaleModifier = Constants.UI.BattleSceneScaleModifier;
            OnLoad.Add(Load);
            OnUpdate.Add(UpdateUI);
            OnRender.Add(Render);
            OnPreRender.Add(RenderBackground);
            Camera.Orthographic = true;
        }

        private void Load() {
            InitializeBattleUI();
            Background?.Load(Content);
        }

        private void UpdateUI() {
            battleUI.UpdateLayout(UIScale);
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

        private void Render() {
            battleUI.Render(SpriteBatch,GetPlayerData(),GetTargetData());
        }
    }
}
