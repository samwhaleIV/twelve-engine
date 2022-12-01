using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class RenderTargetEntity:TextureRectangle {

        /* Does not work with serializer! */

        public const int DEFAULT_SIZE = 256;

        public Point Size { get; set; } = new Point(DEFAULT_SIZE);

        private RenderTarget2D renderTarget = null;

        public Action<GameTime> RenderOnTarget { get; set; } = null;

        public RenderTargetEntity() {
            OnLoad += RenderTargetEntity_OnLoad;
            OnUnload += RenderTargetEntity_OnUnload;
            OnPreRender += RenderTargetEntity_OnPreRender;
        }

        private void RenderTargetEntity_OnPreRender(GameTime gameTime) {
            if(RenderOnTarget == null) {
                return;
            }
            Game.SetRenderTarget(renderTarget);
            RenderOnTarget.Invoke(gameTime);
            Game.RestoreRenderTarget();
        }

        private void RenderTargetEntity_OnLoad() {
            renderTarget = new RenderTarget2D(Game.GraphicsDevice,Size.X,Size.Y);
            Texture = renderTarget;
        }

        private void RenderTargetEntity_OnUnload() {
            renderTarget?.Dispose();
            renderTarget = null;
        }
    }
}
