using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using TiledCS;

namespace TiledCompiler {
    class Program {

        const string outputFile = "map-database.json";

        public struct Map {
            public int Width;
            public int Height;
            public int[][] Layers;
        }

        static void mergeBottomTwoLayers(List<TiledLayer> layers) {
            var layer1 = layers[0];
            var layer2 = layers[1];

            var layer1Data = layer1.data;
            var layer2Data = layer2.data;
            for(var i = 0;i < layer1Data.Length;i++) {
                var layer2Value = layer2Data[i];
                if(layer2Value != 0) {
                    layer1Data[i] = layer2Value;
                }
            }
            layers.RemoveAt(1);
        }

        static Map convertMap(TiledMap tiledMap) {
            var map = new Map();
            map.Width = tiledMap.Width;
            map.Height = tiledMap.Height;

            var inputLayers = new List<TiledLayer>(tiledMap.Layers);
            mergeBottomTwoLayers(inputLayers);

            int[][] layers = new int[inputLayers.Count][];

            for(var i = 0;i<inputLayers.Count;i++) {
                layers[i] = inputLayers[i].data;
            }

            map.Layers = layers;

            return map;
        }

        static void Main() {

            string rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),@"..\..\..\..\"));
            string inputFolder = Path.Combine(rootPath,"Tiled");
            string outputPath = Path.Combine(rootPath,"TwelveEngine","Content",outputFile);

            string[] inputFolders = Directory.GetDirectories(inputFolder);

            List<string> files = new List<string>();

            foreach(var folder in inputFolders) {
                files.AddRange(Directory.GetFiles(folder,"*.tmx"));
            }

            Dictionary<string,Map> maps = new Dictionary<string,Map>();

            foreach(var file in files) {
                var map = new TiledMap(file);
                Console.WriteLine($"Processed '{file}'");
                maps[Path.GetFileNameWithoutExtension(file)] = convertMap(map);
            }

            string jsonData = JsonConvert.SerializeObject(maps,Formatting.None);
            File.WriteAllText(outputPath,jsonData);
        }
    }
}
