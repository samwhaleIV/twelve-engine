namespace TwelveEngine.Input.Routing {
    public sealed class MouseEventRouter:InputEventRouter<MouseEvent,MouseEventHandler> {

        public event Action OnPress, OnRelease, OnMove, OnAltPress, OnAltRelease;
        public event Action<Direction> OnScroll;

        public event Action<Point> OnUpdate;

        public override void RouteEvent(MouseEvent routedEvent) {
            switch(routedEvent.Type) {
                case MouseEventType.Update: OnUpdate?.Invoke(routedEvent.Position); break;
                case MouseEventType.LeftClickPressed: OnPress?.Invoke(); break;
                case MouseEventType.LeftClickReleased: OnRelease?.Invoke();  break;
                case MouseEventType.RightClickPressed: OnAltPress?.Invoke(); break;
                case MouseEventType.RightClickReleased: OnAltRelease?.Invoke(); break;
                case MouseEventType.ScrollUp: OnScroll?.Invoke(Direction.Up); break;
                case MouseEventType.ScrollDown: OnScroll?.Invoke(Direction.Down); break;
                case MouseEventType.PositionChanged: OnMove?.Invoke(); break;
            }
        }
    }
}
