namespace TwelveEngine {
    public sealed partial class SerialFrame {
        public bool GetBool() => get().Bool();
        public byte GetByte() => get().Byte();
        public int GetInt() => get().Int();
        public long GetLong() => get().Long();
        public float GetFloat() => get().Float();
        public double GetDouble() => get().Double();
        public string GetString() => get().String();

        public void Get(ref bool target) => target = get().Bool();
        public void Get(ref byte target) => target = get().Byte();
        public void Get(ref int target) => target = get().Int();
        public void Get(ref long target) => target = get().Long();
        public void Get(ref float target) => target = get().Float();
        public void Get(ref double target) => target = get().Double();
        public void Get(ref string target) => target = get().String();

        public int[] GetIntArray() => get().IntArray();
        public int[,] GetIntArray2D() {
            var width = GetInt();
            return get().IntArray2D(width);
        }
    }
}
