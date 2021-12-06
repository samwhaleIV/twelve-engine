using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.UI.Elements {
    public abstract class RenderElement:Element {

        public RenderElement() {
            LayoutChanged += updateRenderTarget;
        }

        private Rectangle renderArea;
        private bool focused = false;
        private bool primed = false;

        public Rectangle RenderArea => renderArea;
        public bool Focused {
            get => focused;
            set => focused = value;
        }
        public bool Primed {
            get => primed;
            set => primed = value;
        }

        private void updateRenderTarget() {
            var position = GetPosition();
            var size = GetSize();
            renderArea = new Rectangle() {
                X = position.X,
                Y = position.Y,
                Width = size.Width,
                Height = size.Height
            };
        }

        public void Render(GameTime gameTime) {
            OnRender?.Invoke(this,gameTime);
        }
        public void Load(GameManager gameManager) {
            OnLoad?.Invoke(gameManager);
        }
        public void Unload() {
            OnUnload?.Invoke();
        }

        public event Action<RenderElement,GameTime> OnRender;
        public event Action<GameManager> OnLoad;
        public event Action OnUnload;
    }
}
