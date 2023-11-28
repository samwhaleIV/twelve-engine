using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine.Game2D {
    public readonly struct TileMap {

        public readonly int Width { get; init; }
        public readonly int Height { get; init; }

        public readonly ushort[] Data { get; init; }

        public bool TryGetValue(int x,int y,out ushort value) {
            if(x < 0 || y < 0 || x >= Width || y >= Height) {
                value = 0;
                return false;
            }
            value = Data[y * Width + x];
            return true;
        }

        public ushort GetValue(int x,int y) {
            return Data[y * Width + x];
        }

        public void SetValue(int x,int y,ushort value) {
            Data[y * Width + x] = value;
        }

        public TileMap CreateCopy() {
            ushort[] data = new ushort[Data.Length];
            Data.CopyTo(data,0);
            return new TileMap() { Data = data, Width = Width, Height = Height };
        }
    }
}
