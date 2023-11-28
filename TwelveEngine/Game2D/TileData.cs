using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D {
    public readonly struct TileData {
        public Rectangle Source { get; init; }
        public Color Color { get; init; }
        public SpriteEffects SpriteEffects { get; init; }
    }
}
