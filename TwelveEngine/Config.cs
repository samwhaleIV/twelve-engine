using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine {

    public enum ConfigValueType { Int, IntNullable, Bool, StringArray }

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

        private static readonly Dictionary<string,(ConfigValueType Type, object Value)> configValues = new() {
            { nameof(Keys.Flags), (ConfigValueType.StringArray, null) },
            { nameof(Keys.HWFullScreenWidth), (ConfigValueType.IntNullable, null) },
            { nameof(Keys.HWFullScreenHeight), (ConfigValueType.IntNullable, null) },
            { nameof(Keys.BenchmarkStateSwap), (ConfigValueType.Bool, false) },
            { nameof(Keys.StateCleanUpGC), (ConfigValueType.Bool, false) },
            { nameof(Keys.GamePadIndex), (ConfigValueType.Int, 0) },
            { nameof(Keys.LimitFrameDelta), (ConfigValueType.Bool, false) }
        };

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

        public static void SetInt(Keys key,int value) => SetInt(key.ToString(),value);
        public static void SetIntNullable(Keys key,int? value) => SetIntNullable(key.ToString(),value);
        public static void SetBool(Keys key,bool value) => SetBool(key.ToString(),value);
        public static void SetStringArray(Keys key,params string[] value) => SetStringArray(key.ToString(),value);

        public static int GetInt(Keys key) => GetInt(key.ToString());
        public static int? GetIntNullable(Keys key) => GetIntNullable(key.ToString());
        public static bool GetBool(Keys key) => GetBool(key.ToString());
        public static string[] GetStringArray(Keys key) => GetStringArray(key.ToString());


        private static void LoadConfigLines(string[] lines) {
            //todo
            //ignore key that don't exist in the table instead of throwing an error (config values can exist in file that are not being supported by the engine or game)
        }

        public static bool TryLoad(string path) {
            if(!File.Exists(path)) {
                Logger.WriteLine($"No config file found at path \"{path}\".");
                if(path == Constants.ConfigFile) {
                    return false;
                }
                return TryLoad(Constants.ConfigFile);
            }
            string[] lines = null;
            try {
                lines = File.ReadAllLines(path);
            } catch(Exception exception) {
                Logger.WriteLine($"Failure reading config file from \"{path}\": {exception}");
            }
            if(lines == null || lines.Length <= 0) {
                return false;
            }
            LoadConfigLines(lines);
            return true;
        }

        private static readonly StringBuilder stringBuilder = new();

        private static readonly Keys[] KeysList = (Keys[])Enum.GetValues(typeof(Keys));

        public static void WriteToLog() {
            stringBuilder.AppendLine("[Config] {");
            foreach(var key in KeysList) {
                if(!configValues.TryGetValue(key.ToString(),out var value)) {
                    continue;
                }
                var keyValue = key.ToString();
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
            Logger.Write(stringBuilder);
            stringBuilder.Clear();
        }
    }
}
