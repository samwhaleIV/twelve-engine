using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace TwelveEngine {

    public enum SaveValueType:int { String, Int, Flag, Bool }

    public readonly struct SaveValue {
        public SaveValue(SaveValueType type,object value) {
            Type = type;
            Value = value;
        }
        public readonly SaveValueType Type;
        public readonly object Value;
    }

    public sealed class SaveData {

        private readonly Dictionary<int,SaveValue> dataTable;
        public int KeyCount => dataTable.Count;

        public SaveData() {
            dataTable = new Dictionary<int,SaveValue>();
        }

        public SaveData(IEnumerable<KeyValuePair<int,SaveValue>> data) {
            dataTable = dataTable = new Dictionary<int,SaveValue>(data);
        }

        public SaveData((int Key,SaveValue Value)[] data) {
            dataTable = dataTable = new Dictionary<int,SaveValue>(data.Length);
            foreach(var item in data) {
                dataTable[item.Key] = item.Value;
            }
        }

        public void Clear() {
            dataTable.Clear();
        }

        public bool TrySave(string filePath,StringBuilder stringBuilder) {
            bool success = false;
            try {
                using var fs = File.Open(filePath,FileMode.Create,FileAccess.Write);
                using var bw = new BinaryWriter(fs,Encoding.UTF8);
                Export(bw);
                stringBuilder.Append($"Wrote save data to \"{filePath}\".");
                success = true;
            } catch(Exception exception) {
                stringBuilder.Append(exception.ToString());
            }
            return success;
        }

        public bool TryLoad(string filePath,StringBuilder stringBuilder) {
            bool success = false;
            if(!File.Exists(filePath)) {
                stringBuilder.Append($"File \"{filePath}\" does not exist.");
                return false;
            }
            try {
                using var fs = File.Open(filePath,FileMode.Open,FileAccess.Read);
                if(fs.Length <= 0) {
                    stringBuilder.Append($"Cannot read save data. File \"{filePath}\" is empty!");
                    success = false;
                } else {
                    dataTable.Clear();
                    using var br = new BinaryReader(fs,Encoding.UTF8);
                    Import(br);
                    stringBuilder.Append($"Loaded save data from file \"{filePath}\".");
                    success = true;
                }
            } catch(Exception exception) {
                dataTable.Clear();
                stringBuilder.Append(exception.ToString());
            }
            return success;
        }

        private void Export(BinaryWriter binaryWriter) {
            binaryWriter.Write(dataTable.Count);
            foreach(var item in dataTable) {
                binaryWriter.Write(item.Key);
                var valueType = item.Value.Type;
                binaryWriter.Write((int)valueType);
                switch(valueType) {
                    case SaveValueType.String:
                        binaryWriter.Write((string)item.Value.Value);
                        break;
                    case SaveValueType.Int:
                        binaryWriter.Write((int)item.Value.Value);
                        break;
                    case SaveValueType.Bool:
                        binaryWriter.Write((bool)item.Value.Value);
                        break;
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
                switch(valueType) {
                    case SaveValueType.String:
                        dataTable[key] = new SaveValue(valueType,reader.ReadString());
                        break;
                    case SaveValueType.Int:
                        dataTable[key] = new SaveValue(valueType,reader.ReadInt32());
                        break;
                    case SaveValueType.Bool:
                        dataTable[key] = new SaveValue(valueType,reader.ReadBoolean());
                        break;
                    default:
                    case SaveValueType.Flag:
                        dataTable[key] = new SaveValue(SaveValueType.Flag,null);
                        break;
                }
            }
        }

        public bool TryGetString(int key,out string value,string defaultValue = null) {
            if(!dataTable.TryGetValue(key,out var saveValue)) {
                value = defaultValue;
                return false;
            }
            if(saveValue.Type != SaveValueType.String) {
                value = defaultValue;
                return false;
            }
            value = (string)saveValue.Value;
            return true;
        }

        public bool TryGetInt(int key,out int value,int defaultValue = 0) {
            if(!dataTable.TryGetValue(key,out var saveValue)) {
                value = defaultValue;
                return false;
            }
            if(saveValue.Type != SaveValueType.Int) {
                value = defaultValue;
                return false;
            }
            value = (int)saveValue.Value;
            return true;
        }

        public bool TryGetBool(int key,out bool value,bool defaultValue = false) {
            if(!dataTable.TryGetValue(key,out var saveValue)) {
                value = defaultValue;
                return false;
            }
            if(saveValue.Type != SaveValueType.Bool) {
                value = defaultValue;
                return false;
            }
            value = (bool)saveValue.Value;
            return true;
        }

        public bool HasKey(int key) {
            return dataTable.ContainsKey(key);
        }

        public void RemoveKey(int key) {
            dataTable.Remove(key);
        }

        public void SetFlag(int key) {
            dataTable[key] = new SaveValue(SaveValueType.Flag,null);
        }

        public void SetValue(int key,string value) {
            dataTable[key] = new SaveValue(SaveValueType.String,value);
        }

        public void SetValue(int key,int value) {
            dataTable[key] = new SaveValue(SaveValueType.Int,value);
        }

        public void SetValue(int key,bool value) {
            dataTable[key] = new SaveValue(SaveValueType.Bool,value);
        }
    }
}
