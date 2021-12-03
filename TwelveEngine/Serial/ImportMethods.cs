using TwelveEngine.Serial;

namespace TwelveEngine {
    public sealed partial class SerialFrame {
        public void Set(bool value) => set(new Value(value));
        public void Set(byte value) => set(new Value(value));
        public void Set(int value) => set(new Value(value));
        public void Set(long value) => set(new Value(value));
        public void Set(float value) => set(new Value(value));
        public void Set(double value) => set(new Value(value));
        public void Set(int[] value) => set(new Value(value));
        public void Set(string value) => set(new Value(value));

        public void Set(int[,] value) {
            set(new Value(value.GetLength(0)));
            set(new Value(value));
        }
    }
}
