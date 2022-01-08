using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.TileGen {
    internal readonly struct PatternDrawSettings {

        public PatternDrawSettings(bool polarity, Mirroring mirroring, Rotation rotation, Func<bool,Color> paint) {
            Polarity = polarity;
            Mirroring = mirroring;
            Rotation = rotation;
            Paint = paint;
        }

        internal Point[] GetMatrix(int size) {
            var matrixMode = new MatrixMode(size,Mirroring,Rotation);
            return PatternMatrix.Get(matrixMode);
        }

        public readonly Mirroring Mirroring;
        public readonly Rotation Rotation;

        public readonly bool Polarity;
        public readonly Func<bool,Color> Paint;
    }
}
