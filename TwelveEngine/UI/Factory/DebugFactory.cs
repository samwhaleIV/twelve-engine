using System;
using Microsoft.Xna.Framework;
using TwelveEngine.UI.Elements;

namespace TwelveEngine.UI.Factory {
    public static class DebugFactory {
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
    }
}
