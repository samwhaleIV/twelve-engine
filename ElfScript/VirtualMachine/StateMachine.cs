using ElfScript.Errors;
using ElfScript.VirtualMachine.Memory;
using ElfScript.VirtualMachine.Operations;

namespace ElfScript.VirtualMachine {
    internal sealed class StateMachine {

        private sealed class StackFramePool:Pool<StackFrame> {
            protected override void Reset(StackFrame item) => item.Clear();
            protected override StackFrame CreateNew() => new();
        }

        /// <remarks>Don't give this to anyone else... This helps memory safety. </remarks>
        private readonly ExecutionHandshake _executionHandshake;

        private readonly StackFrame _globalStackFrame = new();

        /// <remarks>Initialized to <see cref="_globalStackFrame"/>.</remarks>
        private StackFrame _activeStackFrame;

        private readonly Stack<StackFrame> _stackFrames = new();

        public VirtualMemory Memory { get; private init; } = new();

        public StateMachine() {
            _stackFrames.Push(_globalStackFrame);
            _activeStackFrame = _globalStackFrame;
            _executionHandshake = ExecutionHandshake.Create(this);
        }

        ~StateMachine() => ExecutionHandshake.Delete(_executionHandshake);

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

        public async Task Execute(Operation operation) {
            ShouldExitBlock = false;
            await operation.Execute(_executionHandshake);
            Memory.Sweep();
        }

        public async Task Execute(FunctionDeclaration function,Operation[] arguments) {
            ShouldExitBlock = false;
            await function.Invoke(this,arguments);
            Memory.Sweep();
        }

        public bool ShouldExitBlock { get; set; } = false;

        private Value? _valueRegister;
        public Value ValueRegister {
            get {
                if(!_valueRegister.HasValue) {
                    throw ErrorFactory.OperationResultNullReference();
                }
                return _valueRegister.Value;
            }
            set {
                if(_valueRegister.HasValue) {
                    Memory.Dereference(_valueRegister.Value.Address);
                }
                _valueRegister = value;
                Memory.Reference(value.Address);
            }
        }
    }
}
