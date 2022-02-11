using System.Collections.Generic;
using TwelveEngine.Serial;

namespace JewelEditor.Entity {
    internal sealed class StateEntity:JewelEntity {

        protected override int GetEntityType() => JewelEntities.StateEntity;

        public InputMode InputMode { get; set; } = InputMode.Tile;
        public TileType TileType { get; set; } = TileType.Floor;

        private readonly Stack<HistoryAction[]> UndoStack = new Stack<HistoryAction[]>();
        private readonly Queue<HistoryAction[]> RedoStack = new Queue<HistoryAction[]>();
        private readonly Queue<HistoryAction> ActionBuffer = new Queue<HistoryAction>();

        private bool IsWritingEvent { get; set; } = false;

        public void StartHistoryEvent() {
            if(IsWritingEvent) {
                return;
            }
            IsWritingEvent = true;
        }

        public void EndHistoryEvent() {
            if(!IsWritingEvent) {
                return;
            }
            //Write the action queue to an array, put the array on the undo stack, and clear the redo queue
            IsWritingEvent = false;
        }

        public void AddEventAction(HistoryAction historyAction,bool applyAction = true) {
            if(!IsWritingEvent) {
                return;
            }
            ActionBuffer.Enqueue(historyAction);
            if(!applyAction) {
                return;
            }
            historyAction.Apply(Owner);
        }

        public void Undo() {
            if(IsWritingEvent) {
                return;
            }
            //if the undo stack isn't empty, take an item, apply the actions in reverse order, and put it in the redo queue
        }

        public void Redo() {
            if(IsWritingEvent) {
                return;
            }
            //if the redo queue isn't empty, take an item, apply the actions in forward order, and place the item into the undo stack
        }

        private void ImportHistoryStacks(SerialFrame frame) {
            //todo
        }
        private void ExportHistoryStacks(SerialFrame frame) {
            //todo
        }

        public StateEntity() {
            OnImport += StateEntity_OnImport;
            OnExport += StateEntity_OnExport;
        }

        private void StateEntity_OnExport(SerialFrame frame) {
            frame.Set((int)InputMode);
            frame.Set((int)TileType);
            ExportHistoryStacks(frame);
        }

        private void StateEntity_OnImport(SerialFrame frame) {
            InputMode = (InputMode)frame.GetInt();
            TileType = (TileType)frame.GetInt();
            ImportHistoryStacks(frame);
        }
    }
}
