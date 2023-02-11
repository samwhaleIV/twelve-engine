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
            LimitFrameDelta,
            SampleRate,
            FMODGame,
            FMODMaster,
            FMODStrings
        }

        public enum ConfigValueType { Int, IntNullable, Bool, StringArray, String }

        private static Dictionary<string,(ConfigValueType Type, object Value)> GetConfigValues() => new() {
            { GetKey(Keys.Flags), (ConfigValueType.StringArray, null) },
            { GetKey(Keys.HWFullScreenWidth), (ConfigValueType.IntNullable, null) },
            { GetKey(Keys.HWFullScreenHeight), (ConfigValueType.IntNullable, null) },
            { GetKey(Keys.BenchmarkStateSwap), (ConfigValueType.Bool, false) },
            { GetKey(Keys.StateCleanUpGC), (ConfigValueType.Bool, false) },
            { GetKey(Keys.GamePadIndex), (ConfigValueType.Int, 0) },
            { GetKey(Keys.LimitFrameDelta), (ConfigValueType.Bool, false) },
            { GetKey(Keys.SampleRate), (ConfigValueType.IntNullable, null) },
            { GetKey(Keys.FMODGame), (ConfigValueType.String, Constants.DefaultFMODGameBank) },
            { GetKey(Keys.FMODStrings), (ConfigValueType.String, Constants.DefaultFMODStringBank) },
            { GetKey(Keys.FMODMaster), (ConfigValueType.String, Constants.DefaultFMODMasterBank) },
        };

        private static readonly Dictionary<string,(ConfigValueType Type, object Value)> configValues;

        private static readonly string[] keys;

        private static readonly Keys[] KeysList = Enum.GetValues<Keys>();

        private static string GetKey(Keys key) {
            return keys[(int)key];
        }

        static Config() {

            Keys[] enumValues = KeysList;
            string[] keys = new string[enumValues.Length];

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

        public static string GetString(string key) {
            ValidateKey(key,ConfigValueType.String);
            return (string)configValues[key].Value;
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

        public static void SetString(string key,string value) {
            ValidateKey(key,ConfigValueType.String);
            configValues[key] = (ConfigValueType.String, value);
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

        public static void SetString(Keys key,string value) {
            SetString(GetKey(key),value);
        }

        public static string GetString(Keys key) {
            return GetString(GetKey(key));
        }

        public static bool TryLoad(string path) {
            if(!File.Exists(path)) {
                Logger.WriteLine($"No config file found at path \"{path}\".",LoggerLabel.Config);
                if(path == Constants.ConfigFile) {
                    return false;
                }
                return TryLoad(Constants.ConfigFile);
            }
            List<string> lines = null;
            try {
                using StreamReader reader = new(path);
                lines = new List<string>();
                while(!reader.EndOfStream) {
                    lines.Add(reader.ReadLine());
                }           
            } catch(Exception exception) {
                Logger.WriteLine($"Failure reading config file from \"{path}\": {exception}",LoggerLabel.Config);
                return false;
            }
            if(lines == null || lines.Count <= 0) {
                return true;
            }
            AddConfigLines(lines);
            return true;
        }

        public static void WriteToLog() {
            var lease = Pools.StringBuilder.Lease(out var sb);
            sb.AppendLine("Config data: {");
            foreach(var key in KeysList) {
                if(!configValues.TryGetValue(GetKey(key),out var value)) {
                    continue;
                }
                var keyValue = GetKey(key);
                sb.Append($"    {keyValue} = ");
                switch(value.Type) {
                    case ConfigValueType.Int:
                        sb.Append(GetInt(keyValue));
                        break;
                    case ConfigValueType.IntNullable:
                        int? intValue = GetIntNullable(keyValue);
                        if(intValue.HasValue) {
                            sb.Append(intValue.Value);
                        } else {
                            sb.Append(Logger.NONE_TEXT);
                        }
                        break;
                    case ConfigValueType.Bool:
                        sb.Append(GetBool(keyValue));
                        break;
                    case ConfigValueType.StringArray:
                        var stringArray = GetStringArray(keyValue);
                        if(stringArray == null || stringArray.Length <= 0) {
                            sb.Append(Logger.NONE_TEXT);
                        } else {
                            sb.Append("{ ");
                            foreach(var item in stringArray) {
                                sb.Append($"{(string.IsNullOrWhiteSpace(item) ? Logger.EMPTY_TEXT : item)}, ");
                            }
                            sb.Remove(sb.Length-2,2);
                            sb.Append(" }");
                        }
                        break;
                    case ConfigValueType.String:
                        sb.Append(GetString(keyValue));
                        break;
                    default:
                        sb.Append(Logger.UNKNOWN_TEXT);
                        break;
                }
                sb.AppendLine();
            }
            sb.AppendLine("}");
            Logger.Write(sb,LoggerLabel.Config);
            Pools.StringBuilder.Return(lease);
        }

        private static bool IsOpenCloseSet(string value) => (value[0], value[^1]) switch {
            ('(', ')') => true,
            ('{', '}') => true,
            ('[', ']') => true,
            _ => false
        };

        private static string[] ParseStringArray(string value) {
            List<string> values = new();
            /* Shouldn't have to clear the shared 'stringBuilder' of this static class. Users should clear it when they are done using it */
            int i = 0;
            int end = value.Length;
            if(end - i >= 2 && IsOpenCloseSet(value)) {
                i += 1;
                end -= 1;
            }
            var lease = Pools.StringBuilder.Lease(out var sb);
            void FlushValue() {
                if(sb.Length != 0) {
                    values.Add(sb.ToString());
                }
                sb.Clear();
            }
            while(i < end) {
                char ch = value[i];
                if(ch == ',') {
                    FlushValue();
                } else {
                    sb.Append(ch);
                }
                i += 1;
            }
            FlushValue();
            Pools.StringBuilder.Return(lease);
            return values.ToArray();
        }

        private static bool IsExplicitNullValue(string value) {
            /*
                Very optimal, but very stupid. This is a harder to read version of:
                (value == string.Empty || value.ToLowerInvariant() == "null" || value.ToLowerInvariant() == "none")
            */

            if(value == string.Empty || value.Length != 4 || char.ToLowerInvariant(value[0]) != 'n') {
                return false;
            }

            char b = char.ToLowerInvariant(value[1]), c = char.ToLowerInvariant(value[2]), d = char.ToLowerInvariant(value[3]);

            return (b == 'u' && c == 'l' && d == 'l') || (b == 'o' && c == 'n' && d == 'e');
        }

        private static string RemoveWhiteSpace(string value) {
            var lease = Pools.StringBuilder.Lease(out var sb);
            for(int i = 0;i<value.Length;i++) {
                char c = value[i];
                if(c == ' ') {
                    continue;
                }
                sb.Append(c);
            }
            value = sb.ToString();
            Pools.StringBuilder.Return(lease);
            return value;
        }

        private static void SetConfigValue(string key,string value,ConfigValueType type) {
            switch(type) {
                case ConfigValueType.Bool:
                    value = RemoveWhiteSpace(value);
                    SetBool(key,bool.Parse(value));
                    return;
                case ConfigValueType.Int:
                    value = RemoveWhiteSpace(value);
                    SetInt(key,int.Parse(value));
                    return;
                case ConfigValueType.IntNullable:
                    value = RemoveWhiteSpace(value);
                    if(IsExplicitNullValue(value)) {
                        SetIntNullable(key,null);
                    } else {
                        SetIntNullable(key,int.Parse(value));
                    }
                    return;
                case ConfigValueType.StringArray:
                    value = RemoveWhiteSpace(value);
                    if(IsExplicitNullValue(value)) {
                        SetStringArray(key,Array.Empty<string>());
                    } else {
                        SetStringArray(key,ParseStringArray(value));
                    }
                    return;
                case ConfigValueType.String:
                    SetString(key,value.Trim());
                    break;
                default:
                    Logger.WriteLine($"No value type found for key '{key}'",LoggerLabel.Config);
                    return;
            }
        }

        private static void AddConfigLines(IEnumerable<string> lines) {
            var sbLeases = Pools.StringBuilder.Lease(out var keyBuffer,out var valueBuffer);

            foreach(string line in lines) {

                keyBuffer.Clear();
                valueBuffer.Clear();

                bool writeKey = true;

                foreach(char ch in line) {
                    if(writeKey && ch == ' ') {
                        /* Ignore white space for keys */
                        continue;
                    }
                    if(ch == Constants.ConfigValueOperand) {
                        if(!writeKey) {
                            /* Ignore the value operand if it shows up as part of the value itself */
                            break;
                        }
                        writeKey = false;
                        continue;
                    }
                    if(writeKey) {
                        keyBuffer.Append(ch);
                    } else {
                        valueBuffer.Append(ch);
                    }
                }
                if(writeKey) {
                    /* Never got a a value operand for this line ... */
                    continue;
                }

                string key = keyBuffer.ToString(), value = valueBuffer.ToString();

                if(!configValues.TryGetValue(key,out var configValue)) {
                    Logger.WriteLine($"Key '{key}' is not registered in this application.",LoggerLabel.Config);
                    continue;
                }

                ConfigValueType valueType = configValue.Type;
                try {
                    SetConfigValue(key,value,valueType);
                } catch(Exception exception) {
                    Logger.WriteLine($"Illegal value for key '{key}': {exception.Message}",LoggerLabel.Config);
                }
            }

            Pools.StringBuilder.Return(sbLeases);
        }
    }
}
