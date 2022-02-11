using JewelEditor.HistoryActions;
using TwelveEngine.Game2D;
using TwelveEngine.Serial;

namespace JewelEditor {

    public enum HistoryActionType:int {
        CreateEntity, DeleteEntity, EditEntity, SetTile
    }

    internal abstract class HistoryAction {
        public abstract void Apply(Grid2D grid);
        public abstract void Undo(Grid2D grid);

        public abstract void Export(SerialFrame frame);

        public abstract HistoryActionType GetActionType();

        public static HistoryAction Create(SerialFrame frame,HistoryActionType type) {
            switch(type) {
                default:
                    return null;
                case HistoryActionType.CreateEntity:
                    return new CreateEntity();
                case HistoryActionType.DeleteEntity:
                    return new DeleteEntity();
                case HistoryActionType.EditEntity:
                    return null;
                case HistoryActionType.SetTile:
                    return new SetTile(frame);
            }
        }
    }
}
