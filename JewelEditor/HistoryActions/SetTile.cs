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

        private void SetValue(TileGrid grid,int value) {
            grid.GetLayer(Editor.TileLayer)[location.X,location.Y] = value;
        }

        public override void Apply(TileGrid grid) => SetValue(grid,newValue);
        public override void Undo(TileGrid grid) => SetValue(grid,oldValue);
    }
}
