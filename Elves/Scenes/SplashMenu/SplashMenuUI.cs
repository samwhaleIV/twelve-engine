using System.Collections.Generic;
using TwelveEngine.UI;

namespace Elves.Scenes.SplashMenu {
    public sealed class SplashMenuUI:InputGameStateUIProxy<SplashMenuState,SplashMenuButton> {

        private readonly List<SplashMenuButton> buttonList;

        public SplashMenuUI(SplashMenuState state) : base(state) {
            var button = new SplashMenuButton(State);
            buttonList = new List<SplashMenuButton>() { button };
            DefaultFocusElement = button;
        }

        protected override IEnumerable<SplashMenuButton> GetElements() {
            return buttonList;
        }
    }
}
