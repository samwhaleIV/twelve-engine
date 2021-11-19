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
            grid.PanZoom = true;

            grid.Camera.Scale = 10;
            grid.Camera.EdgePadding = false;
            grid.Camera.X = 0;
            grid.Camera.Y = 0;

            grid.OnLoad = () => {
                var size = 32;

                var width = size;
                var height = size;

                grid.Width = width;
                grid.Height = height;

                var map = new int[width,height];
                var tileRenderer = new TestTileRenderer();
                var layer1 = grid.CreateLayer(map,tileRenderer);
                tileRenderer.grid = layer1;

                grid.SetLayers(new GridLayer[] { layer1 });

                grid.AddEntity(new TheRedBox() {
                    X = 10,
                    Y = 10
                });
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
