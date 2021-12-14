using Microsoft.Xna.Framework;

namespace TwelveEngine.UI.Elements {
    internal sealed class GridItem:Element {

        private readonly Grid grid;

        internal GridItem(Grid grid) {
            Parent = grid; this.grid = grid;

            sizing = Sizing.Fill;
        }

        protected override Point GetPosition() {
            var cellSize = grid.CellSize;
            var x = X * cellSize.Width;
            var y = Y * cellSize.Height;
            return new Point(x,y);
        }

        protected override Point GetFillSize() {
            var cellSize = grid.CellSize;
            var width = Width / grid.Columns * cellSize.Width;
            var height = Height / grid.Rows * cellSize.Height;
            return new Point(width,height);
        }

        public override void AddChild(Element child) {
            AddChild(child,updateLayout: false);
            var startPaused = child.LayoutUpdatesEnabled;
            if(!startPaused) {
                child.PauseLayout();
            }
            child.Sizing = Sizing.Fill;
            if(!startPaused) {
                child.StartLayout();
            }
            UpdateLayout();
        }
    }
}
