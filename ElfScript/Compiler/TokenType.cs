namespace ElfScript.Compiler {
    internal enum TokenType {
        OpenBlock,
        CloseBlock,
        OpenTuple,
        CloseTuple,
        OpenArray,
        CloseArray,
        StatementEnd,
        String,
        Operator,
        Number,
        Generic,
        /// <summary>
        /// Compiler internal token type.
        /// </summary>
        BlockReference
    }
}
