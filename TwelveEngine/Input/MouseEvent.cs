namespace TwelveEngine.Input {
    public readonly struct MouseEvent {

        public readonly MouseEventType Type;
        public readonly Point Position;

        private MouseEvent(MouseEventType type,Point position) {
            Type = type;
            Position = position;
        }

        public static MouseEvent LeftClickPress(Point position) {
            return new(MouseEventType.LeftClickPressed,position);
        }

        public static MouseEvent LeftClickRelease(Point position) {
            return new(MouseEventType.LeftClickReleased,position);
        }

        public static MouseEvent RightClickPress(Point position) {
            return new(MouseEventType.LeftClickPressed,position);
        }

        public static MouseEvent RightClickRelease(Point position) {
            return new(MouseEventType.LeftClickReleased,position);
        }

        public static MouseEvent Scroll(bool scrollingUp,Point position) {
            return new(scrollingUp ? MouseEventType.ScrollUp : MouseEventType.ScrollDown,position);
        }

        public static MouseEvent Move(Point position) {
            return new(MouseEventType.PositionChanged,position);
        }

        public static MouseEvent Update(Point position) {
            return new(MouseEventType.Update,position);
        }
    }
}
