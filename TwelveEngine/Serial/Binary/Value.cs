using static System.BitConverter;
using static TwelveEngine.Serial.Binary.ArrayConverter;
using static System.Text.Encoding;

namespace TwelveEngine.Serial.Binary {
    internal readonly struct Value {

        public readonly Type Type;
        public readonly byte[] Bytes;

        public Value(Type type,byte[] bytes) {
            Type = type;
            Bytes = bytes;
        }

        public Value(bool value) {
            Type = Type.Bool;
            Bytes = GetBytes(value);
        }
        public Value(byte value) {
            Type = Type.Byte; Bytes = GetBytes(value);
        }
        public Value(int value) {
            Type = Type.Int;
            Bytes = GetBytes(value);
        }
        public Value(long value) {
            Type = Type.Long;
            Bytes = GetBytes(value);
        }
        public Value(float value) {
            Type = Type.Float;
            Bytes = GetBytes(value);
        }
        public Value(double value) {
            Type = Type.Double;
            Bytes = GetBytes(value);
        }

        public bool Bool() {
            return Type == Type.Bool ? ToBoolean(Bytes,0) : false;
        }
        public byte Byte() {
            return Type == Type.Byte? Bytes[0] : byte.MinValue;
        }
        public int Int() {
            return Type == Type.Int ? ToInt32(Bytes,0) : 0;
        }
        public long Long() {
            return Type == Type.Long ? ToInt64(Bytes,0) : 0L;
        }
        public float Float() {
            return Type == Type.Float ? Int32BitsToSingle(ToInt32(Bytes,0)) : 0F;
        }
        public double Double() {
            return Type == Type.Double ? Int64BitsToDouble(ToInt64(Bytes,0)) : 0D;
        }

        public Value(int[] value) {
            Type = Type.IntArray;
            Bytes = ToBytes(value);
        }
        public Value(int[,] value) {
            Type = Type.IntArray2D;
            Bytes = ToBytes(value);
        }
        public int[] IntArray() {
            return Type == Type.IntArray ? ToIntArray(Bytes) : null;
        }
        public int[,] IntArray2D(int width) {
            return Type == Type.IntArray2D ? ToIntArray2D(Bytes,width) : null;
        }

        public Value(string value) {
            Type = Type.String;
            Bytes = UTF8.GetBytes(value);
        }
        public string String() {
            return Type == Type.String ? UTF8.GetString(Bytes) : null;
        }
    }
}
