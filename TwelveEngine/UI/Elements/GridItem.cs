namespace TwelveEngine.UI.Elements {
    internal sealed class GridItem:Element {

        private readonly Grid grid;

        internal GridItem(Grid grid) {
            Parent = grid; this.grid = grid;

            positioning = Positioning.Relative;
            sizing = Sizing.Fill;
        }

        protected override (int X, int Y) GetRelativePosition() {
            var cellSize = grid.CellSize;
            var x = grid.ScreenX + this.x * cellSize.Width;
            var y = grid.ScreenY + this.y * cellSize.Height;
            return (x + paddingLeft, y + paddingTop);
        }
        protected override (int Width,int Height) GetFillSize() {
            var cellSize = grid.CellSize;
            var width = this.width / grid.Columns * cellSize.Width;
            var height = this.height / grid.Rows * cellSize.Height;
            return (width - paddingRight - paddingLeft, height - paddingBottom - paddingTop);
        }

        public override void AddChild(Element child) {
            AddChild(child,updateLayout: false);
            var startPaused = child.LayoutUpdatesEnabled;
            if(!startPaused) {
                child.PauseLayout();
            }
            child.Positioning = Positioning.Relative;
            child.Sizing = Sizing.Fill;
            if(!startPaused) {
                child.StartLayout();
            }
            UpdateLayout();
        }
    }
}
