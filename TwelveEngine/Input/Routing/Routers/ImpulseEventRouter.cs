namespace TwelveEngine.Input.Routing {
    public sealed class ImpulseEventRouter:InputEventRouter<ImpulseEvent,ImpulseEventHandler> {
        private readonly Dictionary<Impulse,(Action OnPressed,Action OnReleased)> endPoints;

        public ImpulseEventRouter() => endPoints = GetEndpoints();

        public override void RouteEvent(ImpulseEvent impulseEvent) {
            if(!endPoints.TryGetValue(impulseEvent.Impulse, out var endPoint)) {
                return;
            }
            (impulseEvent.Pressed ? endPoint.OnPressed : endPoint.OnReleased).Invoke();
        }

        public event Action OnAcceptDown, OnAcceptUp, OnCancelDown, OnCancelUp, OnFocusDown, OnFocusUp, OnDebugDown, OnDebugUp;
        public event Action<Direction> OnDirectionUp, OnDirectionDown;

        private void AcceptDown() => OnAcceptDown?.Invoke();
        private void AcceptUp() => OnAcceptUp?.Invoke();
        private void CancelDown() => OnCancelDown?.Invoke();
        private void CancelUp() => OnCancelUp?.Invoke();

        private void LeftDown() => OnDirectionDown?.Invoke(Direction.Left);
        private void LeftUp() => OnDirectionUp?.Invoke(Direction.Left);

        private void RightDown() => OnDirectionDown?.Invoke(Direction.Right);
        private void RightUp() => OnDirectionUp?.Invoke(Direction.Right);

        private void UpDown() => OnDirectionDown?.Invoke(Direction.Up);
        private void UpUp() => OnDirectionUp?.Invoke(Direction.Left);

        private void DownDown() => OnDirectionDown?.Invoke(Direction.Down);
        private void DownUp() => OnDirectionUp?.Invoke(Direction.Down);

        private void FocusDown() => OnFocusDown?.Invoke();
        private void FocusUp() => OnFocusUp?.Invoke();

        private void DebugDown() => OnDebugDown?.Invoke();
        private void DebugUp() =>  OnDebugUp?.Invoke();

        /// <summary>
        /// She dreamt of late binding, every time she closed her eyes...
        /// </summary>
        /// <returns>An event end point set.</returns>
        private Dictionary<Impulse,(Action OnPressed,Action OnReleased)> GetEndpoints() => new() {
            {Impulse.Accept,(AcceptDown,AcceptUp)},
            {Impulse.Cancel,(CancelDown,CancelUp)},
            {Impulse.Focus,(FocusDown,FocusUp)},

            {Impulse.Up,(UpDown,UpUp)},
            {Impulse.Down,(DownDown,DownUp)},
            {Impulse.Left,(LeftDown,LeftUp)},
            {Impulse.Right,(RightDown,RightUp)},

            {Impulse.Debug,(DebugDown,DebugUp)}
        };
    }
}
