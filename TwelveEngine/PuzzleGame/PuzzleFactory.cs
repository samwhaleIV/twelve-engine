﻿using System;
using System.Reflection;
using System.Collections.Generic;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Entities;
using static TwelveEngine.Serial.MapDatabase;

namespace TwelveEngine.PuzzleGame {
    public static class PuzzleFactory {

        private static readonly Dictionary<string,MethodInfo> generatorMethods;

        private static readonly Type levelGenerator = typeof(ComponentFactory);
        private static readonly Type levelType = typeof(Level);

        static PuzzleFactory() {
            generatorMethods = new Dictionary<string,MethodInfo>();
            foreach(var method in levelGenerator.GetMethods()) {
                if(method.ReturnType != levelType) {
                    continue;
                }
                generatorMethods[method.Name] = method;
            }
        }

        public static GameState GetLevel(string levelName) {
            
            if(!generatorMethods.ContainsKey(levelName)) {
                throw new MissingMethodException(levelGenerator.Name,levelName);
            }

            var method = generatorMethods[levelName];
            
            return GenerateState(factory => (Level)method.Invoke(factory,null));
        }
        public static GameState GenerateState(Func<ComponentFactory,Level> levelSelector) {

            var grid = new Grid2D(layerMode: LayerModes.BackgroundForegroundStandard) {
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

            var map = GetMap(level.Map);
            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map);

            grid.OnLoad += () => {
                var player = new Player() {
                    X = level.Player.X,
                    Y = level.Player.Y,
                    Name = "Player",
                    StateLock = false
                };
                grid.AddEntity(player);
            };

            level.Components();
            Component[] components = factory.Export();

            var puzzleManager = new PuzzleManager(grid,components);
            return grid;
        }
    }
}
