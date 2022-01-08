using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TwelveEngine.TileGen {
    internal static class PatternMatrix {
        private static readonly Dictionary<MatrixMode,Point[]> matrices = new Dictionary<MatrixMode,Point[]>();

        private static T[] flattenMatrix<T>(T[,] points,int size,bool rotate,bool reverse) {
            int length = size * size;
            var matrix = new T[length];

            reverse ^= rotate;

            for(int i = 0;i<length;i++) {
                int x = i % size, y = i / size;

                if(rotate) {
                    /* 'z' = Swap variable */
                    int z = x; x = y; y = z; 
                }

                matrix[reverse ? length - i - 1 : i] = points[x,y];
            }

            return matrix;
        }

        private static T[] rotatePoints<T>(T[,] points,int size,Rotation rotation) {
            bool rotate = false, reverse = false;
            switch(rotation) {
                case Rotation.One: rotate = true; break;
                case Rotation.Two: reverse = true; break;
                case Rotation.Three: rotate = true; reverse = true; break;
            }
            return flattenMatrix(points,size,rotate,reverse);
        }

        private static Point[,] getPoints(int size,Mirroring mirroring) {
            var points = new Point[size,size];

            bool mirrorX = mirroring == Mirroring.X || mirroring == Mirroring.XY;
            bool mirrorY = mirroring == Mirroring.Y || mirroring == Mirroring.XY;

            int length = size * size;

            for(int i = 0;i<length;i++) {
                int x = i % size, y = i / size;

                int targetX = x, targetY = y;

                if(mirrorX) targetX = size - x - 1;
                if(mirrorY) targetY = size - y - 1;

                points[x,y] = new Point(targetX,targetY);
            }

            return points;
        }

        private static Point[] generateMatrix(MatrixMode mode) {
            return rotatePoints(getPoints(mode.Size,mode.Mirroring),mode.Size,mode.Rotation);
        }

        public static Point[] Get(MatrixMode mode) {
            if(matrices.ContainsKey(mode)) {
                return matrices[mode];
            }

            var matrix = generateMatrix(mode);
            matrices.Add(mode, matrix);

            return matrix;
        }
    }
}
