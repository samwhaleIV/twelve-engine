using TwelveEngine.Serial;
using System.Collections.Generic;
using static System.BitConverter;
using static System.Array;

namespace TwelveEngine {
    public sealed partial class SerialFrame {
        private static readonly bool RequiresByteFlip = !IsLittleEndian;

        public SerialFrame(byte[] data) {
            int i = 0;
            subframeCount = readInt(data,i);
            i += sizeof(int);

            values = new Dictionary<int,Value>();

            while(i < data.Length) {
                int key = readInt(data,i);
                i += sizeof(int); //Key (int)

                Type type = (Type)data[i];
                i += sizeof(byte); //Type (byte)

                Value value;
                if(arrayType.Contains(type)) {
                    value = new Value(type,decodeArray(data,ref i));
                } else if(type == Type.String) {
                    value = new Value(type,decodeString(data,ref i));
                } else {
                    value = new Value(type,decodeValue(data,ref i,type));
                }
                values[key] = value;
            }
        }
        private static byte[] decodeString(byte[] data,ref int i) {
            int stringSize = readInt(data,i);
            i += sizeof(int); //String length in bytes (counted in int)
            var bytes = new byte[stringSize];

            Copy(data,i,bytes,0,stringSize);

            i += stringSize; //Blob
            return bytes;
        }
        private static byte[] decodeArray(byte[] data,ref int i) {
            int arraySize = readInt(data,i);
            i += sizeof(int); //Array size in bytes (counted in int)
            var bytes = new byte[arraySize];

            Copy(data,i,bytes,0,arraySize);
            
            if(RequiresByteFlip) {
                int end = i + arraySize;
                for(int i2 = i;i2<end;i2+=sizeof(int)) {
                    swapInt(data,i2);
                }
            }

            i += arraySize; //Blob
            return bytes;
        }
        private static byte[] decodeValue(byte[] data,ref int i,Type type) {
            var dataSize = typeSizes[type];
            var bytes = new byte[dataSize];

            Copy(data,i,bytes,0,dataSize);

            if(RequiresByteFlip && endianSensitive.Contains(type)) {
                Reverse(bytes,0,dataSize);
            }

            i += dataSize; //Blob
            return bytes;
        }

        private static Dictionary<Type,int> typeSizes = new Dictionary<Type,int>() {
            { Type.Byte, sizeof(byte) },
            { Type.Bool, sizeof(bool) },
            { Type.Int, sizeof(int) },
            { Type.Long, sizeof(long) },
            { Type.Float, sizeof(float) },
            { Type.Double, sizeof(double) }
        };

        private static readonly HashSet<Type> endianSensitive = new HashSet<Type>() {
            Type.Int, Type.Long, Type.Float, Type.Double
        };
        private static readonly HashSet<Type> arrayType = new HashSet<Type>() {
            Type.IntArray, Type.IntArray2D
        };

        private static int readInt(
            byte[] source,int index
        ) {
            if(RequiresByteFlip) {
                swapInt(source,index);
            }
            return ToInt32(source,index);
        }

        private static void swapInt(
            byte[] source,int index
        ) {
            var buffer = source[index];
            source[index] = source[index+3];
            source[index+3] = buffer;
            buffer = source[index+1];
            source[index+1] = source[index+2];
            source[index+2] = buffer;
        }

        private static void writeInt(
            byte[] value,byte[] destination,int index
        ) {
            value.CopyTo(destination,index);
            if(RequiresByteFlip) {
                swapInt(destination,index);
            }
        }

        private static void writeInt(
            int value,byte[] destination,int index
        ) {
            writeInt(GetBytes(value),destination,index);
        }

        public byte[] Export() {
            int dataSize = 0;
            dataSize += sizeof(int); //Subframe count

            foreach(var set in values.Values) {
                dataSize += sizeof(int); //Key (int)
                dataSize += sizeof(byte); //Type (byte)
                var type = set.Type;
                if(arrayType.Contains(type)) {
                    dataSize += sizeof(int); //Array size in bytes (counted in int)
                } else if(type == Type.String) {
                    dataSize += sizeof(int); //String length in bytes (counted in int)
                }
                dataSize += set.Bytes.Length; //Blob
            }
            byte[] data = new byte[dataSize];

            var index = 0;
            writeInt(subframeCount,data,index); //Subframe count
            index += sizeof(int);


            foreach(KeyValuePair<int,Value> set in values) {
                int key = set.Key;
                Value value = set.Value;
                Type type = value.Type;
                byte[] bytes = value.Bytes;

                writeInt(key,data,index); //Key (int)
                index += sizeof(int);

                data[index] = (byte)type; //Type (byte)
                index += sizeof(byte);

                if(arrayType.Contains(type)) {

                    writeInt(bytes.Length,data,index); //Array size in bytes (counted in int)
                    index += sizeof(int);

                    bytes.CopyTo(data,index); //Blob
                    index += bytes.Length;

                    if(!RequiresByteFlip) continue;
                    for(var i = index - bytes.Length;i<index;i+=sizeof(int)) {
                        swapInt(data,i);
                    }
                    continue;
                } else if(type == Type.String) {
                    writeInt(bytes.Length,data,index); //String length in bytes (counted in int)
                    index += sizeof(int); 
                }

                bytes.CopyTo(data,index); //Blob
                if(RequiresByteFlip && endianSensitive.Contains(type)) {
                    Reverse(data,index,bytes.Length);
                }
                index += bytes.Length;
            }

            return data;
        }
    }
}
