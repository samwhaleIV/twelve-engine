using TwelveEngine.UI;
using TwelveEngine;

namespace Elves.Scenes.SplashMenu {
    public sealed class SplashMenuButton:InteractionElement<SplashMenuButton> {
        private readonly SplashMenuState menu;

        public SplashMenuButton(SplashMenuState menu) {
            this.menu = menu;
            Endpoint = new(Click);
        }

        private void Click() => menu.EndScene(ExitValue.None);

        public override FloatRectangle GetScreenArea() {
            return menu.PlayButton.Area;
        }
    }
}
