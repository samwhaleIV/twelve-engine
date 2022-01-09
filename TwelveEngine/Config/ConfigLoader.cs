using System.IO;

namespace TwelveEngine.Config {
    public static class ConfigLoader {

        internal static KeyBindSet LoadKeyBinds(string path = null) {
            if(path == null) {
                path = Constants.Config.KeyBindsFile;
            }
            if(!File.Exists(path)) {
                return new KeyBindSet();
            }

            var processor = new ConfigProcessor<KeyBindSet>();
            var keyBindSet = processor.Load(path);

            return keyBindSet;
        }

        public static TPropertySet LoadEngineConfig<TPropertySet>(string path = null) where TPropertySet : TwelveConfigSet, new() {
            if(path == null) {
                path = Constants.EngineConfigFile;
            }

            TPropertySet propertySet;
            if(!File.Exists(path)) {
                propertySet = new TPropertySet();
            } else {
                var processor = new ConfigProcessor<TPropertySet>();
                propertySet = processor.Load(path);
            }

            var engineConfig = new TwelveConfig();
            engineConfig.Import(propertySet);
            Constants.Config = engineConfig;

            return propertySet;
        }

        public static void LoadEngineConfig(string path = null) => LoadEngineConfig<TwelveConfigSet>(path);
    }
}
