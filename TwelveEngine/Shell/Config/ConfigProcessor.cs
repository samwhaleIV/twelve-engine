using System;
using System.Text;
using System.Collections.Generic;
using System.Reflection;

namespace TwelveEngine.Shell.Config {
    public sealed class ConfigProcessor<TPropertySet> where TPropertySet : new() {

        private readonly Type propertySetType;
        private readonly FieldInfo[] propertyFields;

        public ConfigProcessor() {
            propertySetType = typeof(TPropertySet);
            propertyFields = propertySetType.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        private static Dictionary<string,string> GetFileProperties(string[] lines) {
            var dictionary = new Dictionary<string,string>();
            foreach(var line in lines) {
                int splitIndex = line.IndexOf(Constants.ConfigValueOperand);
                if(splitIndex < 0) {
                    continue;
                }
                var name = line[..splitIndex].Trim();
                splitIndex += 1;
                var value = line[splitIndex..].Trim();
                dictionary[name] = value;
            }

            return dictionary;
        }

        private static Dictionary<string,int> GetNameTable(FieldInfo[] fields) {
            var table = new Dictionary<string,int>();
            for(var i = 0;i < fields.Length;i++) {
                var field = fields[i];
                table[field.Name] = i;
            }
            return table;
        }

        private TPropertySet GetPropertySet(Dictionary<string,string> fileProperties) {
            var propertySet = new TPropertySet();
            var nameTable = GetNameTable(propertyFields);

            foreach(var item in fileProperties) {
                var propertyName = item.Key;
                if(!nameTable.ContainsKey(propertyName)) {
                    continue;
                }

                var propertyValue = item.Value;
                var fieldIndex = nameTable[propertyName];
                var field = propertyFields[fieldIndex];
                ApplyProperty(propertySet,field,propertyValue);
            }

            return propertySet;
        }

        private static bool IsNullableType(PropertyType propertyType) {
            return propertyType == PropertyType.IntNullable;
        }

        private static void ApplyProperty(TPropertySet set,FieldInfo field,string propertyValue) {
            var typeName = field.FieldType.FullName;

            if(!TypeParser.TryGetType(typeName,out var type)) {
                return;
            }

            object boxedValue = TypeParser.Parse(type,propertyValue);
            if(boxedValue == null && !IsNullableType(type)) {
                return;
            }
            field.SetValue(set,boxedValue);
        }

        public TPropertySet Load(string[] lines) {
            var fileProperties = GetFileProperties(lines);
            return GetPropertySet(fileProperties);
        }

        private static bool TryGetValue(TPropertySet propertySet,FieldInfo field,out string value) {
            if(!TypeParser.TryGetType(field.FieldType.FullName,out var type)) {
                value = null;
                return false;
            }
            var fieldValue = field.GetValue(propertySet);

            if(fieldValue == null) {
                value = null;
                return false;
            }
            value = TypeParser.Export(type,fieldValue);
            return true;
        }

        private static readonly StringBuilder stringBuilder = new();

        public string Save(TPropertySet propertySet) {
            stringBuilder.Clear();

            foreach(var field in propertyFields) {

                if(!TryGetValue(propertySet,field,out string value)) continue;

                var propertyName = field.Name;
                stringBuilder.Append(propertyName);
                stringBuilder.Append(' ');
                stringBuilder.Append(Constants.ConfigValueOperand);
                stringBuilder.Append(' ');
                stringBuilder.Append(value);
                stringBuilder.AppendLine();
            }

            if(stringBuilder.Length < 1) {
                return null;
            }

            stringBuilder.Remove(stringBuilder.Length-1,1);

            var contents = stringBuilder.ToString();
            stringBuilder.Clear();

            return contents;
        }
    }
}
