using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Shell;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI.Factory {
    public static class Debug {
        public static (Panel panel,RenderFrame frame) GetColoredFrame(UIState UI,Color color,Action<RenderFrame> configFrame) {
            var frame = new RenderFrame(UI);
            configFrame?.Invoke(frame);
            var panel = new Panel(color) {
                Sizing = Sizing.Fill
            };
            frame.AddChild(panel);
            return (panel, frame);
        }
        public static (Panel panel,ScrollBox frame) GetColoredScrollBox(UIState UI,Color color,Action<ScrollBox> configScrollBox) {
            var scrollBox = new ScrollBox(UI);
            configScrollBox?.Invoke(scrollBox);
            var panel = new Panel(color) {
                Height = 600,
                Width = 100,
                Sizing = Sizing.PercentX
            };
            scrollBox.AddChild(panel);
            return (panel, scrollBox);
        }
        public static GameState GetNestedFrameTest() {
            return UIGameState.Create(UI => {
                var panel1 = GetColoredScrollBox(UI,Color.Red,frame => {
                    frame.Width = 200;
                    frame.Height = 200;
                    frame.Positioning = Positioning.CenterParent;
                });

                var panel2 = GetColoredFrame(UI,Color.Green,frame => {
                    frame.Width = 150;
                    frame.Height = 150;
                    frame.X = 25;
                    frame.Y = 25;
                });

                var panel3 = GetColoredFrame(UI,Color.Blue,frame => {
                    frame.Width = 100;
                    frame.Height = 100;
                    frame.X = 25;
                    frame.Y = 25;
                });

                panel1.panel.IsInteractable = true;
                panel2.panel.IsInteractable = true;
                panel3.panel.IsInteractable = true;

                UI.AddChild(panel1.frame);

                panel1.panel.AddChild(panel2.frame);
                panel2.panel.AddChild(panel3.frame);
            });
        }
    }
}
