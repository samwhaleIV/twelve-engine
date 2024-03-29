﻿namespace TwelveEngine.Game3D.Entity {
    public abstract class TextureRectangle:WorldMatrixEntity {

        public TextureRectangle() {
            OnRender += Render;
            OnLoad += Load;
            OnUnload += Unload;
        }

        private Texture2D _texture;
        public Texture2D Texture {
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

        public void SetColors(Color[] colors) {
            if(colors.Length != 4) {
                return;
            }
            TopLeftColor = colors[0];
            TopRightColor = colors[1];
            BottomLeftColor = colors[2];
            BottomRightColor = colors[3];
        }

        public Vector2 UVTopLeft = Vector2.Zero, UVBottomRight = Vector2.One;

        public void SetColor(Color color) {
            TopLeftColor = color;
            TopRightColor = color;
            BottomLeftColor = color;
            BottomRightColor = color;
        }

        public Color Color {
            set => SetColor(value);
        }

        public void SetUVArea(Rectangle textureArea,float textureWidth,float textureHeight) {
            UVTopLeft = new Vector2(textureArea.X / textureWidth,textureArea.Y / textureHeight);
            UVBottomRight = new Vector2(textureArea.Right / textureWidth,textureArea.Bottom / textureHeight);
        }

        public bool MirrorX { get; set; } = false;
        public bool MirrorY { get; set; } = false;
        public bool MirrorZ { get; set; } = false;

        private const int VERTEX_COUNT = 6;

        private readonly VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[VERTEX_COUNT];

        private void UpdateVertices(Vector3 start,Vector3 end) {
            Vector2 uvTopLeft = UVTopLeft;
            Vector2 uvBottomRight = UVBottomRight;

            if(MirrorX) {
                (uvBottomRight.X, uvTopLeft.X) = (uvTopLeft.X, uvBottomRight.X);
            }

            if(MirrorY) {
                (uvBottomRight.Y, uvTopLeft.Y) = (uvTopLeft.Y, uvBottomRight.Y);
            }

            if(MirrorZ) {
                (end.X, start.X) = (start.X, end.X);
            }

            vertices[0] = new VertexPositionColorTexture(start,TopLeftColor,uvTopLeft+UVOffset);
            vertices[1] = new VertexPositionColorTexture(new Vector3(end.X,start.Y,start.Z),TopRightColor,new Vector2(uvBottomRight.X,uvTopLeft.Y)+UVOffset);
            vertices[2] = new VertexPositionColorTexture(new Vector3(start.X,end.Y,end.Z),BottomLeftColor,new Vector2(uvTopLeft.X,uvBottomRight.Y)+UVOffset);
            vertices[3] = vertices[1];
            vertices[4] = new VertexPositionColorTexture(end,BottomRightColor,uvBottomRight+UVOffset);
            vertices[5] = vertices[2];
        }

        private void Load() {
            bufferSet = Owner.CreateBufferSet(vertices);
            
            effect = new BasicEffect(GraphicsDevice) {
                TextureEnabled = true,
                VertexColorEnabled = true,
                LightingEnabled = false
            };
        }

        private void Unload() {
            bufferSet?.Dispose();
            bufferSet = null;

            effect?.Dispose();
            effect = null;
        }

        protected override void ApplyProjectionMatrix(ref Matrix projectionMatrix) {
            effect.Projection = projectionMatrix;
        }
        protected override void ApplyViewMatrix(ref Matrix viewMatrix) {
            effect.View = viewMatrix;
        }
        protected override void ApplyWorldMatrix(ref Matrix matrix) {
            effect.World = matrix;
        }

        public bool PixelSmoothing { get; set; } = true;

        public float Alpha { get; set; } = 1f;

        private void Render() {
            UpdateVertices(TopLeft,BottomRight);
            bufferSet.VertexBuffer.SetData(vertices);
            bufferSet.Apply();
            var startingSamplerState = Owner.GraphicsDevice.SamplerStates[0];
            Owner.GraphicsDevice.SamplerStates[0] = PixelSmoothing ? SamplerState.LinearWrap : SamplerState.PointWrap;
            effect.Alpha = Alpha;
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,bufferSet.VertexCount / 3);
            }
            Owner.GraphicsDevice.SamplerStates[0] = startingSamplerState;
        }
    }
}
