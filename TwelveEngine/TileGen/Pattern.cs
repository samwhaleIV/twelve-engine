using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.TileGen {

    public sealed class Pattern {

        public Pattern(bool[,] data) {
            int width = data.GetLength(0);
            int height = data.GetLength(1);

            if(width != height) {
                throw new ArgumentException("Pattern data must be square shaped.","data");
            }

            this.data = data;
            size = width;
            length = size * size;
        }

        private readonly int size;
        private readonly int length;

        public int Size => size;
        public int Length => length;

        private readonly bool[,] data;

        private static bool encodeColor(Color color) => color == Color.White;

        private static bool[,] getDataArray(CPUTexture texture,Point offset,int size) {
            var data = new bool[size,size];

            var textureData = texture.Data;

            for(int x = 0;x<size;x++) {
                for(int y = 0;y<size;y++) {
                    var color = textureData[x+offset.X,y+offset.Y];
                    data[x,y] = encodeColor(color);
                }
            }

            return data;
        }

        public bool GetValue(int x,int y) => data[x,y];
        public bool GetValue(Point location) => data[location.X,location.Y];

        internal void Draw(Color[] target,int targetWidth,Point location,PatternDrawSettings settings) {
            var pixelCount = size * size;

            var matrix = settings.GetMatrix(size);

            for(var i = 0;i<length;i++) {
                var selfLocation = matrix[i];
                bool value = GetValue(selfLocation);

                Color color = settings.Paint(value ^ settings.Polarity);
                if(color == Painter.NoFillColor) {
                    continue;
                }

                var targetLocation = new Point(i % size,i / size) + location;
                var targetIndex = targetLocation.X + targetLocation.Y * targetWidth;
                target[targetIndex] = color;
            }
        }

        public static PatternSet GetSet(CPUTexture texture,int patternSize) {
            var horizontalTiles = texture.Width / patternSize;
            var verticalTiles = texture.Height / patternSize;

            if(horizontalTiles != verticalTiles) {
                throw new ArgumentException("Invalid pattern texture. Texture must be square.","texture");
            }

            var patternCount = horizontalTiles * verticalTiles;
            var patterns = new Pattern[patternCount];

            int tileLength = horizontalTiles;

            for(int i = 0;i<patternCount;i++) {
                var offset = new Point(i % tileLength * patternSize,i / tileLength * patternSize);
                var data = getDataArray(texture,offset,patternSize);
                var pattern = new Pattern(data);
                patterns[i] = pattern;
            }
            return new PatternSet(patterns,patternSize);
        }

        public static PatternSet GetSet(string texture,int patternSize) => GetSet(CPUTexture.Get(texture),patternSize);
    }
}
