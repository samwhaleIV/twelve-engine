using TwelveEngine.Serial;
using System.Collections.Generic;
using static System.BitConverter;
using static System.Array;

namespace TwelveEngine {
    public sealed partial class SerialFrame {
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

        private void import(byte[] data) {
            int i = 0;
            subframeCount = readInt(data,i);
            i += sizeof(int); ////Subframe count

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

        public byte[] Export() {

            /* 'values' is the dictionary that contains the (Type,byte[]) struct */
            byte[] data = new byte[getDataSize(values.Values)];

            var i = 0;
            writeInt(subframeCount,data,i); //Subframe count
            i += sizeof(int);

            foreach(var set in values) {
                Type type = set.Value.Type;
                byte[] bytes = set.Value.Bytes;

                encodeValue(data,set.Key,type,bytes,ref i);
            }
            return data;
        }

        private byte[] decodeString(byte[] data,ref int i) {
            int stringSize = readInt(data,i);
            i += sizeof(int); //String length in bytes (counted in int)
            var bytes = new byte[stringSize];

            Copy(data,i,bytes,0,stringSize);

            i += stringSize; //Blob
            return bytes;
        }

        private byte[] decodeArray(byte[] data,ref int i) {
            int arraySize = readInt(data,i);
            i += sizeof(int); //Array size in bytes (counted in int)
            var bytes = new byte[arraySize];
       
            if(!RequiresByteFlip) {
                Copy(data,i,bytes,0,bytes.Length);
            } else {
                for(int x = 0;x<bytes.Length;x+=sizeof(int)) {
                    int index = i + x;
                    bytes[x+3] = data[index];
                    bytes[x+2] = data[index+1];
                    bytes[x+1] = data[index+2];
                    bytes[x] = data[index+3];
                }
            }

            i += bytes.Length; //Blob
            return bytes;
        }

        private byte[] decodeValue(byte[] data,ref int i,Type type) {
            var dataSize = typeSizes[type];
            var bytes = new byte[dataSize];

            Copy(data,i,bytes,0,dataSize);

            if(RequiresByteFlip && endianSensitive.Contains(type)) {
                Reverse(bytes,0,dataSize);
            }

            i += dataSize; //Blob
            return bytes;
        }

        private void writeInt(
            int value,byte[] destination,int index
        ) {
            writeInt(GetBytes(value),destination,index);
        }

        private void encodeArray(byte[] data,byte[] bytes,ref int i) {
            writeInt(bytes.Length,data,i); //Array size in bytes (counted in int)
            i += sizeof(int);

            if(!RequiresByteFlip) {
                bytes.CopyTo(data,i); //Blob
            } else {
                for(int x = 0;x<bytes.Length;x+=sizeof(int)) {
                    int index = i + x;
                    data[index] = bytes[x+3];
                    data[index+1] = bytes[x+2];
                    data[index+2] = bytes[x+1];
                    data[index+3] = bytes[x];
                }
            }
            i += bytes.Length;
        }

        private void encodeValue(byte[] data,int key,Type type,byte[] bytes, ref int i) {
            writeInt(key,data,i); //Key (int)
            i += sizeof(int);

            data[i] = (byte)type; //Type (byte)
            i += sizeof(byte);

            if(arrayType.Contains(type)) {
                encodeArray(data,bytes,ref i);
                return;
            } else if(type == Type.String) {
                writeInt(bytes.Length,data,i); //String length in bytes (counted in int)
                i += sizeof(int);
            }

            bytes.CopyTo(data,i); //Blob
            if(RequiresByteFlip && endianSensitive.Contains(type)) {
                Reverse(data,i,bytes.Length);
            }
            i += bytes.Length;
        }

        private int readInt(
            byte[] source,int i
        ) {
            var bytes = new byte[sizeof(int)];
            if(RequiresByteFlip) {
                bytes[0] = source[i+3];
                bytes[1] = source[i+2];
                bytes[2] = source[i+1];
                bytes[3] = source[i];
            } else {
                bytes[0] = source[i];
                bytes[1] = source[i+1];
                bytes[2] = source[i+2];
                bytes[3] = source[i+3];
            }
            return ToInt32(bytes,0);
        }

        private void writeInt(
            byte[] value,byte[] destination,int i
        ) {
            if(RequiresByteFlip) {
                destination[i] = value[3];
                destination[i+1] = value[2];
                destination[i+2] = value[1];
                destination[i+3] = value[0];
            } else {
                destination[i] = value[0];
                destination[i+1] = value[1];
                destination[i+2] = value[2];
                destination[i+3] = value[3];
            }
        }

        private static int getDataSize(Dictionary<int,Value>.ValueCollection values) {
            int dataSize = 0;
            dataSize += sizeof(int); //Subframe count

            foreach(var set in values) {
                dataSize += sizeof(int); //Key (int)
                dataSize += sizeof(byte); //Type (byte)
                Type type = set.Type;
                if(arrayType.Contains(type)) {
                    dataSize += sizeof(int); //Array size in bytes (counted in int)
                } else if(type == Type.String) {
                    dataSize += sizeof(int); //String length in bytes (counted in int)
                }
                dataSize += set.Bytes.Length; //Blob
            }
            return dataSize;
        }
    }
}
