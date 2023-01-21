using Elves.UI.SpriteUI;
using System;

namespace Elves.Scenes.SaveSelect {
    public sealed class Tag:SpriteElement {

        public Tag() {
            TextureSource = new(52,126,128,32);
            Offset = new(-0.5f,-0.5f);
            OnUpdate += Tag_OnUpdate;
            OnSelect += Tag_OnHover;
            OnSelectEnd += Tag_OnHoverEnd;
            OnPress += Tag_OnPress;
            OnDepress += Tag_OnDepress;
        }

        private void Tag_OnDepress() {
            pressed = false;
        }

        private void Tag_OnPress() {
            pressed = true;
        }

        private void Tag_OnHoverEnd() {
            hovered = false;
        }

        private void Tag_OnHover() {
            hovered = true;
        }

        public bool hovered = false, pressed = false;

        private void Tag_OnUpdate(TimeSpan now) {
            var newScale = 1f;
            if(hovered) {
                newScale = 1.05f;
            }
            if(pressed) {
                newScale *= 0.95f;
            }
            if(Scale == newScale) {
                return;
            }
            KeyAnimation(now);
            Scale = newScale;
        }
    }
}
