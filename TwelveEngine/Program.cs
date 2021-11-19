﻿using System;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;
using System.Threading;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TwelveEngine {
    public static class Program {

        private static GameState GetTestStartState() {

            MapDatabase.LoadMaps();
            var grid = new Grid2D(LayerModes.BackgroundForegroundStandard) {
                TileRenderer = new TilesetRenderer()
            };
            var map = MapDatabase.GetMap("level4");
            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map);

            grid.PanZoom = true;

            grid.Camera.Scale = 8;
            grid.Camera.EdgePadding = false;
            grid.Camera.X = 0;
            grid.Camera.Y = 0;

            grid.OnLoad = () => {
                grid.AddEntity(new Player());
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
