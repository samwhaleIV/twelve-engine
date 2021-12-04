using System;
using System.Collections.Generic;

namespace TwelveEngine.Game2D {
    public static class EntityFactory {

        private static readonly Dictionary<EntityType,Func<Entity>>
            typeLookup = new Dictionary<EntityType,Func<Entity>>();

        public static void SetType(EntityType type,Func<Entity> generator) {
            typeLookup[type] = generator;
        }

        public static Entity GetEntity(EntityType type) {
            var typeGenerator = typeLookup[type];
            Entity entity = typeGenerator();
            return entity;
        }
    }
}
