using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace TwelveEngine.Config {
    public sealed class ConfigProcessor<TPropertySet> where TPropertySet : new() {

        public const char VALUE_OPERAND = '=', ARRAY_DELIMITER = ',';

        private readonly TypeParser typeParser = new TypeParser(ARRAY_DELIMITER);

        private readonly Type propertySetType;
        private readonly FieldInfo[] propertyFields;

        public ConfigProcessor() {
            propertySetType = typeof(TPropertySet);
            propertyFields = propertySetType.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        private Dictionary<string,string> getFileProperties(string[] lines) {
            var dictionary = new Dictionary<string,string>();
            foreach(var line in lines) {
                int splitIndex = line.IndexOf(VALUE_OPERAND);
                if(splitIndex < 0) {
                    continue;
                }
                var name = line.Substring(0,splitIndex).Trim();
                splitIndex += 1;
                var value = line.Substring(splitIndex,line.Length - splitIndex).Trim();
                dictionary[name] = value;
            }

            return dictionary;
        }

        private Dictionary<string,int> getNameTable(FieldInfo[] fields) {
            var table = new Dictionary<string,int>();
            for(var i = 0;i < fields.Length;i++) {
                var field = fields[i];
                table[field.Name] = i;
            }
            return table;
        }

        private TPropertySet getPropertySet(Dictionary<string,string> fileProperties) {
            var propertySet = new TPropertySet();
            var nameTable = getNameTable(propertyFields);

            foreach(var item in fileProperties) {
                var propertyName = item.Key;
                if(!nameTable.ContainsKey(propertyName)) {
                    continue;
                }
                var propertyValue = item.Value;
                var fieldIndex = nameTable[propertyName];
                var field = propertyFields[fieldIndex];
                applyProperty(propertySet,field,propertyValue);
            }

            return propertySet;
        }

        private void applyProperty(TPropertySet set,FieldInfo field,string propertyValue) {
            var typeString = field.FieldType.FullName;

            if(!typeParser.HasType(typeString)) {
                return;
            }

            var type = typeParser.GetType(typeString);

            object boxedValue = getPropertyValue(type,propertyValue);
            if(boxedValue == null) {
                return;
            }

            field.SetValue(set,boxedValue);
        }

        private object getPropertyValue(PropertyType type,string value) {
            if(!typeParser.HasType(type)) {
                throw new Exception($"Missing PropertyType parser for type '{Enum.GetName(typeof(PropertyType),type)}'!");
            }
            return typeParser.Parse(type,value);
        }

        public TPropertySet Load(string path) {
            var lines = File.ReadAllLines(path);

            var fileProperties = getFileProperties(lines);

            var propertySet = getPropertySet(fileProperties);

            return propertySet;
        }

        public void Save(string path,TPropertySet propertySet) {
            var builder = new StringBuilder();

            foreach(var field in propertyFields) {
                var propertyName = field.Name;
                builder.Append(propertyName);
                builder.Append(' ');
                builder.Append(VALUE_OPERAND);
                builder.Append(' ');
                builder.Append(field.GetValue(propertySet).ToString());
                builder.Append(Environment.NewLine);
            }
            builder.Remove(builder.Length-1,1);

            var contents = builder.ToString();
            builder.Clear();

            File.WriteAllText(path,contents);
        }
    }
}
