using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;

namespace TwelveEngine.PuzzleGame {
    public static class LevelFactory {
        public static GameState GenerateLevel(Level level) {

            var grid = new Grid2D(LayerModes.BackgroundForegroundStandard) {
                TileRenderer = new TilesetRenderer()
            };

            bool showCollisionLayer = false;
            if(showCollisionLayer) {
                grid.LayerMode.BackgroundLength += 1;
            }

            grid.PanZoom = false;
            grid.Camera.Scale = Constants.RenderScale;

            var map = MapDatabase.GetMap(level.MapName);
            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map);

            grid.OnLoad += () => {
                var player = new Player() {
                    X = level.PlayerX,
                    Y = level.PlayerY
                };
                grid.AddEntity(player);
            };

            var puzzleBoard = new PuzzleBoard(grid,level);

            return grid;
        }
    }
}
