using JewelEditor.Entity;
using JewelEditor.HistoryActions;
using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;

namespace JewelEditor.InputContext.MouseHandlers {
    internal sealed class TileEditor:MouseHandler {

        public TileEditor(Grid2D grid) : base(grid) { }

        private int GetPaintValue() {
            return (int)Grid.Entities.Get<StateEntity>(Editor.State).TileType;
        }

        private HistoryEventToken eventToken;

        private void PaintTile(Point screenPoint,StateEntity state) {
            if(!Grid.TryGetTile(screenPoint,out var tile)) {
                return;
            }
            var layer = Grid.GetLayer(0);

            var newValue = GetPaintValue();
            var oldValue = layer[tile.X,tile.Y];

            if(newValue == oldValue) {
                return;
            }

            state.AddEventAction(eventToken,new SetTile(tile,newValue,oldValue), applyAction: true);
        }

        public override void MouseDown(Point point) {
            var state = GetState();
            eventToken = state.StartHistoryEvent();
            PaintTile(point,state);
        }
        public override void MouseMove(Point point) {
            PaintTile(point,GetState());
        }

        public override void MouseUp(Point point) {
            GetState().EndHistoryEvent(eventToken);
        }
    }
}
