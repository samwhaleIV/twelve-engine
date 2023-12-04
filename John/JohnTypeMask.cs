using Microsoft.Xna.Framework;
using System;

namespace John {
    public readonly struct JohnTypeMask {
        public readonly JohnMatchType Type { get; init; }
        public readonly Color Color { get; init; }
        public readonly int ID { get; init; }

        public static JohnTypeMask GetRandom(Random random,int ID) {
            return new JohnTypeMask() {
                Color = JohnConfig.GetRandomColor(random),
                ID = ID,
                Type = (JohnMatchType)random.Next(0,3)
            };
        }
    }
}
