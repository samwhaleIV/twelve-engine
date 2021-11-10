using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TwelveEngine {
    public interface ISerializable {
        void Export(SerialFrame frame);
        void Import(SerialFrame frame);
    }
    public sealed class SerialFrame {

        const char ADDRESS_DELIMITER = '.';
        const string ARRAY_PROPERTY_SUFFIX = "#";

        const string X_DIMENSION = ARRAY_PROPERTY_SUFFIX + "X";
        const string Y_DIMENSION = ARRAY_PROPERTY_SUFFIX + "Y";

        private readonly Dictionary<string,object> dictionary;
        public SerialFrame() {
            dictionary = new Dictionary<string,object>();
        }
        public SerialFrame(string json) {
            dictionary = JsonConvert.DeserializeObject<Dictionary<string,object>>(json);
            /* TODO Expand nested JSON to addressed table */
        }

        public void Clear() {
            dictionary.Clear();
        }
        public string Export() {
            /* TODO Convert addressed table to nested JSON */
            return JsonConvert.SerializeObject(dictionary);
        }

        private Stack<string> propertyAddress = new Stack<string>();
        private string baseAddress = null;
        private string getAddress(string property) {
            if(baseAddress == null) return property;
            return $"{baseAddress}{ADDRESS_DELIMITER}{property}";
        }
        private string getBaseAddress() {
            switch(propertyAddress.Count) {
                case 0:
                    return null;
                case 1:
                    return propertyAddress.First();
                case 2:
                    return $"{propertyAddress.Last()}{ADDRESS_DELIMITER}{propertyAddress.First()}";
                default:
                    return string.Join(ADDRESS_DELIMITER,propertyAddress.Reverse());
            }
        }
        private void updateBaseAddress() {
            baseAddress = getBaseAddress();
        }

        private void addAddressSegment(string property) {
            propertyAddress.Push(property);
            updateBaseAddress();
        }
        private void popAddressSegment() {
            propertyAddress.Pop();
            updateBaseAddress();
        }

        /* ISerializable Methods For Nested Decoding */
        public void Set(string property,ISerializable value) {
            addAddressSegment(property);
            value.Export(this);
            popAddressSegment();
        }
        public ISerializable Get(string property,ISerializable target) {
            addAddressSegment(property);
            target.Import(this);
            popAddressSegment();
            return target;
        }
        public void Set(string property,ISerializable[] value) {
            string address = getAddress(property);
            Set(property,value.Length);
            for(int i = 0;i < value.Length;i++) {
                addAddressSegment($"{address}{ARRAY_PROPERTY_SUFFIX}{i}");
                value[i].Export(this);
                popAddressSegment();
            }
        }
        public T[] GetArray<T>(string property) where T : ISerializable, new() {
            int arrayLength = GetInt(property);
            T[] array = new T[arrayLength];
            string address = getAddress(property);
            for(int i = 0;i < arrayLength;i++) {
                addAddressSegment($"{address}{ARRAY_PROPERTY_SUFFIX}{i}");
                T newObject = new T();
                newObject.Import(this);
                array[i] = newObject;
                popAddressSegment();
            }
            return array;
        }
        public void GetArray<T>(string property,T[] target) where T : ISerializable, new() {
            T[] array = GetArray<T>(property);
            array.CopyTo(target,0);
        }

        public void Set(string property,double value) {
            if(double.IsNaN(value) || double.IsInfinity(value)) {
                value = 0;
            }
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
        public void Set(string property,int value) {
            dictionary[getAddress(property)] = (long)value;
        }
        public void Set(string property,float value) {
            dictionary[getAddress(property)] = (double)value;
        }

        public void Set(string property,double[] value) {
            dictionary[getAddress(property)] = value;
        }
        public void Set(string property,int[] value) {
            long[] longArray = new long[value.Length];
            for(int i = 0;i < longArray.Length;i++) {
                longArray[i] = value[i];
            }
            dictionary[getAddress(property)] = longArray;
        }
        public void Set(string property,string[] value) {
            dictionary[getAddress(property)] = value;
        }
        public void Set(string property,long[] value) {
            dictionary[getAddress(property)] = value;
        }
        public void Set(string property,float[] value) {
            double[] doubleArray = new double[value.Length];
            for(int i = 0;i < doubleArray.Length;i++) {
                doubleArray[i] = value[i];
            }
            dictionary[getAddress(property)] = doubleArray;
        }
        public void Set(string property,bool[] value) {
            dictionary[getAddress(property)] = value;
        }

        public double GetDouble(string property) {
            object value = dictionary[getAddress(property)];
            if(!(value is double)) value = 0d;
            return (double)value;
        }
        public int GetInt(string property) {
            return (int)GetLong(property);
        }
        public float GetFloat(string property) {
            return (float)GetDouble(property);
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

        private T[] getArray<T>(string property) {
            return ((JArray)dictionary[getAddress(property)]).ToObject<T[]>();
        }
        public int[] GetIntArray(string property) {
            return getArray<int>(property);
        }
        public string[] GetStringArray(string property) {
            return getArray<string>(property);
        }
        public long[] GetLongArray(string property) {
            return getArray<long>(property);
        }
        public bool[] GetBoolArray(string property) {
            return getArray<bool>(property);
        }
        public float[] GetFloatArray(string property) {
            return getArray<float>(property);
        }
        public double[] GetDoubleArray(string property) {
            return getArray<double>(property);
        }        

        private void setArray2D<T>(string property,T[,] value) {
            int xLength = value.GetLength(0);
            int yLength = value.GetLength(1);

            T[] flatArray = new T[value.Length];
            for(int i = 0;i<flatArray.Length;i++) {
                flatArray[i] = value[i % xLength,i / xLength];
            }
            string address = getAddress(property);
            dictionary[address + X_DIMENSION] = xLength;
            dictionary[address + Y_DIMENSION] = yLength;
            dictionary[address] = flatArray;
        }
        public void Set(string property,bool[,] value) {
            setArray2D(property,value);
        }
        public void Set(string property,string[,] value) {
            setArray2D(property,value);
        }
        public void Set(string property,float[,] value) {
            setArray2D(property,value);
        }
        public void Set(string property,double[,] value) {
            setArray2D(property,value);
        }
        public void Set(string property,int[,] value) {
            setArray2D(property,value);
        }
        public void Set(string property,long[,] value) {
            setArray2D(property,value);
        }

        private T[,] getArray2D<T>(string property) {
            T[] flatArray = getArray<T>(property);

            string address = getAddress(property);
            int xLength = (int)(long)dictionary[address + X_DIMENSION]; /* Type down cast */
            int yLength = (int)(long)dictionary[address + Y_DIMENSION];

            if(flatArray.Length != xLength * yLength) {
                return new T[xLength,yLength]; /* Fail safe */
            }

            T[,] array = new T[xLength,yLength];
            for(int i = 0;i<flatArray.Length;i++) {
                array[i % xLength,i / xLength] = flatArray[i];
            }
            return array;
        }
        public int[,] GetIntArray2D(string property) {
            return getArray2D<int>(property);
        }
        public float[,] GetFloatArray2D(string property) {
            return getArray2D<float>(property);
        }
        public double[,] GetDoubleArray2D(string property) {
            return getArray2D<double>(property);
        }
        public bool[,] GetBoolArray2D(string property) {
            return getArray2D<bool>(property);
        }
        public long[,] GetLongArray2D(string property) {
            return getArray2D<long>(property);
        }
        public string[,] GetStringArray2D(string property) {
            return getArray2D<string>(property);
        }

        private void getArray<T>(string property,T[] target) {
            T[] array = getArray<T>(property);
            array.CopyTo(target,0);
        }
        public void GetIntArray(string property,int[] target) {
            getArray(property,target);
        }
        public void GetStringArray(string property,string[] target) {
            getArray(property,target);
        }
        public void GetLongArray(string property,long[] target) {
            getArray(property,target);
        }
        public void GetBoolArray(string property,bool[] target) {
            getArray(property,target);
        }
        public void GetFloatArray(string property,float[] target) {
            getArray(property,target);
        }
        public void GetDoubleArray(string property,double[] target) {
            getArray(property,target);
        }
    }
}
