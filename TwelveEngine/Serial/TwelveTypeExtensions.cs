﻿using Microsoft.Xna.Framework.Input;
using TwelveEngine.Game2D;
using TwelveEngine.PuzzleGame;
using System.Collections.Generic;

namespace TwelveEngine {
    public sealed partial class SerialFrame {
        public void Set(EntityType entityType) => Set((int)entityType);
        public EntityType GetEntityType() => (EntityType)GetInt();

        public void Set(Direction direction) => Set((int)direction);
        public Direction GetDirection() => (Direction)GetInt();

        public void Set(SignalState signalState) => Set((int)signalState);
        public SignalState GetSignalState() => (SignalState)GetInt();

        public void Set(Keys key) => Set((byte)key);
        public Keys GetKey() => (Keys)GetByte();

        public void Set(KeyBind bind) => Set((byte)bind);
        public KeyBind GetBind() => (KeyBind)GetByte();
    }
}
