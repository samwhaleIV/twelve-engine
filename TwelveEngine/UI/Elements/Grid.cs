namespace TwelveEngine.UI.Elements {
    public class Grid:Element {

        private (int Width, int Height) cellSize = (0,0);
        internal (int Width,int Height) CellSize => cellSize;

        private void updateCellSize() => cellSize = getCellSize();
        public Grid() => LayoutUpdated += updateCellSize;

        private int rows;
        private int columns;

        public int Rows {
            get => rows;
            set {
                if(rows == value) {
                    return;
                }
                rows = value;
                UpdateLayout();
            }
        }
        public int Columns {
            get => columns;
            set {
                if(columns == value) {
                    return;
                }
                columns = value;
                UpdateLayout();
            }
        }

        private (int Width,int Height) getCellSize() {
            /* TODO: Be corrected to pixel perfection */
            int width = (int)(Width / (float)columns); 
            int height = (int)(Height / (float)rows);
            return (width, height);
        }
    }
}
