using JewelEditor.Entity;
using JewelEditor.HistoryActions;
using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace JewelEditor.InputContext.MouseHandlers {
    internal sealed class TileEditor:MouseHandler {

        public TileEditor(Grid2D grid) : base(grid) { }

        private int GetPaintValue() {
            return (int)Grid.Entities.Get<StateEntity>(Editor.State).TileType;
        }

        private void PaintTile(Point screenPoint,StateEntity state) {
            if(!Grid.TryGetTile(screenPoint,out var tile)) {
                return;
            }
            var layer = Grid.GetLayer(0);

            var newValue = GetPaintValue();
            var oldValue = layer[tile.X,tile.Y];

            state.AddEventAction(new SetTile(tile,newValue,oldValue), applyAction: true);
        }

        public override void MouseDown(Point point) {
            var state = State;
            state.StartHistoryEvent();
            PaintTile(point,state);
        }
        public override void MouseMove(Point point) {
            PaintTile(point,State);
        }

        public override void MouseUp(Point point) {
            State.EndHistoryEvent();
        }
    }
}
