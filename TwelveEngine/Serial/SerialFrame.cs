using System.Collections.Generic;
using TwelveEngine.Serial;

namespace TwelveEngine {
    public sealed partial class SerialFrame {

        public SerialFrame() => values = new Dictionary<int,Value>();

        private const int SUBFRAME_STRIDE = 1024;
        private const int MAX_SUBFRAMES = 2^32 / 2 / SUBFRAME_STRIDE;

        private readonly Dictionary<int,Value> values;

        private int address = 0;
        private int IDPointer = 0;
        private int subframeCount = 0;

        public void StartReadback() {
            address = 0;
            IDPointer = 0;
        }

        private int getNextID() {
            var ID = IDPointer;
            IDPointer += 1;
            return ID;
        }

        private void set(Value value) {
            values[getNextID() + address] = value;
        }
        private Value get() {
            return values[getNextID() + address];
        }

        /* Allows for recursive serialisation without local scope pollution */
        private readonly Stack<(int address,int pointer)> nestStack = new Stack<(int,int)>();
        private void setAddress(int newAddress) {
            nestStack.Push((address,IDPointer));
            address = newAddress;
            IDPointer = 0;
        }
        private void restoreAddress() {
            var frame = nestStack.Pop();
            address = frame.address;
            IDPointer = frame.pointer;
        }

        public void Get(ISerializable serializable) {
            var targetBaseAddress = get().Int();

            setAddress(targetBaseAddress);
            serializable.Import(this);
            restoreAddress();
        }

        public T Get<T>() where T:ISerializable, new() {
            var item = new T();
            Get(item);
            return item;
        }

        public void Set(ISerializable serializable) {
            subframeCount += 1;

            var targetBaseAddress = subframeCount * SUBFRAME_STRIDE;
            set(new Value(targetBaseAddress));

            setAddress(targetBaseAddress);
            serializable.Export(this);
            restoreAddress();
        }
    }
}
