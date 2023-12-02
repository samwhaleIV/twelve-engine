using Microsoft.Xna.Framework;
using System;

namespace John {
    public readonly struct JohnConfig {
        public readonly Color Color1 { get; init; }
        public readonly Color Color2 { get; init; }
        public readonly Color Color3 { get; init; }

        private static Color GetRandomColor(Random random) {
            return new Color((byte)random.Next(0,256),(byte)random.Next(0,256),(byte)random.Next(0,256),byte.MaxValue);
        }

        public static JohnConfig CreateRandom(Random random) => new JohnConfig() {
            Color1 = GetRandomColor(random), Color2 = GetRandomColor(random), Color3 = GetRandomColor(random)
        };
    }
}
