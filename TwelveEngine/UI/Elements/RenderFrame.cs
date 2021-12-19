using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Input;

namespace TwelveEngine.UI.Elements {
    public class RenderFrame:RenderElement {

        private readonly GameManager game;

        private readonly Element rootNode;
        public Element Target => rootNode;

        public RenderFrame(UIState state) {
            game = state.Game;

            IsInteractable = true;
            IsScrollable = true;

            OnRender += RenderFrame_OnRender;
            OnUpdate += RenderFrame_OnUpdate;
            LayoutUpdated += Frame_LayoutUpdated;

            OnMouseDown += RenderFrame_OnMouseDown;
            OnMouseUp += RenderFrame_OnMouseUp;
            OnMouseMove += RenderFrame_OnMouseMove;
            OnScroll += RenderFrame_OnScroll;

            OnLoad += RenderFrame_OnLoad;
            OnUnload += RenderFrame_OnUnload;

            rootNode = new Element() {
                Positioning = Positioning.Fixed,
                Sizing = Sizing.Fill
            };
            base.AddChild(rootNode);
        }

        private RenderCache renderCache;

        private bool loaded = false;

        private void RenderFrame_OnUnload() {
            if(!loaded) {
                throw new Exception("Cannot unload, RenderFrame has not been loaded!");
            }
            renderCache.Unload();
        }

        private void RenderFrame_OnLoad() {
            if(loaded) {
                throw new Exception("RenderFrame has already been loaded!");
            }
            loaded = true;
            renderCache = RenderElementSorter.GenerateCache(rootNode);
            renderCache.Load(game);
        }

        public new void AddChild(params Element[] elements) {
            if(loaded) {
                throw new Exception("Cannot add new elements after frame has already been loadeed!");
            }
            foreach(var element in elements) {
                rootNode.AddChild(element);
            }
        }
        public new void RemoveChild(Element element) {
            rootNode.RemoveChild(element);
        }
        public new bool HasParent() => true;

        private void getChildren(Element root,List<RenderElement> list) {
            if(root is RenderElement element) {
                list.Add(element);
            }
            foreach(var child in root.Children) {
                getChildren(child,list);
            }
        }

        private RenderTarget2D renderTarget = null;

        internal void PreRender(GameTime gameTime) {
            if(renderTarget == null) {
                return;
            }
            game.GraphicsDevice.SetRenderTarget(renderTarget);
            renderCache.RenderElements(game.SpriteBatch,gameTime);
            game.GraphicsDevice.SetRenderTarget(null);
        }

        private void RenderFrame_OnUpdate(GameTime gameTime) {
            renderCache.UpdateElements(gameTime);
        }

        private void RenderFrame_OnRender(GameTime gameTime) {
            if(renderTarget == null) {
                return;
            }
            Draw(renderTarget,Color.White);
        }

        private Point lastSize = Point.Zero;

        private void Frame_LayoutUpdated() {
            var newSize = ScreenArea.Size;

            var sizeChanged = lastSize != newSize;
            lastSize = newSize;

            if(!sizeChanged) {
                return;
            }

            renderTarget?.Dispose();
            if(newSize.X < 1 || newSize.Y < 1) {
                renderTarget = null;
                return;
            }
            renderTarget = new RenderTarget2D(game.GraphicsDevice,newSize.X,newSize.Y);
        }


        private void RenderFrame_OnScroll(int x,int y,ScrollDirection direction) {

        }

        private void RenderFrame_OnMouseMove(int x,int y) {

        }

        private void RenderFrame_OnMouseUp(int x,int y) {

        }

        private void RenderFrame_OnMouseDown(int x,int y) {

        }
    }
}
