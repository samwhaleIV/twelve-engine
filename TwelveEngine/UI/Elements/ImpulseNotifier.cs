using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Input;

namespace TwelveEngine.UI.Elements {
    public sealed class ImpulseNotifier:RenderElement {

        private const string CONTROLLER_GLYPHS = "controller-glpyhs", KEYBOARD_GLYPHS = "keyboard-glyphs";
        private Texture2D controllerGlyphs, keyboardGlyphs;

        private ImpulseHandler impulseHandler;

        public ImpulseNotifier() {
            OnLoad += ImpulseNotifier_OnLoad;
            OnRender += ImpulseNotifier_OnRender;
            OnUnload += ImpulseNotifier_OnUnload;
        }


        private void ImpulseNotifier_OnRender(GameTime gameTime) {
            throw new NotImplementedException();
        }

        private void ImpulseNotifier_OnUnload() {
            impulseHandler.OnInputMethodChanged -= ImpulseHandler_OnInputMethodChanged;
        }

        private void ImpulseNotifier_OnLoad() {
            controllerGlyphs = GetImage(CONTROLLER_GLYPHS);
            keyboardGlyphs = GetImage(KEYBOARD_GLYPHS);

            impulseHandler = Game.ImpulseHandler;
            impulseHandler.OnInputMethodChanged += ImpulseHandler_OnInputMethodChanged;
        }

        private void ImpulseHandler_OnInputMethodChanged(InputMethod obj) {
            throw new NotImplementedException();
        }
    }
}
