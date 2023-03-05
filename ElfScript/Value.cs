namespace ElfScript.IR {
    internal readonly struct Value {
        public readonly Type Type { get; private init; }
        public readonly int ID { get; private init; }
        public Value(int ID,Type type) { this.ID = ID; Type = type; }
        public static readonly Value None = new Value(0,Type.Number);
    }
}
