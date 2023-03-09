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
        Memory_StaticToMemory,

        Memory_RegisterToMemory,
        Memory_RegisterToRegister,
        Memory_MemoryToRegister,
        Memory_MemoryToMemory,
        Memory_DeleteMemory,
        Memory_CollectGarbage,

        Memory_CreateList,
        Memory_CreateTable,

        Control_EnterBlock,
        Control_ExitBlock,

        Control_Jump,
        Control_ConditionalJump,
        Control_JumpDynamic,
        Control_ConditionalJumpDynamic,
        Control_Interrupt,

        Conditional_TypeEqual,
        Conditional_TypeNotEqual,
        Conditional_IsGreaterThan,
        Conditional_IsLessThan,
        Conditional_IsGreaterThanOrEqual,
        Conditional_IsLessThanOrEqual,
        Conditional_IsNotEqualTo,
        Conditional_IsEqualTo,

        Conditional_MemoryExists
    }
}
