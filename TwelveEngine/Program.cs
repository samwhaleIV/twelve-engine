using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;

namespace TwelveEngine {
    public static class Program {

        public static GameState GetTestStartState() {

            bool showCollisionLayer = true;

            MapDatabase.LoadMaps();
            var grid = new Grid2D(LayerModes.BackgroundForegroundStandard) {
                TileRenderer = new TilesetRenderer()
            };
            var map = MapDatabase.GetMap("level4");
            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map);
            if(showCollisionLayer) {
                grid.LayerMode.BackgroundLength += 1;
            }

            grid.PanZoom = true;

            grid.Camera.Scale = 8;
            grid.Camera.EdgePadding = false;
            grid.Camera.X = 0;
            grid.Camera.Y = 0;

            grid.OnLoad += () => {
                var player = new Player() {
                    X = 7.5f,
                    Y = 4.5f
                };
                grid.AddEntity(player);
            };

            return grid;
        }

        public static GameManager GetStartGame() {
            DefaultEntitiesList.Install();
            var startState = GetTestStartState();
            return new GameManager(startState);
        }
        public static void ConfigStartGame(GameManager game) {
            game.SetGameState(GetTestStartState());
        }
    }
}
