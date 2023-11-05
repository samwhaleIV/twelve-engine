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
        public const char DecimalSymbol = '.';
        public const char NewLineCharacter = 'n';
        public const string LineComment = "//";

        public static class Operators {
            public const string Assignment = "=";
            public const string AddAssignment = "+=";
            public const string SubtractAssignment = "-=";
            public const string Parameter = "|";
            public const string Add = "+";
            public const string Subtract = "-";
            public const string Divide = "/";
            public const string Multiply = "*";
            public const string DivideAssignment = "/";
            public const string MultiplyAssignment = "*";
            public const string Comma = ",";
            public const string Not = "!";
            public const string GreaterThan = ">";
            public const string LessThan = "<";
            public const string GreaterThanOrEqual = ">=";
            public const string LessThanOrEqual = "<=";
            public const string And = "&&";
            public const string Or = "||";
            public const string Modulus = "%";
            public const string ModulusAssignment = "%=";
        }

        public static class Keywords {
            public const string Delete = "delete";
            public const string Exec = "exec";
            public const string If = "if";
        }
    }
}
