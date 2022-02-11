using System.Collections.Generic;
using TwelveEngine.Serial;

namespace JewelEditor.Entity {
    internal sealed class StateEntity:JewelEntity {

        protected override int GetEntityType() => JewelEntities.StateEntity;

        public InputMode InputMode { get; set; } = InputMode.Tile;
        public TileType TileType { get; set; } = TileType.Floor;

        private readonly Stack<HistoryAction[]> UndoStack = new Stack<HistoryAction[]>();
        private readonly Stack<HistoryAction[]> RedoStack = new Stack<HistoryAction[]>();

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

            // Write the action queue to an array, clear the action queue, put the array on the undo stack, and clear the redo stack
            if(ActionBuffer.Count <= 0) {
                return;
            }
            var historyActions = ActionBuffer.ToArray();
            ActionBuffer.Clear();

            UndoStack.Push(historyActions);

            RedoStack.Clear();
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
            // If the undo stack isn't empty, take an item, apply the actions in reverse order, and put it in the redo stack
            if(!UndoStack.TryPop(out var historyAction)) {
                return;
            }
            for(int i = historyAction.Length - 1;i >= 0;i--) {
                var action = historyAction[i];
                action.Undo(Owner);
            }
            RedoStack.Push(historyAction);
        }

        public void Redo() {
            if(IsWritingEvent) {
                return;
            }
            // If the redo stack isn't empty, take an item, apply the actions in forward order, and place the item into the undo stack
            if(!RedoStack.TryPop(out var historyAction)) {
                return;
            }
            for(int i = 0;i<historyAction.Length;i++) {
                var action = historyAction[i];
                action.Apply(Owner);
            }
            UndoStack.Push(historyAction);
        }

        private HistoryAction[] ImportHistoryActions(SerialFrame frame) {
            var size = frame.GetInt();
            var historyActions = new HistoryAction[size];
            for(int i = 0;i<size;i++) {
                var actionType = (HistoryActionType)frame.GetInt();
                HistoryAction action = HistoryAction.Create(frame,actionType);
                historyActions[i] = action;
            }
            return historyActions;
        }

        private void ExportHistoryActions(SerialFrame frame,HistoryAction[] actions) {
            var size = actions.Length;
            frame.Set(size);
            for(int i = 0;i<size;i++) {
                HistoryAction action = actions[i];
                frame.Set((int)action.GetActionType());
                actions[i].Export(frame);
            }
        }

        private void ImportHistoryStack(SerialFrame frame,Stack<HistoryAction[]> stack) {
            stack.Clear();
            var stackSize = frame.GetInt();
            for(int i = 0;i<stackSize;i++) {
                var actions = ImportHistoryActions(frame);
                stack.Push(actions);
            }
        }

        private void ExportHistoryStack(SerialFrame frame,Stack<HistoryAction[]> stack) {
            var array = stack.ToArray();
            var stackSize = array.Length;
            frame.Set(stackSize);
            for(int i = stackSize - 1;i >= 0;i--) {
                ExportHistoryActions(frame,array[i]);
            }
        }

        private void ImportHistoryStacks(SerialFrame frame) {
            ImportHistoryStack(frame,UndoStack);
            ImportHistoryStack(frame,RedoStack);
        }
        private void ExportHistoryStacks(SerialFrame frame) {
            ExportHistoryStack(frame,UndoStack);
            ExportHistoryStack(frame,RedoStack);
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
