using Microsoft.Xna.Framework;
using System;

namespace Elves.UI.Battle {
    public abstract class Button:UIElement {

        public bool Selected { get; set; }
        public bool Pressed { get; set; }

        public bool IsEnabled => GetIsEnabled();
        protected virtual bool GetIsEnabled() => true;

        public Rectangle TextureSource { get; set; }

        public Action OnClick;
        internal void Click() {
            if(!IsEnabled) {
                return;
            }
            OnClick?.Invoke();
        }
    }
}
