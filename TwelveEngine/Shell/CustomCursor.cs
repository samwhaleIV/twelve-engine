using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell {
    public static class CustomCursor {
        private static bool TrySetCustomCursor() {
            if(_useCustomCursor && Sources.TryGetValue(_state,out MouseCursor cursor)) {
                Mouse.SetCursor(cursor);
                return true;
            } else {
                Mouse.SetCursor(MouseCursor.Arrow);
                return false;
            }
        }

        private static bool _useCustomCursor = false;
        public static bool UseCustomCursor {
            get => _useCustomCursor;
            set {
                if(_useCustomCursor == value) {
                    return;
                }
                _useCustomCursor = value;
                TrySetCustomCursor();
            }
        }

        private static CursorState _state = CursorState.Default;
        public static CursorState State {
            get => _state;
            set {
                if(_state == value) {
                    return;
                }
                _state = value;
                if(!_useCustomCursor) {
                    return;
                }
                TrySetCustomCursor();
            }
        }

        public static readonly Dictionary<CursorState,MouseCursor> Sources = new();
    }
}
