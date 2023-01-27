using System;
using TwelveEngine.Shell;

namespace Elves.Scenes {
    public interface IScene<TGameState> where TGameState:GameState {
        event Action<TGameState,ExitValue> OnSceneEnd;
        void EndScene(ExitValue data);
    }
}
