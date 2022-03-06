using TwelveEngine.Shell.Input;
using Microsoft.Xna.Framework;
using TwelveEngine.UI;

namespace JewelEditor.Entity {
    internal abstract class UIEntity:JewelEntity {

        public UIEntity() {
            OnLoad += UIEntity_OnLoad;
            OnUnload += UIEntity_OnUnload;
        }

        private UIState UIState { get; set; } = null;

        public IMouseTarget MouseTarget => UIState.MouseTarget;
        public void DropFocus() => UIState.DropFocus();

        protected abstract void GenerateState(UIState state);

        private void UIEntity_OnLoad() {
            var state = new UIState(Game);
            GenerateState(state);

            Owner.OnUpdate += state.Update;
            Owner.OnRender += state.Render;
            Owner.OnPreRender += state.PreRender;

            state.Load();
            state.StartLayout();
            UIState = state;
        }

        private void UIEntity_OnUnload() {
            Owner.OnUpdate -= UIState.Update;
            Owner.OnRender -= UIState.Render;
            Owner.OnPreRender -= UIState.PreRender;
            UIState.Unload();
            UIState = null;
        }
    }
}
