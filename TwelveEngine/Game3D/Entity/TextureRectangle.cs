using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity {
    public abstract class TextureRectangle:WorldMatrixEntity {

        protected Texture2D Texture { get; set; }

        public TextureRectangle() {
            OnUpdate += TextureRectangle_OnUpdate;
            OnRender += TextureRectangle_OnRender;
            OnLoad += TextureRectangle_OnLoad;
            OnUnload += TextureRectangle_OnUnload;
        }

        private BufferSet bufferSet;

        private void TextureRectangle_OnLoad() {
            bufferSet = Owner.CreateBufferSet(GetVertices());
        }

        private void TextureRectangle_OnUnload() {
            bufferSet?.Dispose();
            bufferSet = null;
        }

        private Matrix worldMatrix;

        private void TextureRectangle_OnUpdate(GameTime gameTime) {
            UpdateWorldMatrix(matrix => worldMatrix = matrix);
        }

        private static VertexPositionColorTexture[] GetVertices() {
            float left = -0.5f, top = -0.5f, right = 0.5f, bottom = 0.5f;

            var a = new VertexPositionColorTexture(new Vector3(left,top,0),Color.White,new Vector2(1,1));
            var b = new VertexPositionColorTexture(new Vector3(right,top,0),Color.White,new Vector2(0,1));

            var c = new VertexPositionColorTexture(new Vector3(left,bottom,0),Color.White,new Vector2(1,0));
            var d = new VertexPositionColorTexture(new Vector3(right,bottom,0),Color.White,new Vector2(0,0));

            return new VertexPositionColorTexture[] { a,b,c,b,d,c };
        }

        private void TextureRectangle_OnRender(GameTime gameTime) {
            var effect = Owner.BasicEffect;

            effect.World = worldMatrix;
            effect.Texture = Texture;
            effect.TextureEnabled = true;

            bufferSet.Apply();
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,bufferSet.VertexCount / 3);
            }
        }
    }
}
