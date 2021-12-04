using System;
using System.Collections.Generic;

namespace TwelveEngine.Game2D {
    public static partial class EntityFactory {

        private static readonly Dictionary<EntityType,Func<Entity>>
            generators = new Dictionary<EntityType,Func<Entity>>();

        public static void SetType(EntityType entityType,Func<Entity> generator) {
            generators[entityType] = generator;
        }

        public static bool ContainsType(EntityType type) {
            return generators.ContainsKey(type);
        }

        public static Entity GetEntity(EntityType type) {
            return generators[type].Invoke();
        }
    }
}
