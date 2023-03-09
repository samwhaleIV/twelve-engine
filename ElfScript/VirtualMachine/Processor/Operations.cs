using ElfScript.Errors;
using ElfScript.VirtualMachine.Memory;
using Microsoft.Win32;

namespace ElfScript.VirtualMachine.Processor {
    internal sealed partial class OperationProcessor {

        private void Jump() {
            SetProgramPointer(V1);
        }

        private void JumpConditional() {
            if(!_registers.Conditional.Boolean) {
                return;
            }
            Jump();
        }

        private void JumpDynamic() {
            if(_registers.Jump.Type != Type.Function) {
                throw ErrorFactory.IncorrectJumpRegisterType();
            }
            SetProgramPointer(_registers.Jump.Function.Value);
        }

        private void JumpConditionalDynamic() {
            if(!_registers.Conditional.Boolean) {
                return;
            }
            JumpDynamic();
        }

        private void Add() => APU.Add(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );
        private void Subtract() => APU.Subtract(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );
        private void Multiply() => APU.Multiply(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );
        private void Divide() => APU.Divide(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );
        private void Modulus() => APU.Modulus(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );
        private void Min() => APU.Min(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );
        private void Max() => APU.Max(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Result
        );

        private void StaticToRegister() {
            _registers.Result.Set(_stateMachine.GetStaticValue(V1));
        }

        private void StaticToVariable() {
            StaticRegister staticValue = _stateMachine.GetStaticValue(V1);
            Value value = _stateMachine.Memory.Create(staticValue);
            _stateMachine.SetVariable(V2,value.Address);
        }

        private void RegisterToVariable() {
            Register register = _registers.Get(V1);
            Value value = _stateMachine.Memory.Create(register);
            _stateMachine.SetVariable(V2,value.Address);
        }

        private void RegisterToRegister() {
            _registers.Get(V2).Set(_registers.Get(V1));
        }

        private void VariableToRegister() {
            Value value = _stateMachine.GetVariable(V1);
            Register register = _registers.Get(V2);
            Address address = value.Address;
            _memoryToRegisterConverters[value.Type].Invoke(register,address);
        }

        private void VariableToVariable() {
            Value value = _stateMachine.GetVariable(V1);
            _stateMachine.SetVariable(V2,value.Address);
        }

        private void CreateScope() {
            _stateMachine.SetReturnIndex(ProgramPointer);
            _stateMachine.CreateStackFrame();
            _stateMachine.ClearReturnIndex();
        }

        private void ExitScope() {
            _stateMachine.PopStackFrame();
            SetProgramPointer(_stateMachine.ReturnIndex);
            _stateMachine.ClearReturnIndex();
        }

        private void TypeEqual() {
            _registers.Conditional.Set(_registers.LeftHand == _registers.RightHand);
        }
        private void TypeNotEqual() {
            _registers.Conditional.Set(_registers.LeftHand != _registers.RightHand);
        }
        private void IsGreaterThan() => APU.IsGreaterThan(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Conditional
        );
        private void IsLessThan() => APU.IsLessThan(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Conditional
        );
        private void IsGreaterThanOrEqual() => APU.IsGreaterThanOrEqual(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Conditional
        );
        private void IsLessThanOrEqual() => APU.IsLessThanOrEqual(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Conditional
        );
        private void IsNotEqualTo() => APU.IsNotEqualTo(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Conditional
        );
        private void IsEqualTo() => APU.IsEqualTo(
            _registers.LeftHand,
            _registers.RightHand,
            _registers.Conditional
        );

        private void VariableExists() {
            _registers.Conditional.Set(_stateMachine.VariableExists(V1));
        }
        private void CreateTableVariable() {
            var table = _stateMachine.Memory.CreateTable();
            _stateMachine.SetVariable(V1,table.Address);
        }
        private void CreateListVariable() {
            var list = _stateMachine.Memory.CreateList();
            _stateMachine.SetVariable(V1,list.Address);
        }
        private void MemoryDelete() {
            _stateMachine.DeleteVariable(V1);
        }
        private void GarbageCollect() {
            _stateMachine.Memory.Sweep();
        }
        private void RegisterToCallBuffer() {
            var register = _registers.Get(V1);
            var value = _stateMachine.Memory.Create(register);
            _stateMachine.PushCallingBuffer(value.Address);
        }
        private void VariableToCallBuffer() {
            var value = _stateMachine.GetVariable(V1);
            _stateMachine.PushCallingBuffer(value.Address);
        }
        private void StaticToCallBuffer() {
            var staticRegister = _stateMachine.GetStaticValue(V1);
            var value = _stateMachine.Memory.Create(staticRegister);
            _stateMachine.PushCallingBuffer(value.Address);
        }
        private void CallBufferToVariable() {
            var address = _stateMachine.ReadCallingBuffer();
            _stateMachine.SetVariable(V1,address);
        }
        private void CallBufferToRegister() {
            var address = _stateMachine.ReadCallingBuffer();
            var value = _stateMachine.Memory.Get(address);
            var register = _stateMachine.Registers.Get(V1);
            _memoryToRegisterConverters[value.Type].Invoke(register,address);
        }
    }
}
