using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D {
    public sealed class BufferSet {

        private readonly GraphicsDevice graphicsDevice;
        private readonly int _vertexCount;

        private BufferSet(
            GraphicsDevice graphicsDevice,
            VertexBuffer vertexBuffer,
            IndexBuffer indexBuffer,
            int vertexCount
        ) {
            this.graphicsDevice = graphicsDevice;
            _vertexCount = vertexCount;

            VertexBuffer = vertexBuffer;
            IndexBuffer = indexBuffer;
        }

        public VertexBuffer VertexBuffer { get; private set; }
        public IndexBuffer IndexBuffer { get; private set; }
        public int VertexCount => _vertexCount;

        public void Dispose() {
            VertexBuffer?.Dispose();
            IndexBuffer?.Dispose();

            VertexBuffer = null;
            IndexBuffer = null;
        }

        public void Apply() {
            graphicsDevice.SetVertexBuffer(VertexBuffer);
            graphicsDevice.Indices = IndexBuffer;
        }

        internal static BufferSet Create<TVertex>(GraphicsDevice graphicsDevice,TVertex[] vertices) where TVertex : struct {
            var vertexBuffer = new VertexBuffer(graphicsDevice,typeof(TVertex),vertices.Length,BufferUsage.WriteOnly);
            var indexBuffer = new IndexBuffer(graphicsDevice,IndexElementSize.SixteenBits,vertices.Length,BufferUsage.WriteOnly);

            ushort[] indices = new ushort[indexBuffer.IndexCount];
            for(ushort i = 0;i < indices.Length;i++) {
                indices[i] = i;
            }
            indexBuffer.SetData(indices);

            vertexBuffer.SetData(vertices);
            return new BufferSet(graphicsDevice,vertexBuffer,indexBuffer,vertices.Length);
        }
    }
}
