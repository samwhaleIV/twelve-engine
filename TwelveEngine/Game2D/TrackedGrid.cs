using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public class TrackedGrid {

        public readonly int Width;
        public readonly int Height;

        private readonly int[,] data;
        public TrackedGrid(int width,int height) {
            data = new int[width,height];
            Width = width;
            Height = height;
        }
        public TrackedGrid(int[,] data) {
            this.data = data;
            Width = data.GetLength(0);
            Height = data.GetLength(1);
        }

        public int[,] Data => data;

        public Action Invalidated { get; set; } = null;
        public Action<Rectangle> ValueChanged { get; set; } = null;

        private void updateGridValue(int x,int y,int value) {
            if(ValueChanged == null) {
                return;
            }
            data[x,y] = value;
            ValueChanged(new Rectangle(x,y,1,1));
        }

        public int this[int x,int y] {
            get => data[x,y];
            set => updateGridValue(x,y,value);
        }

        public void Fill(Func<int,int,int> pattern) {
            for(int x = 0;x < Width;x++) {
                for(int y = 0;y < Height;y++) {
                    data[x,y] = pattern(x,y);
                }
            }
            Invalidated?.Invoke();
        }
    }
}
