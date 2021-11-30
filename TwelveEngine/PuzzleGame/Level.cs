using System;

namespace TwelveEngine.PuzzleGame {
    public struct Level {
        public string MapName;
        public (float X, float Y) Player;
        public Action<ComponentFactory> GenerateComponents;
    }
}
