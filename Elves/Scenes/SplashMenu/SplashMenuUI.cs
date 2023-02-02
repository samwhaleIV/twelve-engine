using System.Collections.Generic;
using TwelveEngine.UI;

namespace Elves.Scenes.SplashMenu {
    public sealed class SplashMenuUI:InputGameStateUIProxy<SplashMenuState,SplashMenuButton> {

        private readonly List<SplashMenuButton> buttonList;

        public SplashMenuUI(SplashMenuState state) : base(state) {
            buttonList = new List<SplashMenuButton>() { new SplashMenuButton(State) };
        }

        protected override IEnumerable<SplashMenuButton> GetElements() {
            return buttonList;
        }
    }
}
