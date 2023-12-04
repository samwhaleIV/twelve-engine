using Microsoft.Xna.Framework;
using System;

namespace John {
    public readonly struct JohnTypeMask {
        public readonly JohnMatchType Type { get; init; }
        public readonly Color Color { get; init; }
        public readonly int ID { get; init; }
    }
}
