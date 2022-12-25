using System.IO;
using System.Text;

namespace TwelveEngine.Shell.Config {
    public static class ConfigLoader {

        internal static KeyBindSet LoadKeyBinds(string path = null) {
            path ??= Constants.Config.KeyBindsFile;
            if(!File.Exists(path)) {
                return new KeyBindSet();
            }

            var processor = new ConfigProcessor<KeyBindSet>();

            var lines = File.ReadAllLines(path);
            var keyBindSet = processor.Load(lines);

            return keyBindSet;
        }

        private static TPropertySet LoadEngineConfig<TPropertySet>(TPropertySet propertySet,bool logProgramSource) where TPropertySet : TwelveConfigSet, new() {
            if(logProgramSource) {
                Logger.WriteLine($"Loading engine config from a programmed property set.");
            }
            var engineConfig = new TwelveConfig();
            engineConfig.Import(propertySet);
            Constants.Config = engineConfig;
            var sb = new StringBuilder();
            Constants.Config.Write(sb);
            Logger.WriteLine(sb);
            return propertySet;
        }

        public static TPropertySet LoadEngineConfig<TPropertySet>(TPropertySet propertySet) where TPropertySet : TwelveConfigSet, new() {
            return LoadEngineConfig(propertySet,true);
        }

        public static TPropertySet LoadEngineConfig<TPropertySet>(string path = null) where TPropertySet : TwelveConfigSet, new() {
            path ??= Constants.EngineConfigFile;

            Logger.WriteLine($"Loading engine config from file \"{path}\".");

            TPropertySet propertySet;
            if(!File.Exists(path)) {
                propertySet = new TPropertySet();
            } else {
                var processor = new ConfigProcessor<TPropertySet>();
                var lines = File.ReadAllLines(path);
                propertySet = processor.Load(lines);
            }

            return LoadEngineConfig(propertySet,false);
        }

        public static void LoadEngineConfig(string path = null) => LoadEngineConfig<TwelveConfigSet>(path);
    }
}
