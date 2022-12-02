using System.IO;
using System.Collections.Generic;

namespace TwelveEngine.Game2D.MapFormat {

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

        public static void LoadMaps(string mapDatabase) {
            using(FileStream file = getDatabaseReadStream(mapDatabase)) {
                var decoder = new MapDecoder(file);
                decoder.ReadDatabase(addMap);
            }
        }
    }
}
