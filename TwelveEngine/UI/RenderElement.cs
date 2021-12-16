using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Input;

namespace TwelveEngine.UI {
    public abstract class RenderElement:Element {

        public RenderElement() => LayoutUpdated += updateScreenArea;
        public abstract void Render(GameTime gameTime);

        private GameManager game;
        private Queue<Texture> disposableTextures = new Queue<Texture>();

        private Rectangle screenArea;
        private float depth = 0;

        private bool hovered = false, pressed = false,
                     isInteractable = false, isScrollable = false;

        protected event Action OnLoad, OnUnload;
        public event Action OnMouseDown, OnMouseUp;

        public event Action<MouseMoveData> OnMouseMove;
        public event Action<ScrollDirection> OnScroll;
        
        internal void Load(GameManager game) {
            this.game = game;
            OnLoad?.Invoke();
        }

        internal void Unload() {
            foreach(var texture in disposableTextures) {
                texture.Dispose();
            }
            OnUnload?.Invoke();
        }

        internal void MouseDown() => OnMouseDown?.Invoke();
        internal void MouseUp() => OnMouseUp?.Invoke();
        internal void MouseMove(MouseMoveData data) => OnMouseMove?.Invoke(data);

        internal void Scroll(ScrollDirection direction) => OnScroll?.Invoke(direction);

        private void updateScreenArea() => screenArea = GetScreenArea();
        protected virtual Rectangle GetScreenArea() => ComputedArea;

        private Color getRenderColor() {
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

        protected Texture2D GetImage(string name) {
            return game.Content.Load<Texture2D>(name);
        }

        protected Texture2D GetTexture(int width,int height) {
            var texture = new Texture2D(game.GraphicsDevice,width,height);
            disposableTextures.Enqueue(texture);
            return texture;
        }

        protected Texture2D GetColoredTexture(Color color) {
            var texture = GetTexture(1,1);
            texture.SetData(new Color[] {color});
            return texture;
        }

        protected void Draw(Texture2D texture,Rectangle source,Color color) {
            game.SpriteBatch.Draw(texture,screenArea,source,color,0f,Vector2.Zero,SpriteEffects.None,depth);
        }

        protected void Draw(Texture2D texture,Rectangle source) {
            game.SpriteBatch.Draw(texture,screenArea,source,getRenderColor(),0f,Vector2.Zero,SpriteEffects.None,depth);
        }

        protected void Draw(Texture2D texture,Color color) {
            game.SpriteBatch.Draw(texture,screenArea,null,color,0f,Vector2.Zero,SpriteEffects.None,depth);
        }

        protected void Draw(Texture2D texture) {
            game.SpriteBatch.Draw(texture,screenArea,null,getRenderColor(),0f,Vector2.Zero,SpriteEffects.None,depth);
        }

        internal Rectangle ScreenArea {
            get => screenArea;
        }

        internal float Depth {
            get => depth;
            set => depth = value;
        }

        public bool Hovered {
            get => hovered;
            internal set => hovered = value;
        }

        public bool Pressed {
            get => pressed;
            internal set => pressed = value;
        }

        public bool IsInteractable {
            get => isInteractable;
            set => isInteractable = value;
        }

        public bool IsScrollable {
            get => isScrollable;
            set => isScrollable = value;
        }
    }
}
