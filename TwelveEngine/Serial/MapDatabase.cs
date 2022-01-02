using System.IO;
using System.Collections.Generic;

namespace TwelveEngine.Serial.Map {

    public static class MapDatabase {

        private static readonly Dictionary<string,Map> maps;
        static MapDatabase() {
            maps = new Dictionary<string,Map>();
        }
        private static void addMap(string name,Map map) => maps.Add(name,map);

        public static Map? GetMap(string name) {
            if(maps.ContainsKey(name)) {
                return maps[name];
            } else {
                return null;
            }
        }

        private static FileStream getDatabaseReadStream(string path) {
            return File.Open(path,FileMode.Open,FileAccess.Read,FileShare.Read);
        }

        public static void LoadMaps() {
            var path = $"{Constants.Config.ContentDirectory}/{Constants.MapDatabase}";
            using(var file = getDatabaseReadStream(path)) {
                var decoder = new MapDecoder(file);
                decoder.ReadDatabase(addMap);
            }
        }
    }
}
