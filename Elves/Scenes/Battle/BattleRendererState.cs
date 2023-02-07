using Microsoft.Xna.Framework.Graphics;
using Elves.Scenes.Battle.UI;
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
            OnLoad.Add(LoadBackgroundTexture);
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

        private readonly ScrollingBackground background = ScrollingBackground.GetCheckered(
            scrollTime: Constants.AnimationTiming.ScrollingBackgroundDefault
        );

        public ScrollingBackground Background => background;

        private void PreRender() {
            background.Update(Now);
            background.Render(SpriteBatch,Viewport);
        }

        private void Initialize() {
            UIScaleModifier = Constants.UI.BattleSceneScaleModifier;
            OnLoad.Add(Load);
            OnUpdate.Add(UpdateUI);
            OnRender.Add(Render);
            OnPreRender.Add(PreRender);
            Camera.Orthographic = true;
        }

        private void Load() {
            InitializeBattleUI();
            background.Load(Content);
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
