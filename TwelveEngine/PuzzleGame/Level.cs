using System;

namespace TwelveEngine.PuzzleGame {
    public struct Level {
        public string Map;
        public (float X, float Y) Player;
        public Action Components;
    }
}
