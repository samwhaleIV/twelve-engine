using System;
using Microsoft.Xna.Framework;

namespace TwelveEngine.Game2D {
    public class TrackedGrid<T> {

        private int width;
        private int height;

        public int Width => width;
        public int Height => height;

        private readonly T[,] data;
        public TrackedGrid(int width,int height) {
            data = new T[width,height];
            this.width = width;
            this.height = height;
        }
        public TrackedGrid(T[,] data) {
            this.data = data;
            this.width = data.GetLength(0);
            this.height = data.GetLength(1);
        }

        public T[,] Data => data;

        public Action Invalidated { get; set; } = null;
        public Action<Rectangle> ValueChanged { get; set; } = null;

        private void updateGridValue(int x,int y,T value) {
            if(ValueChanged == null) {
                return;
            }
            data[x,y] = value;
            ValueChanged(new Rectangle(x,y,1,1));
        }

        public T this[int x,int y] {
            get => data[x,y];
            set => updateGridValue(x,y,value);
        }

        public void Fill(Func<int,int,T> pattern) {
            for(int x = 0;x < width;x++) {
                for(int y = 0;y < height;y++) {
                    data[x,y] = pattern(x,y);
                }
            }
            if(Invalidated != null) {
                Invalidated();
            }
        }
    }
}
