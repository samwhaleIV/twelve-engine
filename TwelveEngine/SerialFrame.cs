using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace TwelveEngine {
    public sealed class SerialFrame {

        const char ADDRESS_DELIMITER = '.';

        private readonly Dictionary<string,object> dictionary;
        public SerialFrame() {
            dictionary = new Dictionary<string,object>();
        }
        public SerialFrame(string json) {
            dictionary = JsonConvert.DeserializeObject<Dictionary<string,object>>(json);
        }

        private Stack<string> propertyAddress = new Stack<string>();

        private string getAddress(string property) {
            propertyAddress.Push(property);
            var addressComponents = propertyAddress.Reverse();
            string address = string.Join(ADDRESS_DELIMITER,addressComponents);
            propertyAddress.Pop();
            return address;
        }
        public void Set(string property,ISerializable value) {
            propertyAddress.Push(property);
            value.Export(this);
            propertyAddress.Pop();
        }
        public ISerializable GetSerializable(string property,ISerializable target) {
            propertyAddress.Push(property);
            target.Import(this);
            propertyAddress.Pop();
            return target;
        }
        public void Set(string property,double value) {
            dictionary[getAddress(property)] = value;
        }
        public void Set(string property,long value) {
            dictionary[getAddress(property)] = value;
        }
        public void Set(string property,string value) {
            dictionary[getAddress(property)] = value;
        }
        public void Set(string property,bool value) {
            dictionary[getAddress(property)] = value;
        }
        public double GetDouble(string property) {
            return (double)dictionary[getAddress(property)];
        }
        public long GetLong(string property) {
            return (long)dictionary[getAddress(property)];
        }
        public string GetString(string property) {
            return (string)dictionary[getAddress(property)];
        }
        public bool GetBool(string property) {
            return (bool)dictionary[getAddress(property)];
        }
        public void Clear() {
            dictionary.Clear();
        }
        public string Export() {
            return JsonConvert.SerializeObject(dictionary);
        }
    }
    public interface ISerializable {
        void Export(SerialFrame frame);
        void Import(SerialFrame frame);
    }
}
