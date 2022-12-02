using System;
using System.IO;
using System.Collections.Generic;

namespace TwelveEngine.Game2D.MapFormat {
    public sealed class MapEncoder {

        public MapEncoder(FileStream fileStream) {
            this.fileStream = fileStream;
        }

        private readonly FileStream fileStream;
        private BinaryWriter writer;

        private int countEncodingBytes(Action<int[]> encoder,int[] layer) {
            var memoryStream = new MemoryStream();

            var startWriter = writer;

            using(var writer = new BinaryWriter(memoryStream,Constants.MapStringEncoding,true)) {
                this.writer = writer;
                encoder.Invoke(layer);
            }

            writer = startWriter;

            /* .Capacity is the size of the memory block, .Length is the number of bytes written */
            int size = (int)memoryStream.Length;

            /* MemoryStream doesn't require disposal but implements 'IDisposable' anyway */
            memoryStream.Dispose(); 

            return size;
        }

        private LayerEncodingMode getBestEncoding(int[] layer) {
            /* Not the fastest strategy in the world, but MapEncoder is not intended for use during Game client runtime, per say */

            var sizes = new Dictionary<LayerEncodingMode,int>() {

                { LayerEncodingMode.Uncompressed, sizeof(int) * layer.Length },

                { LayerEncodingMode.RunLength, countEncodingBytes(writeRunLength,layer) },

                { LayerEncodingMode.Table, countEncodingBytes(writeTabulated,layer) }

            };

            (LayerEncodingMode mode, int size) smallestEncoding = (
                LayerEncodingMode.Uncompressed, int.MaxValue
            );

            foreach(var set in sizes) {
                LayerEncodingMode mode = set.Key;
                int size = set.Value;

                if(size > smallestEncoding.size) {
                    continue;
                }
                smallestEncoding = (mode, size);
            }

            return smallestEncoding.mode;
        }

        private void writeUncompressed(int[] layer) {
            for(var i = 0;i<layer.Length;i++) {
                writer.Write(layer[i]);
            }
        }

        private Dictionary<int,Queue<int>> generateTable(int[] layer) {
            var table = new Dictionary<int,Queue<int>>();

            for(int i = 0;i<layer.Length;i++) {
                int value = layer[i];

                /* MapDecoder initializes values to 0 */
                if(value == 0) {
                    continue;
                }

                Queue<int> queue;
                if(table.ContainsKey(value)) {
                    queue = table[value];
                } else {
                    queue = new Queue<int>();
                    table[value] = queue;
                }
                queue.Enqueue(i);
            }

            return table;
        }

        private void writeTabulated(int[] layer) {

            var table = generateTable(layer);

            writer.Write(table.Count);

            foreach(var set in table) {
                int value = set.Key;
                Queue<int> queue = set.Value;

                writer.Write(value);
                writer.Write(queue.Count);

                foreach(var index in queue) {
                    writer.Write(index);
                }
            }
        }

        private void writeRunLength(int[] layer) {
            int value = layer[0];
            int runLength = 1;

            for(var i = 1;i<layer.Length;i++) {
                int layerValue = layer[i];
                if(layerValue == value) {
                    runLength++;
                } else {
                    writer.Write(value);
                    writer.Write(runLength);

                    value = layerValue;
                    runLength = 1;
                }
            }
            writer.Write(value);
            writer.Write(runLength);
        }

        private void writeLayer(int[] layer) {
            var encoding = getBestEncoding(layer);
            Action<int[]> encoder;
            writer.Write((int)encoding);
            switch(encoding) {
                case LayerEncodingMode.Uncompressed: default:
                    encoder = writeUncompressed;
                    break;
                case LayerEncodingMode.Table:
                    encoder = writeTabulated;
                    break;
                case LayerEncodingMode.RunLength:
                    encoder = writeRunLength;
                    break;
            }
            encoder.Invoke(layer);
        }
       
        private void writeMap(string name,Map map) {

            writer.Write(name);
            writer.Write(map.Width);
            writer.Write(map.Height);

            var layerCount = map.Layers.Length;
            writer.Write(layerCount);

            for(int i = 0;i<layerCount;i++) {
                writeLayer(map.Layers[i]);
            }
        }

        public void WriteDatabase(Dictionary<string,Map> database) {
            using(writer = new BinaryWriter(fileStream,Constants.MapStringEncoding,true)) {
                writer.Write(database.Count);
                foreach(var set in database) {
                    string name = set.Key;
                    Map map = set.Value;
                    writeMap(name,map);
                }
            }
            writer = null;
        }
    }
}
