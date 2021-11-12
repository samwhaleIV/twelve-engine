using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using TwelveEngine.Serial;

namespace TwelveEngine {
    public sealed class SerialFrame {

        internal const char ADDRESS_DELIMITER = '.';
        internal const char ARRAY_PROPERTY_SUFFIX = '#';

        private const char SIZE_PREFIX = '~';

        private readonly Dictionary<string,object> dictionary;
        public SerialFrame() {
            dictionary = new Dictionary<string,object>();
        }
        public SerialFrame(string json) {
            dictionary = JsonSmasher.Unsmash(json);
        }

        public void Clear() {
            dictionary.Clear();
        }
        public string Export() {
            return JsonSmasher.Smash(dictionary);
        }

        private Stack<string> propertyAddress = new Stack<string>();
        private string baseAddress = null;
        private string getAddress(string property) {
            if(baseAddress == null)
                return property;
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
            Set(SIZE_PREFIX + property,value.Length);
            for(int i = 0;i < value.Length;i++) {
                addAddressSegment($"{property}{ARRAY_PROPERTY_SUFFIX}{i}");
                value[i].Export(this);
                popAddressSegment();
            }
        }
        public T[] GetArray<T>(string property) where T : ISerializable, new() {
            int arrayLength = GetInt(SIZE_PREFIX + property);
            T[] array = new T[arrayLength];
            for(int i = 0;i < arrayLength;i++) {
                T newObject = new T();
                addAddressSegment($"{property}{ARRAY_PROPERTY_SUFFIX}{i}");
                newObject.Import(this);
                popAddressSegment();
                array[i] = newObject;
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

        private void set<T>(string property,T[] value) {
            dictionary[getAddress(property)] = JArray.FromObject(value);
        }
        public void Set(string property,double[] value) {
            set(property,value);
        }
        public void Set(string property,int[] value) {
            set(property,value);
        }
        public void Set(string property,string[] value) {
            set(property,value);
        }
        public void Set(string property,long[] value) {
            set(property,value);
        }
        public void Set(string property,float[] value) {
            set(property,value);
        }
        public void Set(string property,bool[] value) {
            set(property,value);
        }

        public double GetDouble(string property) {
            object value = dictionary[getAddress(property)];
            if(!(value is double))
                value = 0d;
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
            T[] flatArray = new T[value.Length];
            for(int i = 0;i < flatArray.Length;i++) {
                flatArray[i] = value[i % xLength,i / xLength];
            }
            dictionary[getAddress(SIZE_PREFIX + property)] = xLength;
            dictionary[getAddress(property)] = JArray.FromObject(flatArray);
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

            int xLength = (int)(long)dictionary[getAddress(SIZE_PREFIX + property)]; /* Type down cast */
            if(xLength == 0) {
                return new T[0,0]; /* Fail safe */
            }
            int yLength = flatArray.Length / xLength;

            if(flatArray.Length != xLength * yLength) {
                return new T[xLength,yLength]; /* Fail safe */
            }

            T[,] array = new T[xLength,yLength];
            for(int i = 0;i < flatArray.Length;i++) {
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

        public void SetArray2D<T>(string property,T[,] grid) where T : struct {
            setArray2D(property,grid);
        }
        public T[,] GetArray2D<T>(string property) where T : struct {
            return getArray2D<T>(property);
        }
    }
}
