using Microsoft.Xna.Framework;

namespace John {
    public enum JohnStartPositionDirection { Random, FacingRight, FacingLeft }
    public readonly struct JohnStartPosition {
        public JohnStartPositionDirection Direction { get; init; }
        public Vector2 Value { get; init; }
    }
}
