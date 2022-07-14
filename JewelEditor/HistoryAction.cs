using JewelEditor.HistoryActions;
using TwelveEngine.Game2D;
using TwelveEngine.Serial;

namespace JewelEditor {

    public enum HistoryActionType:int {
        CreateEntity, DeleteEntity, MoveEntity, SetTile
    }

    internal abstract class HistoryAction {

        public static HistoryAction Create(SerialFrame frame,HistoryActionType type) {
            switch(type) {
                default:
                    return null;
                case HistoryActionType.CreateEntity:
                    return new CreateEntity();
                case HistoryActionType.DeleteEntity:
                    return new DeleteEntity();
                case HistoryActionType.MoveEntity:
                    return new MoveEntity(frame);
                case HistoryActionType.SetTile:
                    return new SetTile(frame);
            }
        }

        public abstract void Apply(TileGrid grid);
        public abstract void Undo(TileGrid grid);

        public abstract void Export(SerialFrame frame);

        public abstract HistoryActionType GetActionType();

    }
}
