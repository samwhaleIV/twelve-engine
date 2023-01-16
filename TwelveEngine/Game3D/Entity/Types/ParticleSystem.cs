using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public class ParticleSystem:Entity3D {

        protected readonly Vector3[] p_position;

        private Effect effect;

        private BufferSet bufferSet;

        public Texture2D Texture { get; set; }

        private readonly int particleCount;

        public int ParticleCount => particleCount;

        protected virtual void UpdatePositions() {
            float rows = MathF.Sqrt(p_position.Length);
            float rowSize = 1 / rows;
            for(int i = 0;i<particleCount;i++) {
                p_position[i] = new Vector3(i % rows * rowSize,MathF.Floor(i / rows) * rowSize,0f);
            }
        }

        public ParticleSystem(int count) {
            particleCount = count;
            p_position = new Vector3[count];

            OnRender += ParticleSystem_OnRender;
            OnLoad += ParticleSystem_OnLoad;
            OnUnload += ParticleSystem_OnUnload;
        }

        private void ParticleSystem_OnUnload() {
            effect?.Dispose();
            effect = null;
        }

        private const int SHAPE_VERTEX_COUNT = 6;

        private VertexPositionTexture[] vertices;

        public float ParticleSize { get; set; } = 0.025f;

        public VectorRectangle UVArea { get; set; }

        private void UpdateVertices() {
            float s = ParticleSize * 0.5f;

            Vector3 aOffset = new(s,s,0);
            Vector3 bOffset = new(s,-s,0);
            Vector3 cOffset = new(-s,s,0);
            Vector3 dOffset = new(-s,-s,0);

            VertexPositionTexture a, b, c, d;
            a.TextureCoordinate = UVArea.TopLeft;
            b.TextureCoordinate = UVArea.BottomLeft;
            c.TextureCoordinate = UVArea.TopRight;
            d.TextureCoordinate = UVArea.BottomRight;

            Vector3 position;
            int vertexIndex;

            for(int i = 0;i<particleCount;i+=1) {
                position = p_position[i];
                vertexIndex = i * SHAPE_VERTEX_COUNT;

                a.Position = position + aOffset;
                b.Position = position + bOffset;
                c.Position = position + cOffset;
                d.Position = position + dOffset;

                vertices[vertexIndex] = a;
                vertices[vertexIndex+1] = b;
                vertices[vertexIndex+2] = c;
                vertices[vertexIndex+3] = b;
                vertices[vertexIndex+4] = d;
                vertices[vertexIndex+5] = c;
            }
        }

        private EffectParameter worldViewProjectionMatrixParameter;
        private EffectParameter textureSamplerParameter;

        private void ParticleSystem_OnLoad() {
            effect = Game.Content.Load<Effect>("Shaders/ParticleSystemEffect");
            worldViewProjectionMatrixParameter = effect.Parameters["WorldViewProjection"];
            textureSamplerParameter = effect.Parameters["TextureSampler"];
            vertices = new VertexPositionTexture[particleCount * SHAPE_VERTEX_COUNT];
            bufferSet = BufferSet.Create<VertexPositionTexture>(Game.GraphicsDevice,vertices.Length);
        }

        public SamplerState SamplerState { get; set; } = null;

        private void ParticleSystem_OnRender() {
            Matrix scaleMatrix = Matrix.CreateScale(Scale);
            Matrix translationMatrix = Matrix.CreateWorld(Position + new Vector3(-0.5f,-0.5f,0),Vector3.Forward,Vector3.Up);
            worldViewProjectionMatrixParameter.SetValue(scaleMatrix * translationMatrix * Owner.ViewMatrix * Owner.ProjectionMatrix);
            textureSamplerParameter.SetValue(Texture);
            UpdatePositions();
            UpdateVertices();
            SamplerState oldSamplerState = effect.GraphicsDevice.SamplerStates[0];
            if(SamplerState != null) {
                effect.GraphicsDevice.SamplerStates[0] = SamplerState;
            }
            bufferSet.VertexBuffer.SetData(vertices);
            bufferSet.Apply();
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.TriangleList,0,0,vertices.Length);
            }
            effect.GraphicsDevice.SamplerStates[0] = oldSamplerState;
        }
    }
}
