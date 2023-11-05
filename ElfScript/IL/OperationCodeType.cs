namespace ElfScript.IL {
    internal enum OperationCodeType:byte {
        Operation_NoOperation,
        Operation_Add,
        Operation_Subtract,
        Operation_Multiply,
        Operation_Divide,
        Operation_Modulus,
        Operation_Min,
        Operation_Max,

        Memory_StaticToRegister,
        Memory_StaticToVariable,

        Memory_RegisterToVariable,
        Memory_RegisterToRegister,
        Memory_VariableToRegister,
        Memory_VariableToVariable,
        Memory_DeleteMemory,
        Memory_CollectGarbage,

        Memory_RegisterToCallBuffer,
        Memory_VariableToCallBuffer,
        Memory_StaticToCallBuffer,
        Memory_CallBufferToVariable,
        Memory_CallBufferToRegister,

        Memory_CreateListVariable,
        Memory_CreateTableVariable,

        Flow_CreateScope,
        Flow_ExitScope,

        Flow_Jump,
        Flow_ConditionalJump,
        Flow_JumpDynamic,
        Flow_ConditionalJumpDynamic,
        Flow_Interrupt,

        Conditional_TypeEqual,
        Conditional_TypeNotEqual,
        Conditional_IsGreaterThan,
        Conditional_IsLessThan,
        Conditional_IsGreaterThanOrEqual,
        Conditional_IsLessThanOrEqual,
        Conditional_IsNotEqualTo,
        Conditional_IsEqualTo,
        Conditional_VariableExists
    }
}
