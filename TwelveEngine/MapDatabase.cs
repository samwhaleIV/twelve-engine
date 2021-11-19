using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace TwelveEngine {
    public struct Map {
        public int Width;
        public int Height;
        public int[][] Layers;
    }
    public static class MapDatabase {
        private static Dictionary<string,Map> maps = null;
        public static Map GetMap(string name) {
            if(maps == null) {
                return new Map();
            }
            var map = maps[name];
            return map;
        }
        public static void LoadMaps() {
            string jsonData = File.ReadAllText(Constants.MapDatabase);
            maps = JsonConvert.DeserializeObject<Dictionary<string,Map>>(jsonData);
        }
    }
}
