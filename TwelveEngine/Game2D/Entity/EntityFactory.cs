using System;
using System.Reflection;
using System.Collections.Generic;

namespace TwelveEngine.Game2D {
    public static class EntityFactory {

        private static readonly Dictionary<EntityType,ConstructorInfo>
            typeConstructors = new Dictionary<EntityType,ConstructorInfo>();

        public static void SetType(EntityType entityType,Type type) {
            typeConstructors[entityType] = type.GetConstructor(new Type[0]);
        }

        public static bool ContainsType(EntityType type) {
            return typeConstructors.ContainsKey(type);
        }

        public static Entity GetEntity(EntityType type) {
            return (Entity)typeConstructors[type].Invoke(null);
        }
    }
}
