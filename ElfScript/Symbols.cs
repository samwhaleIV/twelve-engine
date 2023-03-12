namespace ElfScript {
    internal static class Symbols {
        public const char OpenBlock = '{';
        public const char CloseBlock = '}';

        public const char OpenTuple = '(';
        public const char CloseTuple = ')';

        public const char OpenArray = '[';
        public const char CloseArray = ']';

        public const char StatementEnd = ';';
        public const char StringLimit = '"';
        public const char StringEscape = '\\';

        public const char FunctionOperator = '|';
        public const char AddOperator = '+';
        public const char SubtractOperator = '-';
        public const char DivideOperator = '/';
        public const char MultiplyOperator = '*';
        public const char DotOperator = '.';
        public const char DecimalSymbol = '.';
        public const char CommaOperator = ',';

        public const char EqualsOperator = '=';

        public const string LineComment = "//";

        public const char NewLineCharacter = 'n';

        public const bool DecimalSymbolMasksDotOperator = DecimalSymbol == DotOperator;
    }
}
