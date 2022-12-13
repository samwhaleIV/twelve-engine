using System;
using System.IO;

namespace TwelveEngine.Shell.Config {
    public static class ConfigWriter {
        public static void SaveKeyBinds(KeyBindSet keyBindSet,string path = null) {
            if(path == null) {
                path = Constants.Config.KeyBindsFile;
            }
            var processor = new ConfigProcessor<KeyBindSet>();
            var contents = processor.Save(keyBindSet);
            
            File.WriteAllText(path,contents);
            GC.Collect();
        }

        public static void SaveEngineConfig(string path = null) {
            /* Used to generate your game's own config file, this generates a file of the default or modified engine config */
            if(path == null) {
                path = Constants.EngineConfigFile;
            }
            var config = Constants.Config;
            var propertySet = config.Export();

            var processor = new ConfigProcessor<TwelveConfigSet>();
            var contents = processor.Save(propertySet);

            File.WriteAllText(path,contents);
            GC.Collect();
        }
    }
}
