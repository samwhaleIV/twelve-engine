using System.Collections.Generic;
using TwelveEngine.Serial.Binary;

namespace TwelveEngine.Serial {
    public sealed partial class SerialFrame {

        private readonly int subframeSize;
        public int SubframeSize => subframeSize;

        public SerialFrame() {
            subframeSize = Constants.SerialSubframeSize;
        }

        public SerialFrame(int subframeSize) {
            this.subframeSize = subframeSize;
        }

        internal SerialFrame(int subframeCount,int subframeSize = Constants.SerialSubframeSize) {
            this.subframeCount = subframeCount;
            this.subframeSize = subframeSize;
        }

        private readonly Dictionary<int,Value> values = new Dictionary<int,Value>();
        internal Dictionary<int,Value> Values => values;

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

            var targetBaseAddress = subframeCount * subframeSize;
            set(new Value(targetBaseAddress));

            setAddress(targetBaseAddress);
            serializable.Export(this);
            restoreAddress();
        }
    }
}
