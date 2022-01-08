using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.TileGen {
    public struct PatternDrawSettings {
        public Mirroring Mirroring;
        public Rotation Rotation;

        internal Point[] GetMatrix(int size) {
            var matrixMode = new MatrixMode(size,Mirroring,Rotation);
            return PatternMatrix.Get(matrixMode);
        }

        public bool Polarity { get; set; }
        public Func<bool,Color> Paint;
    }
}
