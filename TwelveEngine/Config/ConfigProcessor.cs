using System;
using System.Text;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

namespace TwelveEngine.Config {
    public sealed class ConfigProcessor<TPropertySet> where TPropertySet : new() {

        private readonly TypeParser typeParser = new TypeParser();

        private readonly Type propertySetType;
        private readonly FieldInfo[] propertyFields;

        public ConfigProcessor() {
            propertySetType = typeof(TPropertySet);
            propertyFields = propertySetType.GetFields(BindingFlags.Instance | BindingFlags.Public);
        }

        private Dictionary<string,string> getFileProperties(string[] lines) {
            var dictionary = new Dictionary<string,string>();
            foreach(var line in lines) {
                int splitIndex = line.IndexOf(Constants.ConfigValueOperand);
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
            var typeName = field.FieldType.FullName;

            if(!typeParser.TryGetType(typeName,out var type)) {
                return;
            }

            object boxedValue = typeParser.Parse(type,propertyValue);
            if(boxedValue == null) {
                return;
            }

            field.SetValue(set,boxedValue);
        }

        public TPropertySet Load(string path) {
            var lines = File.ReadAllLines(path);
            var fileProperties = getFileProperties(lines);
            return getPropertySet(fileProperties);
        }

        private bool tryGetValue(TPropertySet propertySet,FieldInfo field,out string value) {
            if(!typeParser.TryGetType(field.FieldType.FullName,out var type)) {
                value = null;
                return false;
            }
            var fieldValue = field.GetValue(propertySet);

            if(fieldValue == null) {
                value = null;
                return false;
            }
            value = typeParser.Export(type,fieldValue);
            return true;
        }

        public void Save(string path,TPropertySet propertySet) {
            var builder = new StringBuilder();

            foreach(var field in propertyFields) {

                if(!tryGetValue(propertySet,field,out string value)) continue;

                var propertyName = field.Name;
                builder.Append(propertyName);
                builder.Append(' ');
                builder.Append(Constants.ConfigValueOperand);
                builder.Append(' ');
                builder.Append(value);
                builder.Append(Environment.NewLine);
            }

            if(builder.Length < 1) {
                File.WriteAllText(path,string.Empty);
                return;
            }

            builder.Remove(builder.Length-1,1);

            var contents = builder.ToString();
            builder.Clear();

            File.WriteAllText(path,contents);
        }
    }
}
