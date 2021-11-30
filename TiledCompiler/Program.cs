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

        static void offsetTileValues(TiledLayer layer,int amount) {
            var data = layer.data;
            for(var i = 0;i < data.Length;i++) {
                data[i] = data[i] + amount;
            }
        }

        static Map convertMap(TiledMap tiledMap) {
            var map = new Map();
            map.Width = tiledMap.Width;
            map.Height = tiledMap.Height;

            var inputLayers = new List<TiledLayer>(tiledMap.Layers);
            mergeBottomTwoLayers(inputLayers);

            int[][] layers = new int[inputLayers.Count][];

            for(var i = 0;i<inputLayers.Count;i++) {
                var layer = inputLayers[i];
                offsetTileValues(layer,-1);
                layers[i] = layer.data;
            }

            map.Layers = layers;

            return map;
        }

        static void Main() {

            string rootPath = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(),@"..\..\..\..\"));
            string inputFolder = Path.Combine(rootPath,"Tiled");
            string outputPath = Path.Combine(rootPath,"TwelveEngine","Content",outputFile);

            string[] inputFolders = Directory.GetDirectories(inputFolder,"*",SearchOption.AllDirectories);

            List<string> files = new List<string>();

            foreach(var folder in inputFolders) {
                var path = folder.Split(Path.DirectorySeparatorChar);
                if(path[path.Length-1] == "rules") {
                    continue;
                }
                files.AddRange(Directory.GetFiles(folder,"*.tmx"));
            }

            Dictionary<string,Map> maps = new Dictionary<string,Map>();

            foreach(var file in files) {

                var path = file.Split(Path.DirectorySeparatorChar);
                var start = Array.IndexOf(path,"Tiled") + 1;
                var folder = string.Join('/',path,start,path.Length-start-1);

                var map = new TiledMap(file);
                if(map.Layers.Length < 4) {
                    Console.WriteLine($"Skipped '{file}'");
                    continue;
                }

                maps[folder + "/" + Path.GetFileNameWithoutExtension(file)] = convertMap(map);

                Console.WriteLine($"Processed '{file}'");
            }

            string jsonData = JsonConvert.SerializeObject(maps,Formatting.None);
            File.WriteAllText(outputPath,jsonData);
        }
    }
}
