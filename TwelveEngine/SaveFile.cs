using System.Text;

namespace TwelveEngine {

    public enum SaveValueType { String, Int, Flag, Bool, ByteArray, Float }

    public readonly struct SaveValue {
        public SaveValue(SaveValueType type,object value) {
            Type = type;
            Value = value;
        }
        public readonly SaveValueType Type;
        public readonly object Value;
    }

    public sealed class SaveFile {

        private readonly Dictionary<int,SaveValue> dataTable;
        public int KeyCount => dataTable.Count;

        private bool _isDirty = false;

        public string Path { get; set; }

        public SaveFile() {
            dataTable = new Dictionary<int,SaveValue>();
        }

        public SaveFile(IEnumerable<KeyValuePair<int,SaveValue>> data) {
            dataTable = new Dictionary<int,SaveValue>(data);
        }

        public SaveFile((int Key,SaveValue Value)[] data) {
            dataTable = new Dictionary<int,SaveValue>(data.Length);
            foreach(var (Key, Value) in data) {
                dataTable[Key] = Value;
            }
        }

        public void Clear() {
            if(dataTable.Count <= 0) {
                return;
            }
            dataTable.Clear();
            _isDirty = true;
        }

        public bool TrySave() {
            if(!_isDirty) {
                Logger.WriteLine($"Save file \"{Path}\" has not been modified.",LoggerLabel.Save);
                return true;
            }
            bool success = false;
            try {
                using var fs = File.Open(Path,FileMode.Create,FileAccess.Write);
                using var bw = new BinaryWriter(fs,Encoding.UTF8);
                Export(bw);
                Logger.WriteLine($"Wrote save data to \"{Path}\".",LoggerLabel.Save);
                success = true;
            } catch(Exception exception) {
                Logger.WriteLine(exception.ToString());
            }
            _isDirty = !success;
            return success;
        }

        public bool TryLoad() {
            bool success = false;
            if(!File.Exists(Path)) {
                Logger.WriteLine($"Save file \"{Path}\" does not exist.",LoggerLabel.Save);
                return false;
            }
            try {
                using var fs = File.Open(Path,FileMode.Open,FileAccess.Read);
                if(fs.Length <= 0) {
                    Logger.WriteLine($"Cannot read save data. File \"{Path}\" is empty!",LoggerLabel.Save);
                    success = false;
                } else {
                    dataTable.Clear();
                    using var br = new BinaryReader(fs,Encoding.UTF8);
                    Import(br);
                    Logger.WriteLine($"Loaded save data from file \"{Path}\".",LoggerLabel.Save);
                    success = true;
                }
            } catch(Exception exception) {
                dataTable.Clear();
                Logger.WriteLine($"Failure reading save data. Save data was corrupted and has been reset: {exception}",LoggerLabel.Save);
            }
            _isDirty = false;
            return success;
        }

        private void Export(BinaryWriter binaryWriter) {
            binaryWriter.Write(dataTable.Count);
            foreach(KeyValuePair<int,SaveValue> item in dataTable) {
                binaryWriter.Write(item.Key);
                SaveValueType valueType = item.Value.Type;
                binaryWriter.Write((int)valueType);
                object value = item.Value.Value;
                switch(valueType) {
                    case SaveValueType.String:
                        binaryWriter.Write((string)value);
                        break;
                    case SaveValueType.Int:
                        binaryWriter.Write((int)value);
                        break;
                    case SaveValueType.Float:
                        binaryWriter.Write((float)value);
                        break;
                    case SaveValueType.Bool:
                        binaryWriter.Write((bool)value);
                        break;
                    case SaveValueType.ByteArray: {
                        byte[] bytes = (byte[])value;
                        binaryWriter.Write(bytes.Length);
                        binaryWriter.Write(bytes);
                        break;
                    }
                    default:
                    case SaveValueType.Flag:
                        break;
                }
            }
        }

        private void Import(BinaryReader reader) {
            int itemCount = reader.ReadInt32();
            for(int i = 0;i<itemCount;i++) {
                var key = reader.ReadInt32();
                var valueType = (SaveValueType)reader.ReadInt32();

                dataTable[key] = valueType switch {
                    SaveValueType.String => new SaveValue(valueType,reader.ReadString()),
                    SaveValueType.Int => new SaveValue(valueType,reader.ReadInt32()),
                    SaveValueType.Bool => new SaveValue(valueType,reader.ReadBoolean()),
                    SaveValueType.ByteArray => new SaveValue(valueType,reader.ReadBytes(reader.ReadInt32())),
                    SaveValueType.Float => new SaveValue(valueType,reader.ReadSingle()),
                    _ => new SaveValue(SaveValueType.Flag,null),
                };
            }
        }

        public bool TryGetString(int key,out string value,string defaultValue = null) {
            if(!dataTable.TryGetValue(key,out var saveValue) || saveValue.Type != SaveValueType.String) {
                value = defaultValue;
                return false;
            }
            value = (string)saveValue.Value;
            return true;
        }

        public bool TryGetInt(int key,out int value,int defaultValue = 0) {
            if(!dataTable.TryGetValue(key,out var saveValue) || saveValue.Type != SaveValueType.Int) {
                value = defaultValue;
                return false;
            }
            value = (int)saveValue.Value;
            return true;
        }

        public bool TryGetFloat(int key,out float value,float defaultValue = 0) {
            if(!dataTable.TryGetValue(key,out var saveValue) || saveValue.Type != SaveValueType.Float) {
                value = defaultValue;
                return false;
            }
            value = (float)saveValue.Value;
            return true;
        }

        public bool TryGetBool(int key,out bool value,bool defaultValue = false) {
            if(!dataTable.TryGetValue(key,out var saveValue) || saveValue.Type != SaveValueType.Bool) {
                value = defaultValue;
                return false;
            }
            value = (bool)saveValue.Value;
            return true;
        }

        public bool TryGetBytes(int key,out byte[] value) {
            if(!dataTable.TryGetValue(key,out var saveValue) || saveValue.Type != SaveValueType.ByteArray) {
                value = null;
                return false;
            }
            value = (byte[])saveValue.Value;
            return true;
        }

        public bool HasFlag(int key) {
            if(!dataTable.TryGetValue(key,out var saveValue) || saveValue.Type != SaveValueType.Flag) {
                return false;
            }
            return true;
        }

        public bool HasKey(int key) {
            return dataTable.ContainsKey(key);
        }

        public void RemoveKey(int key) {
            if(!dataTable.ContainsKey(key)) {
                return;
            }
            dataTable.Remove(key);
            _isDirty = true;
        }

        public void SetFlag(int key) {
            bool hasOldValue = HasFlag(key);
            dataTable[key] = new SaveValue(SaveValueType.Flag,null);
            _isDirty = !hasOldValue;
        }

        public void IncrementCounter(int key,int amount = 1) {
            TryGetInt(key,out var value);
            value += amount;
            SetValue(key,value);
        }

        public void SetValue(int key,string value) {
            bool hasOldValue = TryGetString(key,out var oldValue);
            dataTable[key] = new SaveValue(SaveValueType.String,value);
            _isDirty = !hasOldValue || oldValue != value;
        }

        public void SetValue(int key,int value) {
            bool hasOldValue = TryGetInt(key,out var oldValue);
            dataTable[key] = new SaveValue(SaveValueType.Int,value);
            _isDirty = !hasOldValue || oldValue != value;
        }

        public void SetValue(int key,bool value) {
            bool hasOldValue = TryGetBool(key,out var oldValue);
            dataTable[key] = new SaveValue(SaveValueType.Bool,value);
            _isDirty = !hasOldValue || oldValue != value;
        }

        public void SetValue(int key,float value) {
            bool hasOldValue = TryGetFloat(key,out var oldValue);
            dataTable[key] = new SaveValue(SaveValueType.Float,value);
            _isDirty = !hasOldValue || oldValue != value;
        }

        public void SetBytes(int key,byte[] value) {
            dataTable[key] = new SaveValue(SaveValueType.ByteArray,value);
            _isDirty = true;
        }
    }
}
