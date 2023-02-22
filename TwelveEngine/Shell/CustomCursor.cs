using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell {
    public static class CustomCursor {

        private static bool _enabled = false;
        public static bool Enabled {
            get => _enabled;
            set {
                if(_enabled == value) {
                    return;
                }
                _enabled = value;
                if(!value) {
                    Mouse.SetCursor(MouseCursor.Arrow);
                    return;
                }
                SetCustomCursor(Hidden ? CursorState.None : State);
            }
        }

        private static bool SetCustomCursor(CursorState state) {
            if(Sources.TryGetValue(state,out MouseCursorData cursor)) {
                Mouse.SetCursor(cursor.MouseCursor);
                return true;
            } else {
                Mouse.SetCursor(MouseCursor.Arrow);
                return false;
            }
        }

        private static bool TrySetCustomCursor(CursorState state) {
            if(!Enabled) {
                return false;
            }
            return SetCustomCursor(state);
        }

        private static CursorState _state = CursorState.Default;
        public static CursorState State {
            get => _state;
            set {
                if(_state == value) {
                    return;
                }
                _state = value;
                if(Hidden) {
                    return;
                }
                TrySetCustomCursor(value);
            }
        }

        private static bool _hidden = false;

        public static bool Hidden {
            get => _hidden;
            set {
                if(_hidden == value) {
                    return;
                }
                _hidden = value;
                if(value) {
                    TrySetCustomCursor(CursorState.None);
                } else {
                    TrySetCustomCursor(State);
                }
            }
        }

        public static readonly Dictionary<CursorState,MouseCursorData> Sources = new();
    }
}
