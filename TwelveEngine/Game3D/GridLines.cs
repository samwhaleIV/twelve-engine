using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D {
    public sealed class GridLines {

        private static readonly Color X_AXIS_COLOR = Color.Red;
        private static readonly Color Y_AXIS_COLOR = Color.GreenYellow;
        private static readonly Color Z_AXIS_COLOR = Color.Blue;

        private BasicEffect effect;
        private GraphicsDevice graphicsDevice;

        public void Load(GraphicsDevice graphicsDevice) {
            effect = new BasicEffect(graphicsDevice) {
                VertexColorEnabled = true,
                TextureEnabled = false
            };
            this.graphicsDevice = graphicsDevice;
        }
        public void Unload() {
            effect?.Dispose();
        }

        private static VertexPositionColor GetVertexPosition(Vector3 position,Color color) {
            return new VertexPositionColor(position,color);
        }

        private static VertexPositionColor[] GetVertices(Vector3 start,Vector3 end,Color color) {
            return new[] { GetVertexPosition(start,color), GetVertexPosition(end,color) };
        }

        private void UpdateEffect(Camera3D camera) {
            effect.View = camera.ViewMatrix;
            effect.Projection = camera.ProjectionMatrix;
        }

        private VertexPositionColor[] vertices;

        private void UpdateVertices(Camera3D camera) {

            var length = camera.FarPlane;
            var position = Vector3.Zero;

            var xAxis = GetVertices(
                new Vector3(-length + position.X,0,0),
                new Vector3(length + position.X,0,0),
                X_AXIS_COLOR
            );
            var yAxis = GetVertices(
                new Vector3(0,-length + position.Y,0),
                new Vector3(0,length + position.Y,0),
                Y_AXIS_COLOR
            );
            var zAxis = GetVertices(
                new Vector3(0,0,-length + position.Z),
                new Vector3(0,0,length + position.Z),
                Z_AXIS_COLOR
            );

            vertices = new VertexPositionColor[] {
                xAxis[0], xAxis[1],
                yAxis[0], yAxis[1],
                zAxis[0], zAxis[1]
            };
        }

        public void Update(Camera3D camera) {
            UpdateEffect(camera);
            UpdateVertices(camera);
        }

        public void Render() {
            effect.CurrentTechnique.Passes[0].Apply();
            graphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,vertices,0,vertices.Length / 2);
        }
    }
}
