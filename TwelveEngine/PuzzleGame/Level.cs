using System;
using TwelveEngine.Game2D;

namespace TwelveEngine.PuzzleGame {
    public struct Level {
        public string Map;
        public (float X, float Y) Player;
        public Action Components;
        public CameraPadding Padding;
    }
}
