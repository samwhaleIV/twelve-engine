﻿using System;
using TwelveEngine.Game2D;

namespace Porthole.PuzzleGame {
    public struct Level {
        public string Map;
        public (float X, float Y) Player;
        public Action Components;
        public bool CameraPaddingX;
        public bool CameraPaddingY;
    }
}
