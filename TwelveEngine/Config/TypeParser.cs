using System;
using System.Collections.Generic;

namespace TwelveEngine.Config {
    internal sealed class TypeParser {

        private readonly char seperator;

        public TypeParser(char arraySeperator) {
            seperator = arraySeperator;
            parsers[PropertyType.StringArray] = parseStringArray;
        }

        private readonly Dictionary<string,PropertyType> validTypes = new Dictionary<string,PropertyType>() {

            { typeof(bool).FullName, PropertyType.Bool },
            { typeof(byte).FullName, PropertyType.Byte },
            { typeof(sbyte).FullName, PropertyType.SByte },
            { typeof(char).FullName, PropertyType.Char },
            { typeof(decimal).FullName, PropertyType.Decimal },
            { typeof(double).FullName, PropertyType.Double },
            { typeof(float).FullName, PropertyType.Float },
            { typeof(int).FullName, PropertyType.Int },
            { typeof(uint).FullName, PropertyType.Uint },
            { typeof(long).FullName, PropertyType.Long },
            { typeof(ulong).FullName, PropertyType.ULong },
            { typeof(short).FullName, PropertyType.Short },
            { typeof(ushort).FullName, PropertyType.UShort },
            { typeof(string).FullName, PropertyType.String },
            { typeof(string[]).FullName, PropertyType.StringArray }

        };

        private readonly Dictionary<PropertyType,Func<string,object>> parsers = new Dictionary<PropertyType,Func<string,object>>() {

            { PropertyType.Bool, v => { if(!bool.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Byte, v => { if(!byte.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.SByte, v => { if(!sbyte.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Char, v => { if(!char.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Decimal, v => { if(!decimal.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Double, v => { if(!double.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Float, v => { if(!float.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Int, v => { if(!int.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Uint, v => { if(!uint.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Long, v => { if(!long.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.ULong, v => { if(!ulong.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.Short, v => { if(!short.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.UShort, v => { if(!ushort.TryParse(v,out var cast)) return null; return cast; }},
            { PropertyType.String, v => { if(string.IsNullOrEmpty(v)) return null; return v; }},

        };

        private string[] parseStringArray(string value) {
            if(string.IsNullOrEmpty(value)) {
                return null;
            }
            var items = value.Split(seperator,StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0;i<items.Length;i++) {
                items[i] = items[i].Trim();
            }
            return items;
        }

        public object Parse(PropertyType type,string value) {
            return parsers[type].Invoke(value);
        }
        public bool HasType(PropertyType type) {
            return parsers.ContainsKey(type);
        }
        public bool HasType(string typeName) {
            return validTypes.ContainsKey(typeName);
        }
        public PropertyType GetType(string typeName) {
            return validTypes[typeName];
        }

    }
}
