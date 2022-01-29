using TwelveEngine.Serial;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D.Entity.Types {
    public sealed class GridLinesEntity:Entity3D {
        
        /* Doesn't support Entity3D grid transformations! */

        private static readonly Color X_AXIS_COLOR = Color.Red;
        private static readonly Color Y_AXIS_COLOR = Color.GreenYellow;
        private static readonly Color Z_AXIS_COLOR = Color.Blue;
        private static readonly Color GRID_COLOR = Color.DarkGray;

        protected override int GetEntityType() => Entity3DType.GridLines;

        private float cellSize;
        private int gridSize;

        public GridLinesEntity(float cellSize = 1f,int gridSize = 40) {
            this.cellSize = cellSize;
            this.gridSize = gridSize;

            OnLoad += GridLinesEntity_OnLoad;
            OnUnload += GridLinesEntity_OnUnload;

            OnRender += GridLinesEntity_OnRender;
            OnImport += GridLinesEntity_OnImport;
            OnExport += GridLinesEntity_OnExport;
        }

        private BufferSet bufferSet;

        private void GridLinesEntity_OnLoad() {
            bufferSet = Owner.CreateBufferSet(GetVertices(cellSize,gridSize));
        }

        private void GridLinesEntity_OnUnload() {
            bufferSet?.Dispose();
            bufferSet = null;
        }

        private void GridLinesEntity_OnExport(SerialFrame frame) {
            frame.Set(cellSize);
            frame.Set(gridSize);
        }

        private void GridLinesEntity_OnImport(SerialFrame frame) {
            cellSize = frame.GetFloat();
            gridSize = frame.GetInt();
        }

        private void GridLinesEntity_OnRender(GameTime gameTime) {
            var effect =  Owner.BasicEffect;
            effect.World = Matrix.Identity;
            effect.TextureEnabled = false;
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
                    xColor = X_AXIS_COLOR;
                    yColor = Y_AXIS_COLOR;
                }

                vertices[index++] = GetVertexPosition(new Vector3(axisStart,gridStart,0),xColor);
                vertices[index++] = GetVertexPosition(new Vector3(axisStart,gridStart+length,0),xColor);


                vertices[index++] = GetVertexPosition(new Vector3(gridStart,axisStart,0),yColor);
                vertices[index++] = GetVertexPosition(new Vector3(gridStart+length,axisStart,0),yColor);
            }

            var halfLength = length * 0.5f;
            vertices[index++] = GetVertexPosition(new Vector3(0,0,-halfLength),Z_AXIS_COLOR);
            vertices[index++] = GetVertexPosition(new Vector3(0,0,halfLength),Z_AXIS_COLOR);

            return vertices;
        }
    }
}
