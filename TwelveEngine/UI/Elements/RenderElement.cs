using System;
using Microsoft.Xna.Framework;
using TwelveEngine.Input;

namespace TwelveEngine.UI.Elements {
    public abstract class RenderElement:Element {

        private GameManager game;
        protected GameManager Game => game;

        protected event Action OnLoad;
        protected event Action OnUnload;

        internal void Load(GameManager game) {
            this.game = game;
            OnLoad?.Invoke();
        }
        internal void Unload() => OnUnload?.Invoke();

        public bool IsInteractable { get; set; } = false;

        private bool hovered = false, pressed = false;

        public bool Hovered {
            get => hovered;
            internal set => hovered = value;
        }
        public bool Pressed {
            get => pressed;
            internal set => pressed = value;
        }

        public event Action OnClick;
        internal void Click() => OnClick?.Invoke();

        protected event Action<ScrollDirection> OnScroll;
        internal void Scroll(ScrollDirection direction) => OnScroll?.Invoke(direction);

        public abstract void Render(GameTime gameTime);

        public RenderElement() => LayoutUpdated += updateScreenArea;

        private Rectangle screenArea;
        public Rectangle ScreenArea => screenArea;

        private void updateScreenArea() => screenArea = GetScreenArea();
        protected virtual Rectangle GetScreenArea() => ComputedArea;

        protected Color GetRenderColor() {
            if(!IsInteractable) {
                return Color.White;
            }
            if(Pressed) {
                return Color.DarkGray;
            } else if(Hovered) {
                return Color.LightGray;
            } else {
                return Color.White;
            }
        }
    }
}
