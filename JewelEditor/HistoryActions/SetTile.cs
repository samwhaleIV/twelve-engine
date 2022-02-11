using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Serial;

namespace JewelEditor.HistoryActions {
    internal sealed class SetTile:HistoryAction {

        private Point location;

        private int newValue, oldValue;

        public SetTile(Point location,int newValue,int oldValue) {
            this.location = location;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public override void Export(SerialFrame frame) {
            frame.Set(location);
            frame.Set(newValue);
            frame.Set(oldValue);
        }

        public override HistoryAction Recreate(SerialFrame frame) {
            return new SetTile(frame.GetPoint(),frame.GetInt(),frame.GetInt());
        }

        public override void Apply(Grid2D grid) {
            grid.GetLayer(Editor.TileLayer)[location.X,location.Y] = newValue;
        }

        public override void Undo(Grid2D grid) {
            grid.GetLayer(Editor.TileLayer)[location.X,location.Y] = oldValue;
        }
    }
}
