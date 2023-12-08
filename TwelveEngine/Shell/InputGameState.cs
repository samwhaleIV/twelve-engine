using TwelveEngine.Input;
using TwelveEngine.Input.Routing;

namespace TwelveEngine.Shell {
    public class InputGameState:GameState {

        public ImpulseEventHandler ImpulseHandler { get; private init; } = new();
        public MouseEventHandler MouseHandler { get; private init; } = new();

        private static ImpulseEventHandler oldInputHandler;
        private static MouseEventHandler oldMouseHandler;

        public VirtualMouseProvider VirtualMouseProvider { get; private init; }

        public InputGameState() {
            OnLoad.Add(ImportOldHandlers,EventPriority.First);
            OnUpdate.Add(UpdateInputs,EventPriority.First);
            OnUnload.Add(SetOldHandlers);
            VirtualMouseProvider = new(this);
            OnRender.Add(RenderVirtualMouse,EventPriority.Last);

            if(Flags.Get(Constants.Flags.DebugInputEvents)) {
                ImpulseHandler.OnEvent += DebugImpulseEvents;
                MouseHandler.OnEvent += DebugMouseEvents;
            }
        }

        private void DebugImpulseEvents(ImpulseEvent impulseEvent) {
            Console.WriteLine($"Impulse: {impulseEvent.Impulse}, Pressed: {impulseEvent.Pressed}");
        }

        private void DebugMouseEvents(MouseEvent mouseEvent) {
            if(mouseEvent.Type == MouseEventType.Update || mouseEvent.Type == MouseEventType.PositionChanged) {
                return;
            }
            Console.WriteLine($"Mouse Event: {mouseEvent.Type}, Position: {mouseEvent.Position}");
        }

        private void RenderVirtualMouse() {
            VirtualMouseProvider.TryRenderVirtualMouse(SpriteBatch,CustomCursor.State);
        }

        private void SetOldHandlers() {
            oldInputHandler = ImpulseHandler;
            oldMouseHandler = MouseHandler;
        }

        private void ImportOldHandlers() {
            if(HasFlag(StateFlags.CarryKeyboardInput)) {
                ImpulseHandler.Import(oldInputHandler);
            }
            if(HasFlag(StateFlags.CarryMouseInput)) {
                MouseHandler.Import(oldMouseHandler);
            }
            oldInputHandler = null;
            oldMouseHandler = null;
        }

        /// <summary>
        /// Activated on the first frame that a transition ends.
        /// </summary>
        public event Action OnInputActivated;

        private bool _inputActivated = false, _lastUpdateSentEvents = false;

        private void UpdateInputs() {
            if(!IsTransitioning && !_inputActivated) {
                _inputActivated = true;
                OnInputActivated?.Invoke();
            }

            bool eventsAreAllowed = GameIsActive && !IsTransitioning && _inputActivated;

            if(!eventsAreAllowed && _lastUpdateSentEvents) {
                ImpulseHandler.Release();
                MouseHandler.Release();
            }
            _lastUpdateSentEvents = eventsAreAllowed;

            MouseHandler.Update(eventsAreAllowed);
            ImpulseHandler.Update(eventsAreAllowed);

            VirtualMouseProvider.Update();
        }

        internal override bool GetCustomCursorHiddenState() {
            bool? hiddenState = VirtualMouseProvider.HiddenState;
            if(hiddenState.HasValue) {
                return hiddenState.Value;
            }
            return base.GetCustomCursorHiddenState();
        }
    }
}
