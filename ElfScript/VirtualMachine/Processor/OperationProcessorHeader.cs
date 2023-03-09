using ElfScript.IL;

namespace ElfScript.VirtualMachine.Processor {
    using static OperationCodeType;
    internal sealed partial class OperationProcessor {
        public OperationProcessor(StateMachine stateMachine) => (_stateMachine, _operations, _memoryToRegisterConverters) = (stateMachine, new() {
            { Operation_Add, Add },
            { Operation_Subtract, Subtract },
            { Operation_Multiply, Multiply },
            { Operation_Divide, Divide },
            { Operation_Modulus, Modulus },
            { Operation_Min, Min },
            { Operation_Max, Max },

            { Conditional_TypeEqual, TypeEqual },
            { Conditional_TypeNotEqual, TypeNotEqual },
            { Conditional_IsGreaterThan, IsGreaterThan },
            { Conditional_IsLessThan, IsLessThan },
            { Conditional_IsGreaterThanOrEqual, IsGreaterThanOrEqual },
            { Conditional_IsLessThanOrEqual, IsLessThanOrEqual },
            { Conditional_IsNotEqualTo, IsNotEqualTo },
            { Conditional_IsEqualTo, IsEqualTo },
            { Conditional_VariableExists, VariableExists },

            { Memory_StaticToVariable, StaticToVariable },
            { Memory_StaticToRegister, StaticToRegister },
            { Memory_RegisterToVariable, RegisterToVariable },
            { Memory_RegisterToRegister, RegisterToRegister },
            { Memory_VariableToRegister, VariableToRegister },
            { Memory_VariableToVariable, VariableToVariable },
            { Memory_CreateListVariable, CreateListVariable },
            { Memory_CreateTableVariable, CreateTableVariable },
            { Memory_DeleteMemory, MemoryDelete },
            { Memory_CollectGarbage, GarbageCollect },

            { Flow_ConditionalJump, JumpConditional },
            { Flow_Jump, Jump },
            { Flow_ConditionalJumpDynamic, JumpConditionalDynamic },
            { Flow_JumpDynamic, JumpDynamic },
            { Flow_CreateScope, CreateScope },
            { Flow_ExitScope, ExitScope },
        }, new() {
            { Type.Integer, (r,a) => r.Set(_stateMachine.Memory.GetInteger(a)) },
            { Type.Decimal, (r,a) => r.Set(_stateMachine.Memory.GetDecimal(a)) },
            { Type.Boolean, (r,a) => r.Set(_stateMachine.Memory.GetBoolean(a)) },
            { Type.Character, (r,a) => r.Set(_stateMachine.Memory.GetCharacter(a)) },
            { Type.String, (r,a) => r.Set(_stateMachine.Memory.GetString(a)) },
            { Type.Function, (r,a) => r.Set(_stateMachine.Memory.GetFunction(a)) },
        });
    }
}
