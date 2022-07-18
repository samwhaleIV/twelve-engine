using System;
using System.Reflection;
using System.Collections.Generic;
using TwelveEngine.Serial.Map;
using TwelveEngine.Game2D;
using TwelveEngine;
using TwelveEngine.Shell;
using TwelveEngine.EntitySystem;
using Porthole.PuzzleGame.Entity;
using TwelveEngine.Game2D.Entity;
using Porthole.Collision;

namespace Porthole.PuzzleGame {
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

        private static Map getMap(Level level) {
            Map? map = MapDatabase.GetMap(level.Map);
            if(!map.HasValue) {
                throw new NullReferenceException($"Level map '{level.Map}' does not exist in the map database!");
            }
            return map.Value;
        }

        public static GameState GenerateState(Func<ComponentFactory,Level> levelSelector) {

            var grid = new PuzzleGrid();

            grid.EntityFactory = new EntityFactory<Entity2D,Grid2D>(
                (1, () => new Player())
            );


            grid.CollisionTypes = new TileCollisionTypes(Tiles.CollisionArea, Tiles.CollisionColor);
            grid.TileRenderer = new TilesetRenderer();

            var factory = new ComponentFactory(grid);
            Level level = levelSelector.Invoke(factory);

            var map = getMap(level);

            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map.Layers.Length);

            grid.Camera.HorizontalPadding = level.CameraPaddingX;
            grid.Camera.VerticalPadding = level.CameraPaddingY;

            bool showCollisionLayer = Constants.Config.ShowCollision;

            if(showCollisionLayer) {
                LayerMode layerMode = grid.LayerMode;
                layerMode.BackgroundLength += 1;
                grid.LayerMode = layerMode;
            }

            grid.InputGuide.SetDescriptions(
                (Impulse.Up, Strings.MoveUp),
                (Impulse.Down, Strings.MoveDown),
                (Impulse.Left, Strings.MoveLeft),
                (Impulse.Right, Strings.MoveRight),
                (Impulse.Accept, Strings.Interact)
            );

            grid.OnLoad += () => {

                var player = new Player() {
                    X = level.Player.X,
                    Y = level.Player.Y,
                    Name = "Player",
                    StateLock = false
                };

                grid.Entities.Add(player);
            };

            level.Components();
            Component[] components = factory.Export();

            var puzzleManager = new PuzzleManager(grid,components);
            return grid;
        }
    }
}
