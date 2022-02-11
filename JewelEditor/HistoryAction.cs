using TwelveEngine.Game2D;
using TwelveEngine.Serial;

namespace JewelEditor {
    internal abstract class HistoryAction {
        public abstract void Apply(Grid2D grid);
        public abstract void Undo(Grid2D grid);

        public abstract void Export(SerialFrame frame);
        public abstract HistoryAction Recreate(SerialFrame frame);
    }
}
