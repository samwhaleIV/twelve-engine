using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Elves {
    public sealed class SaveData {

        private readonly Dictionary<string,SaveValue> dataTable = new Dictionary<string,SaveValue>();
        public int KeyCount => dataTable.Count;

        private enum SaveValueType:int { String, Int, Flag, Bool }

        private readonly struct SaveValue {
            public SaveValue(SaveValueType type,object value) {
                Type = type;
                Value = value;
            }
            public readonly SaveValueType Type;
            public readonly object Value;
        }

        public bool TrySave(string filePath,StringBuilder stringBuilder) {
            bool success = false;
            try {
                using var fs = File.Open(filePath,FileMode.Create,FileAccess.Write);
                using var bw = new BinaryWriter(fs,Encoding.UTF8);
                Export(bw);
                stringBuilder.AppendLine($"Wrote save data to \"{filePath}\".");
                success = true;
            } catch(Exception exception) {
                stringBuilder.AppendLine(exception.ToString());
            }
            return success;
        }

        public bool TryLoad(string filePath,StringBuilder stringBuilder) {
            bool success = false;
            if(!File.Exists(filePath)) {
                stringBuilder.AppendLine($"File \"{filePath}\" does not exist.");
                return false;
            }
            try {
                using var fs = File.Open(filePath,FileMode.Open,FileAccess.Read);
                if(fs.Length <= 0) {
                    stringBuilder.AppendLine($"Cannot read save data. File \"{filePath}\" is empty!");
                    success = false;
                } else {
                    dataTable.Clear();
                    using var br = new BinaryReader(fs,Encoding.UTF8);
                    Import(br);
                    stringBuilder.AppendLine($"Loaded save data from file \"{filePath}\".");
                    success = true;
                }
            } catch(Exception exception) {
                dataTable.Clear();
                stringBuilder.AppendLine(exception.ToString());
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
                var key = reader.ReadString();
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

        public bool TryGetString(string key,out string value,string defaultValue = null) {
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

        public bool TryGetInt(string key,out int value,int defaultValue = 0) {
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

        public bool TryGetBool(string key,out bool value,bool defaultValue = false) {
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

        public bool HasKey(string key) {
            return dataTable.ContainsKey(key);
        }

        public void RemoveKey(string key) {
            dataTable.Remove(key);
        }

        public void SetFlag(string key) {
            dataTable[key] = new SaveValue(SaveValueType.Flag,null);
        }

        public void SetValue(string key,string value) {
            dataTable.Add(key,new SaveValue(SaveValueType.String,value));
        }

        public void SetValue(string key,int value) {
            dataTable.Add(key,new SaveValue(SaveValueType.Int,value));
        }

        public void SetValue(string key,bool value) {
            dataTable.Add(key,new SaveValue(SaveValueType.Bool,value));
        }
    }
}
