using TwelveEngine.Serial;
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

        public Vector3 TopLeft { get; set; } = new Vector3(-0.5f,0.5f,0f);
        public Vector3 BottomRight { get; set; } = new Vector3(0.5f,-0.5f,0f);

        public TextureRectangle() {
            OnUpdate += TextureRectangle_OnUpdate;
            OnRender += TextureRectangle_OnRender;

            OnLoad += TextureRectangle_OnLoad;
            OnUnload += TextureRectangle_OnUnload;

            OnImport += TextureRectangle_OnImport;
            OnExport += TextureRectangle_OnExport;
        }

        private void TextureRectangle_OnImport(SerialFrame frame) {
            frame.Set(TopLeft);
            frame.Set(BottomRight);
        }

        private void TextureRectangle_OnExport(SerialFrame frame) {
            TopLeft = frame.GetVector3();
            BottomRight = frame.GetVector3();
        }

        private BufferSet bufferSet;

        private static VertexPositionColorTexture[] GetVertices(Vector3 start,Vector3 end) {

            var a = new VertexPositionColorTexture(start,Color.White,new Vector2(0,0));

            var b = new VertexPositionColorTexture(new Vector3(end.X,start.Y,start.Z),Color.White,new Vector2(1,0));
            var c = new VertexPositionColorTexture(new Vector3(start.X,end.Y,end.Z),Color.White,new Vector2(0,1));

            var d = new VertexPositionColorTexture(end,Color.White,new Vector2(1,1));

            return new VertexPositionColorTexture[] { a,b,c,b,d,c };
        }

        private void TextureRectangle_OnLoad() {
            bufferSet = Owner.CreateBufferSet(GetVertices(TopLeft,BottomRight));
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

        private void TextureRectangle_OnRender(GameTime gameTime) {
            bufferSet.Apply();
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,bufferSet.VertexCount / 3);
            }
        }
    }
}
