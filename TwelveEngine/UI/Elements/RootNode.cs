using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI.Elements {
    internal sealed class RootNode:Element {

        private readonly Func<Point> getSize;
        public RootNode(Func<Point> getSize) {
            this.getSize = getSize;
        }

        public void Update() => SetSize(getSize.Invoke());

        public bool ElementOnSurface(RenderElement element) {
            return element.ScreenArea.Intersects(ComputedArea);
        }
    }
}
