﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Game3D {
    public sealed class GridLines {

        private static readonly Color X_AXIS_COLOR = Color.Red;
        private static readonly Color Y_AXIS_COLOR = Color.GreenYellow;
        private static readonly Color Z_AXIS_COLOR = Color.Blue;
        private static readonly Color GRID_COLOR = Color.DarkGray;

        private float cellSize = 1f;
        private int gridSize = 40;

        public float CellSize {
            get => cellSize;
            set {
                if(cellSize == value) {
                    return;
                }
                cellSize = value;
                if(!loaded) {
                    return;
                }
                UpdateVertices();
            }
        }

        public int GridSize {
            get => gridSize;
            set {
                if(gridSize == value) {
                    return;
                }
                gridSize = value;
                if(!loaded) {
                    return;
                }
                UpdateVertices();
            }
        }

        private bool loaded = false;

        private BasicEffect effect;
        private World world;

        public void Load(World world) {
            effect = new BasicEffect(world.Game.GraphicsDevice) {
                VertexColorEnabled = true,
                TextureEnabled = false,
                Projection = world.ProjectionMatrix,
                View = world.ViewMatrix
            };
            UpdateVertices();
            this.world = world;
            world.OnViewMatrixChanged += World_OnViewMatrixChanged;
            world.OnProjectionMatrixChanged += World_OnProjectionMatrixChanged;
            loaded = true;
        }

        private void World_OnProjectionMatrixChanged(Matrix projectionMatrix) {
            effect.Projection = projectionMatrix;
        }
        private void World_OnViewMatrixChanged(Matrix viewMatrix) {
            effect.View = viewMatrix;
        }

        public void Unload() {
            world.OnViewMatrixChanged -= World_OnViewMatrixChanged;
            world.OnProjectionMatrixChanged -= World_OnProjectionMatrixChanged;
            world = null;
            effect?.Dispose();
        }

        private static VertexPositionColor GetVertexPosition(Vector3 position,Color color) {
            return new VertexPositionColor(position,color);
        }

        private VertexPositionColor[] vertices;

        private void UpdateVertices() {
            var lineCount = gridSize + 1;

            vertices = new VertexPositionColor[lineCount * 4 + 2];

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
        }

        public void Render() {
            effect.CurrentTechnique.Passes[0].Apply();
            effect.GraphicsDevice.DrawUserPrimitives(PrimitiveType.LineList,vertices,0,vertices.Length / 2);
        }
    }
}
