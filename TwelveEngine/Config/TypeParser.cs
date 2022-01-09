using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Config {
    internal sealed class TypeParser {

        private static readonly Type keysType = typeof(Keys);

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
            { typeof(string[]).FullName, PropertyType.StringArray },
            { keysType.FullName, PropertyType.XNAKeys }

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
            { PropertyType.StringArray, parseArray },
            { PropertyType.XNAKeys, parseKeys }

        };

        private readonly Dictionary<PropertyType,Func<object,string>> exporters = new Dictionary<PropertyType,Func<object,string>>() {

            { PropertyType.Bool, v => export(v).ToLowerInvariant() },
            { PropertyType.Byte, export},
            { PropertyType.SByte, export},
            { PropertyType.Char, export},
            { PropertyType.Decimal, export},
            { PropertyType.Double, export},
            { PropertyType.Float, export},
            { PropertyType.Int, export},
            { PropertyType.Uint, export},
            { PropertyType.Long, export},
            { PropertyType.ULong, export},
            { PropertyType.Short, export},
            { PropertyType.UShort, export},
            { PropertyType.String, export},
            { PropertyType.StringArray, exportArray },
            { PropertyType.XNAKeys, export },

        };

        private static string export(object value) => value.ToString();


                                                          /* This space is intentional |
                                                                                       V   */
        private static readonly string ARRAY_JOINT = $"{Constants.ConfigArrayDelimiter} "; 

        private static string exportArray(object value) {
            object[] array = value as object[];
            return string.Join(ARRAY_JOINT,array);
        }

        private static object parseKeys(string value) {
            if(!Enum.TryParse(keysType,value,out object cast)) {
                return null;
            }
            return cast;
        }

        private static object parseArray(string value) {
            if(string.IsNullOrEmpty(value)) {
                return null;
            }
            var items = value.Split(Constants.ConfigArrayDelimiter,StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0;i<items.Length;i++) {
                items[i] = items[i].Trim();
            }
            return items;
        }

        public object Parse(PropertyType type,string value) {
            return parsers[type].Invoke(value);
        }

        public string Export(PropertyType type,object value) {
            return exporters[type].Invoke(value);
        }

        public bool HasType(string typeName) => validTypes.ContainsKey(typeName);

        public bool TryGetType(string typeName,out PropertyType type) {
            if(validTypes.ContainsKey(typeName)) {
                type = validTypes[typeName];
                return true;
            }
            type = PropertyType.Invalid;
            return false;
        }

        public object TryParse(string typeName,string value) {
            if(!TryGetType(typeName,out var type)) {
                return null;
            }
            return Parse(type,value);
        }

        public string TryExport(string typeName,object value) {
            if(!TryGetType(typeName,out var type)) {
                return null;
            }
            return Export(type,value);
        }
    }
}
