using ElfScript.Errors;

namespace ElfScript.VirtualMachine {
    internal sealed class StateMachine {

        private sealed class StackFramePool:Pool<StackFrame> {
            protected override void Reset(StackFrame item) => item.Clear();
            protected override StackFrame CreateNew() => new();
        }

        private readonly StackFrame _globalStackFrame = new();
        private StackFrame _activeStackFrame; /* Initialized to _globalBlockFrame */

        private readonly Stack<StackFrame> _stackFrames = new();

        public VirtualMemory Memory { get; private init; } = new();

        public StateMachine() {
            _stackFrames.Push(_globalStackFrame);
            _activeStackFrame = _globalStackFrame;
        }

        private readonly StackFramePool _stackFramePool = new();

        public void CreateStackFrame() {
            var frame = _stackFramePool.Lease();
            _stackFrames.Push(frame);
            _activeStackFrame = frame;
        }

        public void PopStackFrame() {
            if(_stackFrames.Count <= 1) {
                throw ErrorFactory.StackUnderflow();
            }
            var oldStackFrame = _stackFrames.Pop();
            _stackFramePool.Return(oldStackFrame);
            _activeStackFrame = _stackFrames.Peek();
        }

        private Value _returnRegister;

        public void SetReturnRegisterValue(Value value) {
            _returnRegister = value;
        }

        public Value GetReturnRegisterValue() {
            return _returnRegister;
        }

        public Value GetVariable(string variableName) {
            if(!_activeStackFrame.TryGetValue(variableName,out Address address)) {
                throw ErrorFactory.VariableReferenceError(variableName);
            }
            return Memory.Get(address);
        }

        public void DeleteVariable(string variableName) {
            if(!_activeStackFrame.TryGetValue(variableName,out Address address)) {
                throw ErrorFactory.VariableReferenceError(variableName);
            }
            Memory.Dereference(address);
            _activeStackFrame.Remove(variableName);
        }

        public void SetVariable(string variableName,Address address) {
            if(_activeStackFrame.TryGetValue(variableName,out Address oldAddress)) {
                Memory.Dereference(oldAddress);
            }
            Memory.Reference(address);
            _activeStackFrame[variableName] = address;
        }
    }
}
