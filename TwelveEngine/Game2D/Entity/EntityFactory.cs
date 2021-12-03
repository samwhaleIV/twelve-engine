using System;
using System.Collections.Generic;

namespace TwelveEngine.Game2D {
    public static class EntityFactory {
        private static readonly Dictionary<string,Func<Entity>> typeLookup = new Dictionary<string,Func<Entity>>();
        public static void SetType(string factoryID,Func<Entity> generator) {
            typeLookup[factoryID] = generator;
        }
        public static Entity GetEntity(string factoryID) {
            var typeGenerator = typeLookup[factoryID];
            Entity entity = typeGenerator();
            return entity;
        }
    }
}
