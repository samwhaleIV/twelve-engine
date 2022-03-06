using Microsoft.Xna.Framework;
using TwelveEngine.UI;
using JewelEditor.InputContext;
using TwelveEngine.UI.Elements;

namespace JewelEditor.Entity {
    internal sealed class QuickActionBar:UIEntity {

        protected override int GetEntityType() => JewelEntities.QuickActionBar;

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

        public override bool Contains(Vector2 location) {
            var point = Owner.GetScreenPoint(location);
            return point.Y < Editor.QuickActionBarHeight;
        }

        protected override void GenerateState(UIState state) {
            int panelHeight = Editor.QuickActionBarHeight;

            Panel panel = new Panel(Color.FromNonPremultiplied(255,255,255,127)) {
                Sizing = Sizing.PercentX,
                Width = 100,
                Height = panelHeight
            };

            ButtonData[] buttons = Editor.Buttons;

            var buttonGroup = new UIElement() {
                Sizing = Sizing.Normal,
                Height = panel.Height,
                Width = buttons.Length * panelHeight,
                Positioning = Positioning.CenterParentX
            };

            for(int i = 0;i<buttons.Length;i++) {
                var parent = new UIElement() {
                    Width = panelHeight,
                    Height = panelHeight,
                    X = i * panelHeight,
                    Padding = 8
                };
                ButtonData button = buttons[i];

                ContextButton contextButton = new ContextButton(this,button) { Sizing = Sizing.Fill };

                parent.AddChild(contextButton);
                buttonGroup.AddChild(parent);
            }

            panel.AddChild(buttonGroup);
            state.AddChild(panel);
        }
    }
}
