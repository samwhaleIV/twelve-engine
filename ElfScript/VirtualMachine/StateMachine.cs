using ElfScript.Errors;

namespace ElfScript.VirtualMachine {
    internal sealed class StateMachine {

        private sealed class StackFramePool:Pool<StackFrame> {
            protected override void Reset(StackFrame item) => item.Clear();
            protected internal override StackFrame CreateNew() => new();
        }

        private readonly Dictionary<string,Address> _globalStackFrame = new();
        private Dictionary<string,Address> _activeStackFrame; /* Initialized to _globalBlockFrame */

        private readonly Stack<Dictionary<string,Address>> stackFrames = new();

        public VirtualMemory Memory { get; private init; } = new();

        public StateMachine() {
            stackFrames.Push(_globalStackFrame);
            _activeStackFrame = _globalStackFrame;
        }

        private readonly StackFramePool _stackFramePool = new();

        public void CreateStackFrame() {
            var frame = _stackFramePool.Lease();
            stackFrames.Push(frame);
            _activeStackFrame = frame;
        }

        public void PopStackFrame() {
            if(stackFrames.Count <= 1) {
                throw ErrorFactory.StackUnderflow();
            }
            foreach(Address address in _activeStackFrame.Values) {
                Memory.RemovePin(address);
            }
            stackFrames.Pop();
            _activeStackFrame = stackFrames.Peek();
            Memory.AddPin(_returnRegister.Address);
            Memory.CleanUp();
        }

        private Value _returnRegister;

        public void SetReturnRegisterValue(Value value) {
            _returnRegister = value;
        }

        public Value GetReturnRegisterValue() {
            return _returnRegister;
        }

        public Value GetValue(string variableName) {
            if(!_activeStackFrame.TryGetValue(variableName,out Address address)) {
                throw ErrorFactory.VariableReferenceError(variableName);
            }
            return Memory.Get(address);
        }

        public void DeleteValue(string variableName) {
            if(!_activeStackFrame.TryGetValue(variableName,out Address address)) {
                throw ErrorFactory.VariableReferenceError(variableName);
            }
            Memory.RemovePin(address);
            Memory.Delete(address);
            _activeStackFrame.Remove(variableName);
        }

        public void SetValue(string variableName,Address address) {
            _activeStackFrame[variableName] = address;
            Memory.AddPin(address);
        }
    }
}
