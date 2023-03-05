using ElfScript.Errors;

namespace ElfScript {
    internal sealed class StateMachine {

        private sealed class StackFrame:Dictionary<string,int> { public int PoolID { get; set; } }

        private sealed class StackFramePool:Pool<StackFrame> {
            protected override void Reset(StackFrame item) => item.Clear();
            protected internal override StackFrame CreateNew() => new();
        }

        private readonly Dictionary<string,int> _globalStackFrame = new();
        private Dictionary<string,int> _activeStackFrame; /* Initialized to _globalBlockFrame */

        private readonly Stack<Dictionary<string,int>> stackFrames = new();

        public VirtualMemory Memory { get; private init; } = new();

        public StateMachine() {
            stackFrames.Push(_globalStackFrame);
            _activeStackFrame = _globalStackFrame;
        }

        private readonly StackFramePool _stackFramePool = new();
    
        public void CreateStackFrame() {
            var poolID = _stackFramePool.Lease(out var newFrame);
            newFrame.PoolID = poolID;
            stackFrames.Push(newFrame);
            _activeStackFrame = newFrame;
        }

        public void PopStackFrame() {
            if(stackFrames.Count <= 1) {
                throw ErrorFactory.StackUnderflow();
            }
            foreach(var ID in _activeStackFrame.Values) {
                Memory.RemovePin(ID);
            }
            stackFrames.Pop();
            _activeStackFrame = stackFrames.Peek();
            Memory.AddPin(_returnRegister.ID);
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
            if(!_activeStackFrame.TryGetValue(variableName,out var value)) {
                throw ErrorFactory.VariableReferenceError(variableName);
            }
            return Memory.Get(value);
        }

        public void DeleteValue(string variableName) {
            if(!_activeStackFrame.TryGetValue(variableName,out var value)) {
                throw ErrorFactory.VariableReferenceError(variableName);
            }
            Memory.RemovePin(value);
            Memory.Delete(value);
            _activeStackFrame.Remove(variableName);
        }

        public void SetValue(string variableName,int ID) {
            _activeStackFrame[variableName] = ID;
            Memory.AddPin(ID);
        }
    }
}
