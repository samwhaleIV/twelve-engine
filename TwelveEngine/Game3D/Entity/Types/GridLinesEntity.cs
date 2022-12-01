using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class GridLinesEntity:WorldMatrixEntity {
        
        private static readonly Color X_AXIS_COLOR = Color.Red;
        private static readonly Color Y_AXIS_COLOR = Color.GreenYellow;
        private static readonly Color Z_AXIS_COLOR = Color.Blue;
        private static readonly Color GRID_COLOR = Color.DarkGray;

        private readonly float cellSize;
        private readonly int gridSize;

        private BufferSet bufferSet;
        private BasicEffect effect;

        public GridLinesEntity(float cellSize = 1f,int gridSize = 40) {
            this.cellSize = cellSize;
            this.gridSize = gridSize;

            OnLoad += GridLinesEntity_OnLoad;
            OnUnload += GridLinesEntity_OnUnload;
            OnUpdate += GridLinesEntity_OnUpdate;
            OnRender += GridLinesEntity_OnRender;
        }

        private void GridLinesEntity_OnUpdate(GameTime gameTime) {
            UpdateWorldMatrix(worldMatrix => effect.World = worldMatrix);
        }
        private void UpdateEffectViewMatrix(Matrix viewMatrix) {
            effect.View = viewMatrix;
        }
        private void UpdateEffectProjectionMatrix(Matrix projectionMatrix) {
            effect.Projection = projectionMatrix;
        }

        private void GridLinesEntity_OnLoad() {
            bufferSet = Owner.CreateBufferSet(GetVertices(cellSize,gridSize));

            effect = new BasicEffect(Game.GraphicsDevice) {
                TextureEnabled = false,
                LightingEnabled = false,
                VertexColorEnabled = true
            };

            UpdateEffectProjectionMatrix(Owner.ProjectionMatrix);
            UpdateEffectViewMatrix(Owner.ViewMatrix);

            Owner.OnProjectionMatrixChanged += UpdateEffectProjectionMatrix;
            Owner.OnViewMatrixChanged += UpdateEffectViewMatrix;
        }

        
        private void GridLinesEntity_OnUnload() {
            Owner.OnProjectionMatrixChanged -= UpdateEffectProjectionMatrix;
            Owner.OnViewMatrixChanged -= UpdateEffectViewMatrix;

            effect?.Dispose();
            effect = null;

            bufferSet?.Dispose();
            bufferSet = null;
        }

        private void GridLinesEntity_OnRender(GameTime gameTime) {
            bufferSet.Apply();
            foreach(var pass in effect.CurrentTechnique.Passes) {
                pass.Apply();
                effect.GraphicsDevice.DrawIndexedPrimitives(PrimitiveType.LineList,0,0,bufferSet.VertexCount / 2);
            }
        }

        private static VertexPositionColor GetVertexPosition(Vector3 position,Color color) {
            return new VertexPositionColor(position,color);
        }

        private static VertexPositionColor[] GetVertices(float cellSize,int gridSize) {
            var lineCount = gridSize + 1;

            var vertices = new VertexPositionColor[lineCount * 4 + 2];

            var length = gridSize * cellSize;
            var gridStart = gridSize * -0.5f;

            int index = 0;

            for(int i = 0;i<lineCount;i++) {
                var axisStart = gridStart + i * cellSize;
                Color xColor = GRID_COLOR, yColor = GRID_COLOR;

                if(axisStart == 0) {
                    xColor = Z_AXIS_COLOR;
                    yColor = X_AXIS_COLOR;
                }

                vertices[index++] = GetVertexPosition(new Vector3(axisStart,0,gridStart),xColor);
                vertices[index++] = GetVertexPosition(new Vector3(axisStart,0,gridStart+length),xColor);


                vertices[index++] = GetVertexPosition(new Vector3(gridStart,0,axisStart),yColor);
                vertices[index++] = GetVertexPosition(new Vector3(gridStart+length,0,axisStart),yColor);
            }

            var halfLength = length * 0.5f;
            vertices[index++] = GetVertexPosition(new Vector3(0,-halfLength,0),Y_AXIS_COLOR);
            vertices[index++] = GetVertexPosition(new Vector3(0,halfLength,0),Y_AXIS_COLOR);

            return vertices;
        }
    }
}
