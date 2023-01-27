using System;
using TwelveEngine.Shell;

namespace Elves.Scenes {
    public abstract class Scene:InputGameState, IScene<Scene> {
        public Scene() => Name = "Scene";

        public event Action<Scene,ExitValue> OnSceneEnd;
        public void EndScene(ExitValue data) => OnSceneEnd?.Invoke(this,data);
    }
}
