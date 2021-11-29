using System;

namespace TwelveEngine.PuzzleGame {
    public struct Level {
        public string MapName;
        public float PlayerX;
        public float PlayerY;
        public Func<ComponentFactory,WorldInterface[]> ComponentGenerator;
    }
}
