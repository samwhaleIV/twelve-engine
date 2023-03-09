using ElfScript.Errors;
using ElfScript.IL;
using ElfScript.VirtualMachine.Memory;

namespace ElfScript.VirtualMachine {
    using static OperationCodeType;

    internal sealed class OperationProcessor {

        public int V1 { get; private set; }
        public int V2 { get; private set; }

        private readonly Dictionary<OperationCodeType,Action> _operations;
        private readonly StateMachine _stateMachine;

        private RegisterSet Registers => _stateMachine.Registers;

        public async Task ExecuteProgram(OperationCode[] program,Func<Task> onInterrupt) {
            int programPointer = 0;
            while(programPointer < program.Length) {
                OperationCode operation = program[programPointer++];
                switch(operation.Type) {
                    case Control_ConditionalJump:
                        if(!Registers.Conditional.Boolean) {
                            continue;
                        }
                        programPointer = operation.V1;
                        continue;
                    case Control_Jump:
                        programPointer = operation.V1;
                        continue;
                    case Control_Interrupt:
                        await onInterrupt.Invoke();
                        continue;
                    case Control_ConditionalJumpDynamic:
                        if(!Registers.Conditional.Boolean) {
                            continue;
                        }
                        if(Registers.Jump.Type != Type.Function) {
                            throw ErrorFactory.IncorrectJumpRegisterType();
                        }
                        break;
                    case Control_JumpDynamic:
                        break;
                    default:
                        ExecuteOperation(operation);
                        continue;
                }
            }
        }

        public void ExecuteOperation(OperationCode opCode) {
            V1 = opCode.V1;
            V2 = opCode.V2;
            if(!_operations.TryGetValue(opCode.Type,out Action? action)) {
                return;
            }
            action.Invoke();
        }

        public OperationProcessor(StateMachine stateMachine) => (_stateMachine, _operations) = (stateMachine, new() {
            { Operation_Add, Add },
            { Operation_Subtract, Subtract },
            { Operation_Multiply, Multiply },
            { Operation_Divide, Divide },
            { Operation_Modulus, Modulus },
            { Operation_Min, Min },
            { Operation_Max, Max },

            { Memory_StaticToMemory, StaticToMemory },
            { Memory_StaticToRegister, StaticToRegister },
            { Memory_RegisterToMemory, RegisterToMemory },
            { Memory_RegisterToRegister, RegisterToRegister },
            { Memory_MemoryToRegister, MemoryToRegister },
            { Memory_MemoryToMemory, MemoryToMemory },

            { Memory_CreateList, CreateList },
            { Memory_CreateTable, CreateTable },

            { Control_EnterBlock, EnterBlock },
            { Control_ExitBlock, ExitBlock },

            { Conditional_TypeEqual, TypeEqual },
            { Conditional_TypeNotEqual, TypeNotEqual },
            { Conditional_IsGreaterThan, IsGreaterThan },
            { Conditional_IsLessThan, IsLessThan },
            { Conditional_IsGreaterThanOrEqual, IsGreaterThanOrEqual },
            { Conditional_IsLessThanOrEqual, IsLessThanOrEqual },
            { Conditional_IsNotEqualTo, IsNotEqualTo },
            { Conditional_IsEqualTo, IsEqualTo },
            { Conditional_MemoryExists, MemoryExists },

            { Memory_DeleteMemory, MemoryDelete },
            { Memory_CollectGarbage, GarbageCollect }
        });

        private void Add() {
            APU.Add(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void Subtract() {
            APU.Subtract(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void Multiply() {
            APU.Multiply(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void Divide() {
            APU.Divide(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void Modulus() {
            APU.Modulus(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void Min() {
            APU.Min(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void Max() {
            APU.Max(
                Registers.LeftHand,
                Registers.RightHand,
                Registers.Result
            );
        }
        private void StaticToRegister() {
            Registers.Result.Set(_stateMachine.GetStaticValue(V1));
        }
        private void StaticToMemory() {
            StaticRegister staticValue = _stateMachine.GetStaticValue(V1);
            Value value = _stateMachine.Memory.Create(staticValue);
            _stateMachine.SetVariable(V2,value.Address);
        }
        private void RegisterToMemory() {
            Register register = Registers.Get(V1);
            Value value = _stateMachine.Memory.Create(register);
            _stateMachine.SetVariable(V2,value.Address);
        }
        private void RegisterToRegister() {
            Registers.Get(V2).Set(Registers.Get(V1));
        }

        private void MemoryToRegister() {
            Value value = _stateMachine.GetVariable(V1);
            Register register = Registers.Get(V2);
            VirtualMemory memory = _stateMachine.Memory;
            Address address = value.Address;
            switch(value.Type) {
                case Type.Boolean:
                    register.Set(memory.GetBoolean(address));
                    break;
                case Type.Integer:
                    register.Set(memory.GetInteger(address));
                    break;
                case Type.Decimal:
                    register.Set(memory.GetDecimal(address));
                    break;
                case Type.Character:
                    register.Set(memory.GetCharacter(address));
                    break;
                case Type.String:
                    register.Set(memory.GetString(address));
                    break;
                case Type.Function:
                    register.Set(memory.GetFunction(address));
                    break;
            }
        }
        private void MemoryToMemory() {
            Value value = _stateMachine.GetVariable(V1);
            _stateMachine.SetVariable(V2,value.Address);
        }
        private void EnterBlock() {
            _stateMachine.CreateStackFrame();
        }
        private void ExitBlock() {
            _stateMachine.PopStackFrame();
        }
        private void TypeEqual() {
            Registers.Conditional.Set(Registers.LeftHand == Registers.RightHand);
        }
        private void TypeNotEqual() {
            Registers.Conditional.Set(Registers.LeftHand != Registers.RightHand);
        }
        private void IsGreaterThan() {
            APU.IsGreaterThan(Registers.LeftHand,Registers.RightHand,Registers.Conditional);
        }
        private void IsLessThan() {
            APU.IsLessThan(Registers.LeftHand,Registers.RightHand,Registers.Conditional);
        }
        private void IsGreaterThanOrEqual() {
            APU.IsGreaterThanOrEqual(Registers.LeftHand,Registers.RightHand,Registers.Conditional);
        }
        private void IsLessThanOrEqual() {
            APU.IsLessThanOrEqual(Registers.LeftHand,Registers.RightHand,Registers.Conditional);
        }
        private void IsNotEqualTo() {
            APU.IsNotEqualTo(Registers.LeftHand,Registers.RightHand,Registers.Conditional);
        }
        private void IsEqualTo() {
            APU.IsEqualTo(Registers.LeftHand,Registers.RightHand,Registers.Conditional);
        }
        private void MemoryExists() {
            Registers.Conditional.Set(_stateMachine.VariableExists(V1));
        }
        private void CreateTable() {
            var table = _stateMachine.Memory.CreateTable();
            _stateMachine.SetVariable(V1,table.Address);
        }
        private void CreateList() {
            var list = _stateMachine.Memory.CreateList();
            _stateMachine.SetVariable(V1,list.Address);
        }
        private void MemoryDelete() {
            _stateMachine.DeleteVariable(V1);
        }
        private void GarbageCollect() {
            _stateMachine.Memory.Sweep();
        }
    }
}
