namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class RenderTargetEntity:TextureRectangle {

        public const int DEFAULT_SIZE = 256;

        public Point Size { get; set; } = new Point(DEFAULT_SIZE);

        private RenderTarget2D renderTarget = null;

        public Action RenderOnTarget { get; set; } = null;

        public RenderTargetEntity() {
            OnLoad += RenderTargetEntity_OnLoad;
            OnUnload += RenderTargetEntity_OnUnload;
            OnPreRender += RenderTargetEntity_OnPreRender;
        }

        private void RenderTargetEntity_OnPreRender() {
            if(RenderOnTarget == null) {
                return;
            }
            RenderTarget.Push(renderTarget);
            RenderOnTarget.Invoke();
            RenderTarget.Pop();
        }

        private void RenderTargetEntity_OnLoad() {
            renderTarget = new RenderTarget2D(GraphicsDevice,Size.X,Size.Y);
            Texture = renderTarget;
        }

        private void RenderTargetEntity_OnUnload() {
            renderTarget?.Dispose();
            renderTarget = null;
        }
    }
}
