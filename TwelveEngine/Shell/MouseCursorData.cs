using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell {
    public readonly struct MouseCursorData {

        public MouseCursorData(Texture2D texture,MouseCursor mouseCursor) {
            Texture = texture;
            MouseCursor = mouseCursor;
        }

        public readonly MouseCursor MouseCursor { get; private init; }
        public readonly Texture2D Texture { get; private init; }
    }
}
