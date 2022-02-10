using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Input;

namespace TwelveEngine.UI {
    public class RenderElement:Element {

        public RenderElement() {
            LayoutUpdated += RenderElement_LayoutUpdated;
        }

        private void RenderElement_LayoutUpdated() {
            updateScreenArea();
        }

        public void Unload() {
            foreach(var texture in disposableTextures) {
                texture.Dispose();
            }
            OnUnload?.Invoke();
        }

        protected event Action<GameTime> OnRender, OnUpdate;

        internal void Update(GameTime gameTime) {
            OnUpdate?.Invoke(gameTime);
        }

        internal void Render(GameTime gameTime) {
            OnRender?.Invoke(gameTime);
        }

        /* Must be accessed after or during a Load event */
        protected GameManager Game { get; private set; }

        private Queue<Texture> disposableTextures = new Queue<Texture>();

        protected internal Rectangle ScreenArea { get; private set; }

        private bool hovered = false, pressed = false,
                     isInteractable = false, isScrollable = false;

        protected event Action OnLoad, OnUnload, OnMouseLeave;
        protected event Action<Point> OnMouseDown, OnMouseUp, OnMouseMove;
        protected event Action<Point,ScrollDirection> OnScroll;
        
        public void Load(GameManager game) {
            Game = game;
            OnLoad?.Invoke();
        }

        internal void MouseDown(Point mousePosition) => OnMouseDown?.Invoke(mousePosition);
        internal void MouseUp(Point mousePosition) => OnMouseUp?.Invoke(mousePosition);
        internal void MouseMove(Point mousePosition) => OnMouseMove?.Invoke(mousePosition);
        internal void MouseLeave() => OnMouseLeave?.Invoke();

        internal void Scroll(Point mousePosition,ScrollDirection direction) => OnScroll?.Invoke(mousePosition,direction);

        private void updateScreenArea() => ScreenArea = GetScreenArea();
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
            return Game.Content.Load<Texture2D>(name);
        }

        protected Texture2D GetTexture(int width,int height) {
            var texture = new Texture2D(Game.GraphicsDevice,width,height);
            disposableTextures.Enqueue(texture);
            return texture;
        }

        protected Texture2D GetColoredTexture(Color color) {
            var texture = GetTexture(1,1);
            texture.SetData(new Color[] {color});
            return texture;
        }

        protected void Draw(Texture2D texture,Rectangle source,Color color) {
            Game.SpriteBatch.Draw(texture,ScreenArea,source,color,0f,Vector2.Zero,SpriteEffects.None,Depth);
        }

        protected void Draw(Texture2D texture,Rectangle source) {
            Game.SpriteBatch.Draw(texture,ScreenArea,source,getRenderColor(),0f,Vector2.Zero,SpriteEffects.None,Depth);
        }

        protected void Draw(Texture2D texture,Color color) {
            Game.SpriteBatch.Draw(texture,ScreenArea,null,color,0f,Vector2.Zero,SpriteEffects.None,Depth);
        }

        protected void Draw(Texture2D texture) {
            Game.SpriteBatch.Draw(texture,ScreenArea,null,getRenderColor(),0f,Vector2.Zero,SpriteEffects.None,Depth);
        }

        public float Depth { get; internal set; }

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
