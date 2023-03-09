using ElfScript.Errors;
using ElfScript.VirtualMachine.Memory;

namespace ElfScript.VirtualMachine {
    internal sealed class StateMachine {

        private sealed class StackFramePool:Pool<StackFrame> {
            protected override void Reset(StackFrame item) => item.Clear();
            protected override StackFrame CreateNew() => new();
        }

        private readonly StackFrame _globalStackFrame = new();

        /// <remarks>Initialized to <see cref="_globalStackFrame"/>.</remarks>
        private StackFrame _activeStackFrame;

        private readonly Stack<StackFrame> _stackFrames = new();

        public VirtualMemory Memory { get; private init; } = new();
        public RegisterSet Registers { get; private init; } = new();
        public OperationProcessor Processor { get; private init; }

        public StateMachine() {
            _stackFrames.Push(_globalStackFrame);
            _activeStackFrame = _globalStackFrame;
            Processor = new(this);
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

        public Value GetVariable(int variableID) {
            if(!_activeStackFrame.TryGetValue(variableID,out Address address)) {
                throw ErrorFactory.VariableReferenceError(variableID);
            }
            return Memory.Get(address);
        }

        public bool VariableExists(int variableID) {
            return _activeStackFrame.ContainsKey(variableID);
        }

        public void DeleteVariable(int variableID) {
            if(!_activeStackFrame.TryGetValue(variableID,out Address address)) {
                throw ErrorFactory.VariableReferenceError(variableID);
            }
            Memory.Dereference(address);
            _activeStackFrame.Remove(variableID);
        }

        public void SetVariable(int variableID,Address address) {
            if(_activeStackFrame.TryGetValue(variableID,out Address oldAddress)) {
                Memory.Dereference(oldAddress);
            }
            Memory.Reference(address);
            _activeStackFrame[variableID] = address;
        }

        private readonly Dictionary<int,StaticRegister> _staticValues = new();

        public void AddStaticValue(int ID,StaticRegister staticRegister) {
            _staticValues[ID] = staticRegister;
        }

        public StaticRegister GetStaticValue(int ID) {
            return _staticValues[ID];
        }
    }
}
