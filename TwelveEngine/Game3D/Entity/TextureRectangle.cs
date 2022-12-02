using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity {
    public abstract class TextureRectangle:WorldMatrixEntity {

        private Texture2D _texture;
        public Texture2D Texture {
            get => _texture;
            protected set {
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
        }

        private BufferSet bufferSet;

        public Vector2 UVOffset = Vector2.Zero;

        public Color TopLeftColor { get; set; } = Color.White;
        public Color TopRightColor { get; set; } = Color.White;
        public Color BottomLeftColor { get; set; } = Color.White;
        public Color BottomRightColor { get; set; } = Color.White;

        public void SetColors(Color topLeft,Color topRight,Color bottomLeft,Color bottomRight) {
            TopLeftColor = topLeft;
            TopRightColor = topRight;
            BottomLeftColor = bottomLeft;
            BottomRightColor = bottomRight;
        }

        public Vector2 UVTopLeft = Vector2.Zero;
        public Vector2 UVBottomRight = Vector2.One;

        public void SetColor(Color color) {
            TopLeftColor = color;
            TopRightColor = color;
            BottomLeftColor = color;
            BottomRightColor = color;
        }

        public Color Color {
            set => SetColor(value);
        }

        private VertexPositionColorTexture[] GetVertices(Vector3 start,Vector3 end) {

            var a = new VertexPositionColorTexture(start,TopLeftColor,UVTopLeft+UVOffset);

            var b = new VertexPositionColorTexture(new Vector3(end.X,start.Y,start.Z),TopRightColor,new Vector2(UVBottomRight.X,UVTopLeft.Y)+UVOffset);
            var c = new VertexPositionColorTexture(new Vector3(start.X,end.Y,end.Z),BottomLeftColor,new Vector2(UVTopLeft.X,UVBottomRight.Y)+UVOffset);

            var d = new VertexPositionColorTexture(end,BottomRightColor,UVBottomRight+UVOffset);

            return new VertexPositionColorTexture[] { a,b,c,b,d,c };
        }

        private void TextureRectangle_OnLoad() {
            bufferSet = Owner.CreateBufferSet(GetVertices(TopLeft,BottomRight));
            
            effect = new BasicEffect(Game.GraphicsDevice) {
                TextureEnabled = true,
                VertexColorEnabled = true,
                LightingEnabled = false
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

        public bool PixelSmoothing { get; set; } = false;

        private void TextureRectangle_OnRender(GameTime gameTime) {
            bufferSet.VertexBuffer.SetData(GetVertices(TopLeft,BottomRight));
            bufferSet.Apply();
            Owner.PushSamplerState(PixelSmoothing ? SamplerState.LinearWrap : SamplerState.PointWrap);
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,bufferSet.VertexCount / 3);
            }
            Owner.PopSamplerState();
        }
    }
}
