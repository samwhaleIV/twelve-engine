using System;
using System.Collections.Generic;
using TwelveEngine.Shell;
using TwelveEngine.UI;

namespace Elves.Scenes.SplashMenu {
    public sealed class SplashMenuUI:InteractionAgent<SplashMenuButton> {

        private readonly InputGameState menu;

        public SplashMenuUI(SplashMenuState menu) {
            this.menu = menu;
            BindInputEvents(menu);
            var button = new SplashMenuButton(menu);
            button.CanInteract = true;
            buttons.Add(button);
            DefaultFocusElement = button;
            menu.OnUpdate += Menu_OnUpdate;
        }

        private void Menu_OnUpdate() {
            CustomCursor.State = CursorState;
        }

        protected override bool GetContextTransitioning() {
            return menu.IsTransitioning;
        }

        protected override TimeSpan GetCurrentTime() {
            return menu.Now;
        }

        private readonly List<SplashMenuButton> buttons = new();

        protected override IEnumerable<SplashMenuButton> GetElements() {
            return buttons;
        }

        protected override bool GetLeftMouseButtonIsCaptured() {
            return menu.Mouse.CapturingLeft;
        }

        protected override bool GetRightMouseButtonIsCaptured() {
            return menu.Mouse.CapturingRight;
        }
    }
}
