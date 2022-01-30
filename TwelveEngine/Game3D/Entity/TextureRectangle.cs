using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity {
    public abstract class TextureRectangle:WorldMatrixEntity {

        private Texture2D _texture;
        protected Texture2D Texture {
            get => _texture;
            set {
                if(_texture == value) {
                    return;
                }
                _texture = value;
                effect.Texture = _texture;
            }
        }

        private BasicEffect effect;

        public TextureRectangle() {
            OnUpdate += TextureRectangle_OnUpdate;
            OnRender += TextureRectangle_OnRender;
            OnLoad += TextureRectangle_OnLoad;
            OnUnload += TextureRectangle_OnUnload;
        }

        private BufferSet bufferSet;

        private void TextureRectangle_OnLoad() {
            bufferSet = Owner.CreateBufferSet(GetVertices());
            effect = new BasicEffect(Game.GraphicsDevice) {
                TextureEnabled = true
            };

            Owner.OnProjectionMatrixChanged += Owner_OnProjectionMatrixChanged;
            Owner.OnViewMatrixChanged += Owner_OnViewMatrixChanged;

            effect.View = Owner.ViewMatrix;
            effect.Projection = Owner.ProjectionMatrix;
        }

        private void TextureRectangle_OnUpdate(GameTime gameTime) {
            UpdateWorldMatrix(matrix => effect.World = matrix);
        }

        private void Owner_OnViewMatrixChanged(Matrix viewMatrix) {
            effect.View = viewMatrix;
        }

        private void Owner_OnProjectionMatrixChanged(Matrix projectionMatrix) {
            effect.Projection = projectionMatrix;
        }

        private void TextureRectangle_OnUnload() {
            bufferSet?.Dispose();
            bufferSet = null;

            Owner.OnProjectionMatrixChanged -= Owner_OnProjectionMatrixChanged;
            Owner.OnViewMatrixChanged -= Owner_OnViewMatrixChanged;

            effect?.Dispose();
            effect = null;
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
            bufferSet.Apply();
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,bufferSet.VertexCount / 3);
            }
        }
    }
}
