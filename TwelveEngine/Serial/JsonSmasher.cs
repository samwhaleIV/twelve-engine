using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwelveEngine.Serial {
    internal static class JsonSmasher {

        private enum AddressSegmentType {
            Value = 1, Object = 2, Array = 3
        };

        private struct AddressSegment {
            public AddressSegmentType Type;
            public int Index;
            public string Value;
        }

        private static int getArrayItemIndex(char[] characters,ref int searchIndex,StringBuilder builder) {
            int index = -1;
            while(searchIndex < characters.Length && index < 0) {
                var character = characters[searchIndex];
                if(character == SerialFrame.ADDRESS_DELIMITER) {
                    int value;
                    int.TryParse(builder.ToString(),out value);
                    index = value;
                } else {
                    builder.Append(character);
                    searchIndex++;
                }
            }
            builder.Clear();
            return index;
        }

        private static AddressSegment[] getAddressSegments(string address) {
            List<AddressSegment> addressSegments = new List<AddressSegment>();
            var builder = new StringBuilder();
            var characters = address.ToCharArray();
            for(var i = 0;i<characters.Length;i++) {
                var character = characters[i];
                switch(character) {
                    case SerialFrame.ADDRESS_DELIMITER:
                        addressSegments.Add(new AddressSegment() {
                            Type = AddressSegmentType.Object,
                            Value = builder.ToString()
                        });
                        builder.Clear();
                        break;
                    case SerialFrame.ARRAY_PROPERTY_SUFFIX:
                        var value = builder.ToString();
                        builder.Clear();
                        i++;
                        addressSegments.Add(new AddressSegment() {
                            Type = AddressSegmentType.Array,
                            Value = value,
                            Index = getArrayItemIndex(characters,ref i,builder)
                        });
                        break;
                    default:
                        builder.Append(character);
                        break;
                }
            }
            if(builder.Length > 0) {
                addressSegments.Add(new AddressSegment() {
                    Type = AddressSegmentType.Value,
                    Value = builder.ToString()
                });
            }
            return addressSegments.ToArray();
        }

        private static void parseObjectSegment(AddressSegment segment,ref JObject pointer) {
            if(pointer.ContainsKey(segment.Value)) {
                pointer = (JObject)pointer.GetValue(segment.Value);
            } else {
                var newObject = new JObject();
                pointer.Add(segment.Value,newObject);
                pointer = newObject;
            }
        }
        private static void parseArraySegment(AddressSegment segment,ref JObject pointer) {
            if(pointer.ContainsKey(segment.Value)) {
                pointer = (JObject)pointer.GetValue(segment.Value);
            } else {
                var newArray = new JObject();
                pointer.Add(segment.Value,newArray);
                pointer = newArray;
            }
            var stringIndex = segment.Index.ToString();
            if(pointer.ContainsKey(stringIndex)) {
                pointer = (JObject)pointer.GetValue(stringIndex);
            } else {
                var newArrayObject = new JObject();
                pointer.Add(stringIndex,newArrayObject);
                pointer = newArrayObject;
            }
        }
        private static void parseValueSegment(AddressSegment segment,object value,JObject pointer) {
            JToken token;
            if(value == null) {
                token = JsonConvert.Null;
            } else {
                token = JToken.FromObject(value);
            }
            pointer.Add(segment.Value,token);
        }

        private static void parseAddressSegments(AddressSegment[] address,object value,JObject pointer) {
            foreach(var segment in address) {
                switch(segment.Type) {
                    case AddressSegmentType.Object:
                        parseObjectSegment(segment,ref pointer);
                        break;
                    case AddressSegmentType.Array:
                        parseArraySegment(segment,ref pointer);
                        break;
                    case AddressSegmentType.Value:
                        parseValueSegment(segment,value,pointer);
                        break;
                }
            }
        }

        internal static string Smash(Dictionary<string,object> dictionary) {
            JObject rootObject = new JObject();
            foreach(var item in dictionary) {
                AddressSegment[] address = getAddressSegments(item.Key);
                parseAddressSegments(address,item.Value,rootObject);
            }
            return rootObject.ToString(Formatting.None);
        }

        private static void unsmashValue(string key,JToken value,string path,Dictionary<string,object> dictionary) {
            if(path.Length > 0) {
                path += SerialFrame.ADDRESS_DELIMITER;
            }
            path = path + key;
            if(value is JValue) {
                dictionary[path] = ((JValue)value).Value;
            } else if(value is JArray) {
                dictionary[path] = (JArray)value;
            }
        }
        private static void unsmashObject(string key,JToken value,string path,Dictionary<string,object> dictionary) {
            bool isArray = long.TryParse(key,out _);
            if(path.Length > 0) {
                path += isArray ? SerialFrame.ARRAY_PROPERTY_SUFFIX : SerialFrame.ADDRESS_DELIMITER;
            }
            unsmashItems(path + key,(JObject)value,dictionary);
        }

        private static void unsmashItems(string path,JObject root,Dictionary<string,object> dictionary) {
            foreach(var item in root) {
                string key = item.Key;
                JToken value = item.Value;
                if(value is JObject) {
                    unsmashObject(key,value,path,dictionary);
                } else if(value is JValue || item.Value is JArray) {
                    unsmashValue(key,value,path,dictionary);
                }
            }
        }

        internal static Dictionary<string,object> Unsmash(string json) {
            var dictionary = new Dictionary<string,object>();
            JObject rootObject = JObject.Parse(json);
            unsmashItems(string.Empty,rootObject,dictionary);
            return dictionary;
        }
    }
}
