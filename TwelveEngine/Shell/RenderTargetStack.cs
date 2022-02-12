using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Shell {
    public sealed class RenderTargetStack {

        internal RenderTargetStack(GraphicsDevice graphicsDevice) {
            this.graphicsDevice = graphicsDevice;
            renderTargetStack = new Stack<RenderTargetData>();
        }

        private readonly Stack<RenderTargetData> renderTargetStack;
        private readonly GraphicsDevice graphicsDevice;

        private readonly struct RenderTargetData {
            public RenderTargetData(RenderTarget2D renderTarget,Viewport viewport) {
                RenderTarget = renderTarget;
                Viewport = viewport;
            }

            public readonly RenderTarget2D RenderTarget;
            public readonly Viewport Viewport;
        }

        public Viewport GetViewport() {
            if(renderTargetStack.Count == 0) {
                return graphicsDevice.Viewport;
            } else {
                return renderTargetStack.Peek().Viewport;
            }
        }

        public void Push(RenderTarget2D renderTarget) {
            if(renderTargetStack.Count > 0) {
                var viewport = new Viewport(0,0,renderTarget.Width,renderTarget.Height,0f,1f);
                var renderTargetData = new RenderTargetData(renderTarget,viewport);
                renderTargetStack.Push(renderTargetData);
            }
            graphicsDevice.SetRenderTarget(renderTarget);
        }

        public void Pop() {
            if(!renderTargetStack.TryPop(out var renderTargetData)) {
                graphicsDevice.SetRenderTarget(null);
            } else {
                graphicsDevice.SetRenderTarget(renderTargetData.RenderTarget);
            }
        }
    }
}
