using TwelveEngine.Game2D;
using TwelveEngine.Input;
using Microsoft.Xna.Framework;
using TwelveEngine.UI;

namespace JewelEditor.InputContext {
    internal sealed class UIEntity:Entity2D {

        protected override int GetEntityType() => JewelEntities.UIEntity;

        private const int PanelHeight = 96;

        public UIEntity() {
            OnLoad += UIEntity_OnLoad;
            OnUnload += UIEntity_OnUnload;
        }

        public void DropFocus() => UIState.DropFocus();
        public UIState UIState { get; private set; }
        public IMouseTarget MouseTarget => UIState.MouseTarget;

        public InputMode InputMode { get; set; } = InputMode.Pointer;
        private SelectorUI selector;

        private void UIEntity_OnLoad() {
            UIState = new UIState(Game);

            selector = new SelectorUI(this);
            selector.Generate(PanelHeight);

            Owner.OnUpdate += UIState.Update;
            Owner.OnRender += UIState.Render;
            Owner.OnPreRender += UIState.PreRender;
            UIState.Load();
            UIState.StartLayout();
        }

        private void UIEntity_OnUnload() {
            Owner.OnUpdate -= UIState.Update;
            Owner.OnRender -= UIState.Render;
            Owner.OnPreRender -= UIState.PreRender;
            UIState.Unload();
            UIState = null;
        }

        public override bool Contains(Vector2 location) {
            var point = Owner.GetScreenPoint(location);
            return point.Y < PanelHeight;
        }
    }
}
