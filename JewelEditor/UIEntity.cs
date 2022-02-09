using TwelveEngine.Game2D;
using TwelveEngine.Input;
using Microsoft.Xna.Framework;
using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using System;

namespace JewelEditor {
    internal sealed class UIEntity:Entity2D {

        protected override int GetEntityType() => JewelEntities.UIEntity;

        private const int PanelHeight = 96;

        public UIEntity() {
            OnLoad += UIEntity_OnLoad;
            OnUnload += UIEntity_OnUnload;
        }

        private UIState uiState;

        public IMouseTarget MouseTarget => uiState.MouseTarget;

        private sealed class PanelButton:Panel {
            public PanelButton(Color color):base(color) {
                OnMouseDown += point => OnClick(this);
                Padding = 4;
                IsInteractable = true;
            }
            public event Action<PanelButton> OnClick;
        }

        private static void GenerateState(UIState state) {
            var panel = new Panel(Color.FromNonPremultiplied(255,255,255,127)) {
                Sizing = Sizing.PercentX,
                Width = 100,
                Height = PanelHeight
            };
            state.AddChild(panel);

            
        }


        private void UIEntity_OnLoad() {
            uiState = new UIState(Game);
            GenerateState(uiState);
            Owner.OnUpdate += uiState.Update;
            Owner.OnRender += uiState.Render;
            Owner.OnPreRender += uiState.PreRender;
            uiState.Load();
            uiState.StartLayout();
        }

        private void UIEntity_OnUnload() {
            Owner.OnUpdate -= uiState.Update;
            Owner.OnRender -= uiState.Render;
            Owner.OnPreRender -= uiState.PreRender;
            uiState.Unload();
            uiState = null;
        }

        public override bool Contains(Vector2 location) {
            var point = Owner.GetScreenPoint(location);
            return point.Y < PanelHeight;
        }
    }
}
