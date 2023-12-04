using Microsoft.Xna.Framework;
using System;

namespace John {
    public struct JohnConfig {
        public Color Color1 { get; init; }
        public Color Color2 { get; init; }
        public Color Color3 { get; init; }

        public static Color GetRandomColor(Random random) {
            return new Color((byte)random.Next(0,256),(byte)random.Next(0,256),(byte)random.Next(0,256),byte.MaxValue);
        }

        public static JohnConfig CreateRandom(Random random) => new JohnConfig() {
            Color1 = GetRandomColor(random), Color2 = GetRandomColor(random), Color3 = GetRandomColor(random)
        };

        public JohnConfig ApplyMask(JohnTypeMask mask) {
            return mask.Type switch {
                JohnMatchType.Hair => new JohnConfig() {
                    Color1 = mask.Color,
                    Color2 = Color2,
                    Color3 = Color3
                },
                JohnMatchType.Shirt => new JohnConfig() {
                    Color1 = Color1,
                    Color2 = mask.Color,
                    Color3 = Color3
                },
                JohnMatchType.Pants => new JohnConfig() {
                    Color1 = Color1,
                    Color2 = Color2,
                    Color3 = mask.Color
                },
                _ => throw new NotImplementedException()
            };
        }
    }
}
