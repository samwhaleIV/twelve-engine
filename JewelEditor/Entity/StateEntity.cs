using System;
using System.Collections.Generic;
using TwelveEngine.Serial;
using System.Runtime.Serialization;

namespace JewelEditor.Entity {

    internal sealed class HistoryEventToken {}

    [Serializable]
    internal class InvalidHistoryEventToken:Exception {
        public InvalidHistoryEventToken() { }
        public InvalidHistoryEventToken(string message) : base(message) { }
        public InvalidHistoryEventToken(string message,Exception inner) : base(message,inner) { }
        protected InvalidHistoryEventToken(SerializationInfo info,StreamingContext context) : base(info,context) { }
    }

    internal sealed class StateEntity:JewelEntity {

        protected override int GetEntityType() => JewelEntities.StateEntity;

        public InputMode InputMode { get; set; } = InputMode.Tile;
        public TileType TileType { get; set; } = TileType.Floor;

        private int entityNameCounter = 1;
        public string GetNewEntityName() => $"Entity_{entityNameCounter++}";

        private readonly Stack<HistoryAction[]> UndoStack = new Stack<HistoryAction[]>();
        private readonly Stack<HistoryAction[]> RedoStack = new Stack<HistoryAction[]>();

        private readonly Queue<HistoryAction> ActionBuffer = new Queue<HistoryAction>();

        private bool IsWritingEvent => activeToken != null;
        private HistoryEventToken activeToken = null;

        private const string BAD_HISTORY_TOKEN = "Provided token is not the active history event token. Did your events get criss-crossed?";
        private const string CANNOT_START_NEW_EVENT = "Cannot start a new history event while one is already active";
        private const string CANNOT_END_NON_EVENT = "Cannot end an event because one has not been started";

        public HistoryEventToken StartHistoryEvent() {
            if(IsWritingEvent) {
                throw new InvalidOperationException(CANNOT_START_NEW_EVENT);
            }
            activeToken = new HistoryEventToken();
            return activeToken;
        }

        public void EndHistoryEvent(HistoryEventToken token) {
            if(token == null) {
                throw new ArgumentNullException("token");
            }
            if(!IsWritingEvent) {
                throw new Exception(CANNOT_END_NON_EVENT);
            }
            if(!Equals(token,activeToken)) {
                throw new InvalidHistoryEventToken(BAD_HISTORY_TOKEN);
            }
            activeToken = null;

            // Write the action queue to an array, clear the action queue, put the array on the undo stack, and clear the redo stack
            if(ActionBuffer.Count <= 0) {
                return;
            }
            var historyActions = ActionBuffer.ToArray();
            ActionBuffer.Clear();

            UndoStack.Push(historyActions);

            RedoStack.Clear();
        }

        public void AddEventAction(HistoryEventToken token,HistoryAction historyAction,bool applyAction = true) {
            if(!Equals(token,activeToken)) {
                throw new InvalidHistoryEventToken(BAD_HISTORY_TOKEN);
            }
            if(!IsWritingEvent) {
                return;
            }
            ActionBuffer.Enqueue(historyAction);
            if(!applyAction) {
                return;
            }
            historyAction.Apply(TileGrid);
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
                action.Undo(TileGrid);
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
                action.Apply(TileGrid);
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
            frame.Set(entityNameCounter);
            ExportHistoryStacks(frame);
        }

        private void StateEntity_OnImport(SerialFrame frame) {
            InputMode = (InputMode)frame.GetInt();
            TileType = (TileType)frame.GetInt();
            entityNameCounter = frame.GetInt();
            ImportHistoryStacks(frame);
        }
    }
}
