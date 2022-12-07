using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace TwelveEngine.Game3D {

    public sealed class SharedBuffer {

        private const int BLOCK_SIZE = 64;
        private const int BUFFER_SIZE = ushort.MaxValue;

        private int index = 0;

        /* Allocations doesn't reflect overfill potential. */
        private readonly Dictionary<int,int> allocations = new Dictionary<int,int>(); /* <index,blockCount> */

        private readonly Queue<int> deletedBlocks = new Queue<int>(); /* <index> */

        public int Allocate(int vertexCount) {
            int blockCount = (int)MathF.Ceiling((float)vertexCount / BLOCK_SIZE);
            if(deletedBlocks.Count >= 1 && blockCount == 1) {
                /* We can't reallocate multiple blocks in a row because the buffer reading crosses block boundaries and we don't translate indices */
                return deletedBlocks.Dequeue();
            }
            int allocationSize = blockCount * BLOCK_SIZE;
            if(index + allocationSize >= BUFFER_SIZE) {
                throw new SharedBufferException("Cannot allocate space, the buffer has reached the end");
            }
            int handle = index;
            allocations[index] = blockCount;
            index += allocationSize;
            return handle;
        }

        public int Allocate(VertexPositionColorTexture[] data) {
            int index = Allocate(data.Length);
            SetVertices(index,data);
            return index;
        }

        public void Release(int index) {
            if(!allocations.TryGetValue(index, out int blockCount)) {
                throw new SharedBufferException("Cannot free block that has not been allocated.");
            }
            for(int i = 0;i < blockCount; i++) {
                deletedBlocks.Enqueue(index+BLOCK_SIZE * i);
            }
            allocations.Remove(index);
        }

        private VertexBuffer vertexBuffer;
        private IndexBuffer indexBuffer;

        public VertexBuffer Vertices => vertexBuffer;
        public IndexBuffer Indices => indexBuffer;

        public int BlockSize => BLOCK_SIZE;
        public int MaxBlocks => (int)MathF.Floor((float)BUFFER_SIZE / BLOCK_SIZE);

        internal void Unload() {
            vertexBuffer?.Dispose();
            indexBuffer?.Dispose();
            vertexBuffer = null;
            indexBuffer = null;
        }

        private int VertexStride = VertexPositionColorTexture.VertexDeclaration.VertexStride;

        internal void Load(GraphicsDevice graphicsDevice) {
            vertexBuffer = new VertexBuffer(graphicsDevice,typeof(VertexPositionColorTexture),BUFFER_SIZE*VertexStride,BufferUsage.WriteOnly);
            indexBuffer = new IndexBuffer(graphicsDevice,IndexElementSize.SixteenBits,BUFFER_SIZE,BufferUsage.WriteOnly);
            ushort[] indices = new ushort[indexBuffer.IndexCount];
            for(ushort i = 0;i < indices.Length;i++) {
                indices[i] = i;
            }
            indexBuffer.SetData(indices);
        }

        public void SetVertices(int index,VertexPositionColorTexture[] data) {
            vertexBuffer.SetData(index*VertexStride,data,0,data.Length,VertexStride);
        }
    }

    [Serializable]
    public class SharedBufferException:Exception {
        public SharedBufferException() { }
        public SharedBufferException(string message) : base(message) { }
        public SharedBufferException(string message,Exception inner) : base(message,inner) { }
        protected SharedBufferException(SerializationInfo info,StreamingContext context) : base(info,context) { }
    }
}
