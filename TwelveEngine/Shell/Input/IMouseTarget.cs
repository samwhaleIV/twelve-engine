using Microsoft.Xna.Framework;

namespace TwelveEngine.Shell.Input {
    public interface IMouseTarget {
        public void MouseUp(Point point);
        public void MouseDown(Point point);
        public void MouseMove(Point point);
        public void Scroll(Point point,ScrollDirection direction);
    }
}
