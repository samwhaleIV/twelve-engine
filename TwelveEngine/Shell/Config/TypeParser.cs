﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Config {
    public static class TypeParser {

        private static readonly Type keysType = typeof(Keys);

        private static readonly Dictionary<PropertyType,(string TypeName,Func<string,object> Objectify,Func<object,string> Stringify)> types =
            new() {

            { PropertyType.Bool, (typeof(bool).FullName, i => {
                if(!bool.TryParse(i,out var o)) return null; return o;
            }, i => Export(i).ToLowerInvariant())},

            { PropertyType.Byte, (typeof(byte).FullName, i => {
                if(!byte.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.SByte, (typeof(sbyte).FullName, i => {
                if(!sbyte.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Char, (typeof(char).FullName, i => {
                if(!char.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Decimal, (typeof(decimal).FullName, i => {
                if(!decimal.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Double, (typeof(double).FullName, i => {
                if(!double.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Float, (typeof(float).FullName, i => {
                if(!float.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Int, (typeof(int).FullName, i => {
                if(!int.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Uint, (typeof(uint).FullName, i => {
                if(!uint.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Long, (typeof(long).FullName, i => {
                if(!long.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.ULong, (typeof(ulong).FullName, i => {
                if(!ulong.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.Short, (typeof(short).FullName, i => {
                if(!short.TryParse(i,out var o)) return null; return o;
                }, Export)},

            { PropertyType.UShort, (typeof(ushort).FullName, i => {
                if(!ushort.TryParse(i,out var o)) return null; return o;
            }, Export)},

            { PropertyType.String, (typeof(string).FullName, i => {
                if(string.IsNullOrEmpty(i)) return null; return i;
            }, Export)},

            { PropertyType.StringArray, (typeof(string[]).FullName, ParseArray, ExportArray)},
            { PropertyType.XNAKeys, (keysType.FullName, ParseKeys, Export)}

        };

        private static Dictionary<string,PropertyType> GetValidTypes() {
            var validTypes = new Dictionary<string,PropertyType>();
            foreach(var type in types) {
                var propertyType = type.Key;
                var typeName = type.Value.TypeName;

                validTypes.Add(typeName,propertyType);
            }
            return validTypes;
        }

        private static readonly Dictionary<string,PropertyType> validTypes = GetValidTypes();

        private static string Export(object value) => value.ToString();

                                                          /* This space is intentional |
                                                                                       V   */
        private static readonly string ARRAY_JOINT = $"{Constants.ConfigArrayDelimiter} "; 

        private static string ExportArray(object value) {
            return string.Join(ARRAY_JOINT,(object[])value);
        }

        private static object ParseKeys(string value) {
            if(!Enum.TryParse(keysType,value,out object cast)) {
                return null;
            }
            return cast;
        }

        private static object ParseArray(string value) {
            if(string.IsNullOrEmpty(value)) {
                return null;
            }
            var items = value.Split(Constants.ConfigArrayDelimiter,StringSplitOptions.RemoveEmptyEntries);
            for(int i = 0;i<items.Length;i++) {
                items[i] = items[i].Trim();
            }
            return items;
        }

        public static object Parse(PropertyType type,string value) {
            return types[type].Objectify(value);
        }

        public static string Export(PropertyType type,object value) {
            return types[type].Stringify(value);
        }

        public static bool HasType(string typeName) => validTypes.ContainsKey(typeName);

        public static bool TryGetType(string typeName,out PropertyType type) {
            if(validTypes.ContainsKey(typeName)) {
                type = validTypes[typeName];
                return true;
            }
            type = PropertyType.Invalid;
            return false;
        }

        public static object TryParse(string typeName,string value) {
            if(!TryGetType(typeName,out var type)) {
                return null;
            }
            return Parse(type,value);
        }

        public static string TryExport(string typeName,object value) {
            if(!TryGetType(typeName,out var type)) {
                return null;
            }
            return Export(type,value);
        }
    }
}
