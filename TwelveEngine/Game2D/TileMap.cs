using System.Text.Json;

namespace TwelveEngine.Game2D {
    public readonly struct TileMap {

        public readonly int Width { get; init; }
        public readonly int Height { get; init; }

        public readonly short[,] Data { get; init; }

        public readonly int LayerCount { get; init; }

        public bool InRange(int x,int y) {
            return !(x < 0 || y < 0 || x >= Width || y >= Height);
        }

        public bool InRange(Point point) {
            return !(point.X < 0 || point.Y < 0 || point.X >= Width || point.Y >= Height);
        }

        public short GetValue(int x,int y,int layer = 0) {
            return Data[layer,y * Width + x];
        }

        public short GetValue(Point point,int layer = 0) {
            return Data[layer,point.Y * Width + point.X];
        }

        public bool TryGetValue(Point point,out short value,int layer = 0) {
            if(!InRange(point)) {
                value = 0;
                return false;
            }
            value = Data[layer,point.Y * Width + point.X];
            return true;
        }

        public void SetValue(int x,int y,short value,int layer = 0) {
            Data[layer,y * Width + x] = value;
        }

        public TileMap CreateCopy() {
            short[,] data = new short[Data.GetLength(0),Data.GetLength(1)];
            Data.CopyTo(data,0);
            return new TileMap() { Data = data, Width = Width, Height = Height };
        }

        public static TileMap CreateFromCSV(string file) {
            string[] lines = File.ReadAllLines(file);
            int height = lines.Length;
            int width = 1;
            string firstLine = lines[0];
            foreach(char character in firstLine) {
                if(character == ',') width++;
            }

            /* Single layer */
            short[,] tileData = new short[1,width * height];

            TileMap tileMap = new() { Width = width,Height = height,Data = tileData, LayerCount = 1 };

            int y = 0;

            foreach(var line in lines) {
                string[] splitLine = line.Split(',',StringSplitOptions.TrimEntries);
                if(splitLine.Length != width) {
                    throw new IndexOutOfRangeException("Row does not contain the correct quantity of values.");
                }
                for(int x = 0;x<width;x++) {
                    if(!short.TryParse(splitLine[x],out short tileValue)) {
                        continue;
                    }
                    tileMap.SetValue(x,y,tileValue);
                }
                y++;
            }

            return tileMap;
        }

        private class TiledJsonSchemaLayer {
            public short[] data { get; set; }
        }

        private class TiledJSONSchema {
            public int width { get; set; }
            public int height { get; set; }
            public TiledJsonSchemaLayer[] layers { get; set; }
        }

        public static TileMap CreateFromJSON(string file,short tileValueOffset = -1) {

            string text;
            try {
                text = File.ReadAllText(file);
            } catch(Exception exception) {
                Logger.WriteLine($"Could not open JSON map file '{file}'. {exception.Message}",LoggerLabel.None);
                throw;
            }

            var mapData = JsonSerializer.Deserialize<TiledJSONSchema>(text);

            short[,] tileData = new short[mapData.layers.Length,mapData.width * mapData.height];

            for(int i = 0;i<mapData.layers.Length;i++) {
                short[] layer = mapData.layers[i].data;
                for(int j = 0;j<layer.Length;j++) {
                    tileData[i,j] = (short)(layer[j] + tileValueOffset);
                }
            }

            return new TileMap() { Width = mapData.width, Height = mapData.height, Data = tileData, LayerCount = mapData.layers.Length };
        }
    }
}
