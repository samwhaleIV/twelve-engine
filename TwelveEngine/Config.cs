using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine {

    public static class Config {

        public enum Keys {
            Flags,
            HWFullScreenWidth,
            HWFullScreenHeight,
            BenchmarkStateSwap,
            StateCleanUpGC,
            GamePadIndex,
            LimitFrameDelta
        }

        public enum ConfigValueType { Int, IntNullable, Bool, StringArray }

        private static Dictionary<string,(ConfigValueType Type, object Value)> GetConfigValues() => new() {
            { GetKey(Keys.Flags), (ConfigValueType.StringArray, null) },
            { GetKey(Keys.HWFullScreenWidth), (ConfigValueType.IntNullable, null) },
            { GetKey(Keys.HWFullScreenHeight), (ConfigValueType.IntNullable, null) },
            { GetKey(Keys.BenchmarkStateSwap), (ConfigValueType.Bool, false) },
            { GetKey(Keys.StateCleanUpGC), (ConfigValueType.Bool, false) },
            { GetKey(Keys.GamePadIndex), (ConfigValueType.Int, 0) },
            { GetKey(Keys.LimitFrameDelta), (ConfigValueType.Bool, false) }
        };

        private static readonly Dictionary<string,(ConfigValueType Type, object Value)> configValues;

        private static readonly string[] keys;

        private static string GetKey(Keys key) {
            return keys[(int)key];
        }

        static Config() {

            var enumValues = KeysList;
            var keys = new string[enumValues.Length];

            for(int i = 0;i<keys.Length;i++) {
                keys[i] = enumValues[i].ToString();
            }

            Config.keys = keys;

            configValues = GetConfigValues();
        }

        public static void RegisterConfigValue(string key,ConfigValueType Type,object Value) {
            configValues.Add(key,(Type,Value));
        }

        public static void RegisterConfigValue(params (string Key,ConfigValueType Type,object Value)[] values) {
            foreach(var value in values) {
                configValues.Add(value.Key,(value.Type, value.Value));
            }
        }

        private static void ValidateKey(string key,ConfigValueType type) {
            if(!configValues.TryGetValue(key,out var value)) {
                throw new InvalidOperationException($"Missing key '{key}'");
            }
            if(value.Type != type) {
                throw new InvalidOperationException($"Config value '{key}' is not of type {type}.");
            }
        }

        public static void SetInt(string key,int value) {
            ValidateKey(key,ConfigValueType.Int);
            configValues[key] = (ConfigValueType.Int, value);
        }

        public static void SetIntNullable(string key,int? value) {
            ValidateKey(key,ConfigValueType.IntNullable);
            configValues[key] = (ConfigValueType.IntNullable, value);
        }

        public static void SetBool(string key,bool value) {
            ValidateKey(key,ConfigValueType.Bool);
            configValues[key] = (ConfigValueType.Bool, value);
        }

        public static void SetStringArray(string key,params string[] value) {
            ValidateKey(key,ConfigValueType.StringArray);
            configValues[key] = (ConfigValueType.StringArray, value);
        }

        public static int GetInt(string key) {
            ValidateKey(key,ConfigValueType.Int);
            return (int)configValues[key].Value;
        }

        public static int? GetIntNullable(string key) {
            ValidateKey(key,ConfigValueType.IntNullable);
            return (int?)configValues[key].Value;
        }

        public static bool GetBool(string key) {
            ValidateKey(key,ConfigValueType.Bool);
            return (bool)configValues[key].Value;
        }

        public static string[] GetStringArray(string key) {
            ValidateKey(key,ConfigValueType.StringArray);
            return (string[])configValues[key].Value;
        }

        public static void SetInt(Keys key,int value) {
            SetInt(GetKey(key),value);
        }


        public static void SetIntNullable(Keys key,int? value) {
            SetIntNullable(GetKey(key),value);
        }

        public static void SetBool(Keys key,bool value) {
            SetBool(GetKey(key),value);
        }

        public static void SetStringArray(Keys key,params string[] value) {
            SetStringArray(GetKey(key),value);
        }

        public static int GetInt(Keys key) {
            return GetInt(GetKey(key));
        }

        public static int? GetIntNullable(Keys key) {
            return GetIntNullable(GetKey(key));
        }
        public static bool GetBool(Keys key) {
            return GetBool(GetKey(key));
        }

        public static string[] GetStringArray(Keys key) {
            return GetStringArray(GetKey(key));
        }

        public static bool TryLoad(string path) {
            if(!File.Exists(path)) {
                Logger.WriteLine($"No config file found at path \"{path}\".",LoggerLabel.Config);
                if(path == Constants.ConfigFile) {
                    return false;
                }
                return TryLoad(Constants.ConfigFile);
            }
            string[] lines = null;
            try {
                LoadConfigLines(path);
            } catch(Exception exception) {
                Logger.WriteLine($"Failure reading config file from \"{path}\": {exception}",LoggerLabel.Config);
            }
            if(lines == null || lines.Length <= 0) {
                return false;
            }

            return true;
        }

        private static readonly StringBuilder stringBuilder = new();

        private static readonly Keys[] KeysList = Enum.GetValues<Keys>();

        public static void WriteToLog() {
            stringBuilder.AppendLine("Config data: {");
            foreach(var key in KeysList) {
                if(!configValues.TryGetValue(GetKey(key),out var value)) {
                    continue;
                }
                var keyValue = GetKey(key);
                stringBuilder.Append($"    {keyValue} = ");
                switch(value.Type) {
                    case ConfigValueType.Int:
                        stringBuilder.Append(GetInt(keyValue));
                        break;
                    case ConfigValueType.IntNullable:
                        stringBuilder.Append(GetIntNullable(keyValue));
                        break;
                    case ConfigValueType.Bool:
                        stringBuilder.Append(GetBool(keyValue));
                        break;
                    case ConfigValueType.StringArray:
                        var stringArray = GetStringArray(keyValue);
                        if(stringArray == null || stringArray.Length <= 0) {
                            stringBuilder.Append(Constants.Logging.None);
                        } else {
                            stringBuilder.Append("{ ");
                            foreach(var item in stringArray) {
                                stringBuilder.Append($"{(string.IsNullOrWhiteSpace(item) ? Constants.Logging.Empty : item)}, ");
                            }
                            stringBuilder.Remove(stringBuilder.Length-2,2);
                            stringBuilder.Append(" }");
                        }
                        break;
                    default:
                        stringBuilder.Append(Constants.Logging.Unknown);
                        break;
                }
                stringBuilder.AppendLine();
            }
            stringBuilder.AppendLine("}");
            Logger.Write(stringBuilder,LoggerLabel.Config);
            stringBuilder.Clear();
        }

        private static void LoadConfigLines(string path) {
            return;
            using var reader = new StreamReader(path);

            //todo
            //ignore key that don't exist in the table instead of throwing an error (config values can exist in file that are not being supported by the engine or game)
        }
    }
}
