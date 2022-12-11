using System;

namespace Elves.UI.Battle {
    public abstract class Button:NineGrid {

        public bool Selected { get; set; }
        public bool Pressed { get; set; }

        public bool IsEnabled => GetIsEnabled();
        protected virtual bool GetIsEnabled() => true;

        public Action OnClick;
        internal void Click() {
            if(!IsEnabled) {
                return;
            }
            OnClick?.Invoke();
        }
    }
}
