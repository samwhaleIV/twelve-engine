using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;
using TwelveEngine.Shell.States;

namespace Elves {
    public sealed class MemoryTestState:InputGameState {
        public MemoryTestState() {
            OnUpdate += MemoryTestState_OnUpdate;
        }

        private void MemoryTestState_OnUpdate(Microsoft.Xna.Framework.GameTime gameTime) {
            UpdateInputs(gameTime);
        }
    }
}
