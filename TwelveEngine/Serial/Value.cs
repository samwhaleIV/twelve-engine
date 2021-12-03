using static System.BitConverter;
using static TwelveEngine.Serial.ArrayConverter;
using static System.Text.Encoding;

namespace TwelveEngine.Serial {
    internal readonly struct Value {

        public readonly Type type;
        public readonly byte[] value;

        public Value(bool value) {
            type = Type.Bool;
            this.value = GetBytes(value);
        }
        public Value(byte value) {
            type = Type.Byte; this.value = GetBytes(value);
        }
        public Value(int value) {
            type = Type.Int;
            this.value = GetBytes(value);
        }
        public Value(long value) {
            type = Type.Long;
            this.value = GetBytes(value);
        }
        public Value(float value) {
            type = Type.Float;
            this.value = GetBytes(value);
        }
        public Value(double value) {
            type = Type.Double;
            this.value = GetBytes(value);
        }

        public bool Bool() {
            return type == Type.Bool ? ToBoolean(value,0) : false;
        }
        public byte Byte() {
            return type == Type.Byte? value[0] : byte.MinValue;
        }
        public int Int() {
            return type == Type.Int ? ToInt32(value,0) : 0;
        }
        public long Long() {
            return type == Type.Long ? ToInt64(value,0) : 0L;
        }
        public float Float() {
            return type == Type.Float ? Int32BitsToSingle(ToInt32(value,0)) : 0F;
        }
        public double Double() {
            return type == Type.Double ? Int64BitsToDouble(ToInt64(value,0)) : 0D;
        }

        public Value(int[] value) {
            type = Type.IntArray;
            this.value = ToBytes(value);
        }
        public Value(int[,] value) {
            type = Type.IntArray2D;
            this.value = ToBytes(value);
        }
        public int[] IntArray() {
            return type == Type.IntArray ? ToIntArray(value) : null;
        }
        public int[,] IntArray2D(int width) {
            return type == Type.IntArray2D ? ToIntArray2D(value,width) : null;
        }

        public Value(string value) {
            type = Type.String;
            this.value = UTF8.GetBytes(value);
        }
        public string String() {
            return type == Type.String ? UTF8.GetString(value) : null;
        }
    }
}
