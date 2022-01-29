using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class RenderTargetEntity:TextureRectangle {

        /* Does not work with serializer! */

        public const int RENDER_TARGET_SIZE = 256;
        protected override int GetEntityType() => Entity3DType.RenderTarget;

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
            Owner.Game.SetRenderTarget(renderTarget);
            RenderOnTarget.Invoke(gameTime);
            Owner.Game.RestoreRenderTarget();
        }

        private void RenderTargetEntity_OnLoad() {
            renderTarget = new RenderTarget2D(Owner.Game.GraphicsDevice,RENDER_TARGET_SIZE,RENDER_TARGET_SIZE);
            Texture = renderTarget;
        }

        private void RenderTargetEntity_OnUnload() {
            renderTarget?.Dispose();
            renderTarget = null;
        }
    }
}
