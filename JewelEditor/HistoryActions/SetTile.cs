using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Serial;

namespace JewelEditor.HistoryActions {
    internal sealed class SetTile:HistoryAction {

        public override HistoryActionType GetActionType() => HistoryActionType.SetTile;

        private Point location;
        private int newValue, oldValue;

        public SetTile(Point location,int newValue,int oldValue) {
            this.location = location;
            this.newValue = newValue;
            this.oldValue = oldValue;
        }

        public SetTile(SerialFrame frame) {
            location = frame.GetPoint();
            newValue = frame.GetInt();
            oldValue = frame.GetInt();
        }

        public override void Export(SerialFrame frame) {
            frame.Set(location);
            frame.Set(newValue);
            frame.Set(oldValue);
        }

        public override void Apply(Grid2D grid) {
            grid.GetLayer(Editor.TileLayer)[location.X,location.Y] = newValue;
        }

        public override void Undo(Grid2D grid) {
            grid.GetLayer(Editor.TileLayer)[location.X,location.Y] = oldValue;
        }
    }
}
