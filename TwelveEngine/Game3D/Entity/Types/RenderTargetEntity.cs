namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class RenderTargetEntity:TextureRectangle {

        public const int DEFAULT_SIZE = 256;

        public Point Size { get; set; } = new Point(DEFAULT_SIZE);

        private RenderTarget2D renderTarget = null;

        public Action RenderOnTarget { get; set; } = null;

        public RenderTargetEntity() {
            OnLoad += LoadRenderTarget;
            OnUnload += UnloadRenderTarget;
            OnPreRender += RenderToRenderTarget;
        }

        private void RenderToRenderTarget() {
            if(RenderOnTarget == null) {
                return;
            }
            RenderTarget.Push(renderTarget);
            RenderOnTarget.Invoke();
            RenderTarget.Pop();
        }

        private void LoadRenderTarget() {
            renderTarget = new RenderTarget2D(GraphicsDevice,Size.X,Size.Y);
            Texture = renderTarget;
        }

        private void UnloadRenderTarget() {
            renderTarget?.Dispose();
            renderTarget = null;
        }
    }
}
