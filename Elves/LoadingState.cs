using System;
using System.Collections.Generic;
using System.Text;
using TwelveEngine.Shell;

namespace Elves {
    public sealed class LoadingState:GameState {
        public LoadingState(Func<GameState> gameState) => OnLoad += () => {
            Textures.Load(Game);
            Fonts.Load();
            Game.SetState(gameState);
        };
        public LoadingState(GameState gameState) => OnLoad += () => {
            Textures.Load(Game);
            Fonts.Load();
            Game.SetState(gameState);
        };
    }
}
