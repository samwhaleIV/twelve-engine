using System;

namespace TwelveEngine.Input.Routing {
    public sealed class MouseEventRouter:InputEventRouter<MouseEvent,MouseEventHandler> {

        public event Action OnPress, OnRelease, OnMove;
        public event Action<Direction> OnScroll;

        public override void RouteEvent(MouseEvent routedEvent) {
            switch(routedEvent.Type) {
                case MouseEventType.LeftClickPressed: OnPress?.Invoke(); break;
                case MouseEventType.LeftClickReleased: OnRelease?.Invoke();  break;
                case MouseEventType.ScrollUp: OnScroll?.Invoke(Direction.Up); break;
                case MouseEventType.ScrollDown: OnScroll?.Invoke(Direction.Down); break;
                case MouseEventType.PositionChanged: OnMove?.Invoke(); break;
            }
        }
    }
}
