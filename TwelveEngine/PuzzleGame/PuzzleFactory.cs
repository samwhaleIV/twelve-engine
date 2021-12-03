using System;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;

namespace TwelveEngine.PuzzleGame {
    public static class PuzzleFactory {
        public static GameState GenerateState(Func<ComponentFactory,Level> levelSelector) {

            var grid = new Grid2D(LayerModes.BackgroundForegroundStandard) {
                TileRenderer = new TilesetRenderer()
            };

            var factory = new ComponentFactory(grid);
            Level level = levelSelector.Invoke(factory);

            bool showCollisionLayer = Constants.ShowCollisionLayer;
            if(showCollisionLayer) {
                grid.LayerMode.BackgroundLength += 1;
            }

            grid.PanZoom = false;
            grid.Camera.Scale = Constants.RenderScale;

            var map = MapDatabase.GetMap(level.Map);
            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map);

            grid.OnLoad += () => {
                grid.EntityManager.PauseListChanges();
                var player = new Player() {
                    X = level.Player.X,
                    Y = level.Player.Y,
                    Name = "Player",
                    StateLock = false
                };
                grid.AddEntity(player);
                grid.EntityManager.ResumeListChanges();
            };

            level.Components();
            Component[] components = factory.Export();

            var puzzleManager = new PuzzleManager(grid,components);
            return grid;
        }
    }
}
