using Microsoft.Xna.Framework;
using TwelveEngine.Game2D;
using TwelveEngine.Serial;

namespace JewelEditor.HistoryActions {
    internal class MoveEntity:HistoryAction {

        public override HistoryActionType GetActionType() => HistoryActionType.MoveEntity;

        private string name;
        private Vector2 newLocation, oldLocation;

        public MoveEntity(SerialFrame frame) {
            name = frame.GetString();
            newLocation = frame.GetVector2();
            oldLocation = frame.GetVector2();
        }

        public MoveEntity(string name,Vector2 newLocation,Vector2 oldLocation) {
            this.name = name;
            this.newLocation = newLocation;
            this.oldLocation = oldLocation;
        }

        private void SetLocation(Grid2D grid,Vector2 location) {
            grid.Entities.Get(name).Position = location;
        }

        public override void Apply(Grid2D grid) => SetLocation(grid,newLocation);
        public override void Undo(Grid2D grid) => SetLocation(grid,oldLocation);

        public override void Export(SerialFrame frame) {
            frame.Set(name);
            frame.Set(newLocation);
            frame.Set(oldLocation);
        }
    }
}
