using JewelEditor.Entity;
using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Input;

namespace JewelEditor.InputContext.MouseHandlers {
    internal sealed class TileEditor:MouseHandler {

        public TileEditor(Grid2D grid) : base(grid) { }

        private int GetPaintValue() => (int)Grid.Entities.Get<StateEntity>(Editor.State).TileType;

        private void PaintTile(Point screenPoint) {
            if(!Grid.TryGetTile(screenPoint,out var tile)) {
                return;
            }
            Grid.GetLayer(0)[tile.X,tile.Y] = GetPaintValue();
        }

        public override void MouseDown(Point point) => PaintTile(point);
        public override void MouseMove(Point point) => PaintTile(point);

        public override void MouseUp(Point point) { }
        public override void Scroll(Point point,ScrollDirection direction) { }
    }
}
