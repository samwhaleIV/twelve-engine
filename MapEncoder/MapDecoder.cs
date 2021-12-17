using System;
using System.IO;

namespace TwelveEngine.Serial.Map {
    public sealed class MapDecoder {

        public MapDecoder(FileStream fileStream) {
            this.fileStream = fileStream;
        }

        private readonly FileStream fileStream;
        private BinaryReader reader;

        private void readUncompressed(int[] layer) {
            for(var i = 0;i<layer.Length;i++) {
                layer[i] = reader.ReadInt32();
            }
        }

        private void readTabulated(int[] layer) {
            int tableEntries = reader.ReadInt32();
            
            /* new int[]'s start with values at zero */

            for(int x = 0;x < tableEntries;x++) {

                int value = reader.ReadInt32();
                var columnSize = reader.ReadInt32();

                for(int y = 0;y<columnSize;y++) {

                    int index = reader.ReadInt32();
                    layer[index] = value;
                }
            }
        }

        private void readRunLength(int[] layer) {
            int i = 0;
            while(i < layer.Length) {

                int value = reader.ReadInt32();
                int stride = reader.ReadInt32();

                var strideEnd = i + stride;
                while(i < strideEnd) {

                    layer[i] = value;
                    i += 1;
                }
            }
        }

        private int[] getLayer(int size) {
            int[] layer = new int[size];
            LayerEncodingMode mode = (LayerEncodingMode)reader.ReadInt32();
            Action<int[]> decoder;
            switch(mode) {
                case LayerEncodingMode.Uncompressed:
                default:
                    decoder = readUncompressed;
                    break;
                case LayerEncodingMode.RunLength:
                    decoder = readRunLength;
                    break;
                case LayerEncodingMode.Table:
                    decoder = readTabulated;
                    break;
            }
            decoder.Invoke(layer);
            return layer;
        }

        private int[][] readLayers(int width,int height) {
            int layerCount = reader.ReadInt32();
            var layers = new int[layerCount][];

            var layerSize = width * height;

            for(int i = 0;i< layerCount;i++) {
                layers[i] = getLayer(layerSize);
            }

            return layers;
        }

        public void ReadDatabase(Action<string,Map> addMap) {
            using(reader = new BinaryReader(fileStream,Constants.MapStringEncoding,true)) {
                var mapCount = reader.ReadInt32();
                for(int i = 0;i<mapCount;i++) {
                    string name = reader.ReadString();

                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();

                    var layers = readLayers(width,height);

                    Map map = new Map(width,height,layers);
                    addMap.Invoke(name,map);
                }

            }
            reader = null;
        }
    }
}
