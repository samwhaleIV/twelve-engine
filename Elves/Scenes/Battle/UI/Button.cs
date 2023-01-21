using Microsoft.Xna.Framework;
using System;

namespace Elves.Scenes.Battle.UI {
    public abstract class Button:UIElement {

        public bool Selected { get; set; }
        public bool Pressed { get; set; }

        public bool IsEnabled => GetIsEnabled();
        protected virtual bool GetIsEnabled() => true;

        public Rectangle TextureSource { get; set; }
        public int ID { get; set; }

        public Action<int> OnClick;

        public void Click() {
            if(!IsEnabled) {
                return;
            }
            OnClick?.Invoke(ID);
        }
    }
}
