using System;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TwelveEngine {
    public static class Program {

        private static GameState GetTestStartState() {
            var grid = new Grid2D(LayerModes.SingleLayerBackground);

            grid.Camera.Scale = 8;
            grid.Camera.EdgePadding = false;
            grid.Camera.X = 0;
            grid.Camera.Y = 0;

            grid.OnLoad = () => {
                var width = 48;
                var height = 48;

                grid.Width = width;
                grid.Height = height;

                var map = new int[width,height];
                var tileGrid = new TrackedGrid(map);
                var tileRenderer = new TestTileRenderer(tileGrid);
                var layer1 = grid.CreateLayer(tileGrid,tileRenderer);
                grid.SetLayers(new GridLayer[] { layer1 });

                grid.AddEntity(new TheRedBox() {
                    X = 1,
                    Y = 1
                });

                grid.UpdateTarget = new PanZoom(grid);
            };

            return grid;
        }

        public static GameManager GetStartGame() {
            DefaultEntitiesList.Install();
            var startState = GetTestStartState();
            return new GameManager(startState);
        }
        public static void ConfigStartGame(GameManager game) {
            game.GameState = GetTestStartState();
        }
    }
}
