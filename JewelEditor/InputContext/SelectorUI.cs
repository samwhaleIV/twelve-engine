using TwelveEngine.UI;
using TwelveEngine.UI.Elements;
using Microsoft.Xna.Framework;
using JewelEditor.Entity;

namespace JewelEditor.InputContext {
    internal static class SelectorUI {

        private static Color GetBackgroundColor() => Color.FromNonPremultiplied(255,255,255,127);

        public static void Generate(UIEntity owner) {
            int panelHeight = Editor.ButtonsPanelHeight;

            Panel panel = new Panel(GetBackgroundColor()) {
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

                ContextButton contextButton = new ContextButton(owner,button) { Sizing = Sizing.Fill };

                parent.AddChild(contextButton);
                buttonGroup.AddChild(parent);
            }

            owner.UIState.AddChild(panel);
            panel.AddChild(buttonGroup);
        }

    }
}
