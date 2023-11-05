namespace ElfScript.Compiler.ILGen {
    internal readonly struct ExpressionPattern {
        public readonly ExpressionPatternItem[] Items { get; init;  }
        public ExpressionPattern(params ExpressionPatternItem[] items) {
            Items = items;
        }
    }
}
