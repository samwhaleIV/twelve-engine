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
            var x = grid.X + this.x * cellSize.Width;
            var y = grid.Y + this.y * cellSize.Height;
            return (x + leftPadding, y + topPadding);
        }
        protected override (int Width,int Height) GetFillSize() {
            var cellSize = grid.CellSize;
            var width = this.width / grid.Columns * cellSize.Width;
            var height = this.height / grid.Rows * cellSize.Height;
            return (width - rightPadding, height - bottomPadding);
        }
    }
}
