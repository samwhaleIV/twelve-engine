using System;
using System.Reflection;
using System.Collections.Generic;
using TwelveEngine.Serial.Map;
using TwelveEngine.Game2D;
using TwelveEngine.Game2D.Collision;
using TwelveEngine.Game2D.Entity.Types;
using TwelveEngine;

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

            var grid = new Grid2D(
                layerMode: LayerModes.BackgroundForegroundStandard,
                collisionTypes: new CollisionTypes(Tiles.CollisionArea,Tiles.CollisionColor)
            ) {
                TileRenderer = new TilesetRenderer()
            };

            var factory = new ComponentFactory(grid);
            Level level = levelSelector.Invoke(factory);

            grid.PanZoom = false;

            var map = getMap(level);

            grid.ImportMap(map);
            grid.LayerMode = LayerModes.GetAutomatic(map.Layers.Length);

            bool showCollisionLayer = Constants.Config.ShowCollision;
            if(showCollisionLayer) {
                grid.LayerMode.BackgroundLength += 1;
            }

            grid.OnLoad += () => {
                var player = new Player() {
                    X = level.Player.X,
                    Y = level.Player.Y,
                    Name = "Player",
                    StateLock = false
                };
                grid.ImpulseGuide.SetDescriptions(
                    (Impulse.Up, Strings.MoveUp),
                    (Impulse.Down, Strings.MoveDown),
                    (Impulse.Left, Strings.MoveLeft),
                    (Impulse.Right, Strings.MoveRight),
                    (Impulse.Accept, Strings.Interact)
                );
                grid.AddEntity(player);
            };

            level.Components();
            Component[] components = factory.Export();

            var puzzleManager = new PuzzleManager(grid,components);
            return grid;
        }
    }
}
