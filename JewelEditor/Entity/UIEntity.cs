using TwelveEngine.Shell.Input;
using Microsoft.Xna.Framework;
using TwelveEngine.UI;
using JewelEditor.InputContext;

namespace JewelEditor.Entity {
    internal sealed class UIEntity:JewelEntity {

        protected override int GetEntityType() => JewelEntities.UIEntity;

        public UIEntity() {
            OnLoad += UIEntity_OnLoad;
            OnUnload += UIEntity_OnUnload;
        }

        public void DropFocus() => UIState.DropFocus();
        public UIState UIState { get; private set; }
        public IMouseTarget MouseTarget => UIState.MouseTarget;

        public bool TryGetInputMode(int index,out (InputMode Mode, TileType? Type) mode) {
            var buttonSources = Editor.Buttons;
            if(index < 0 || index >= buttonSources.Length) {
                mode = (InputMode.Tile, null);
                return false;
            }
           (Rectangle _,InputMode Mode,TileType? Type) = buttonSources[index];
            mode = (Mode, Type);
            return true;
        }

        private void UIEntity_OnLoad() {
            UIState = new UIState(Game);

            SelectorUI.Generate(this);

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
            return point.Y < Editor.ButtonsPanelHeight;
        }
    }
}
