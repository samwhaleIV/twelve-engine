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
        Integer,
        Decimal,
        Generic,
        /// <summary>
        /// Compiler internal token type.
        /// </summary>
        BlockReference,
        Operator,
        Keyword
    }
}
