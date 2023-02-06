using Elves.Battle;
using Elves.ElfData;

namespace Elves.Scenes {
    /// <summary>
    /// This struct is an over-engineering of an integer. It's for context awareness and more meaingful metadata when routing events with <c>Action</c>.
    /// </summary>
    public readonly struct ExitValue {

        private const int DEFAULT_VALUE = 0, FLAGGED_VALUE = 1;

        private ExitValue(int integer) => _value = integer;

        public static readonly ExitValue None = new(integer: DEFAULT_VALUE);
        public static readonly ExitValue Flagged = new(integer: FLAGGED_VALUE);
        public static readonly ExitValue Back = new(integer: -1);

        public static ExitValue Get(int integer) => new(integer);
        public static ExitValue Get(bool flag) => new(flag ? FLAGGED_VALUE : DEFAULT_VALUE);

        public static ExitValue Get(BattleResult battleResult) => new((int)battleResult);
        public static ExitValue Get(ElfID elfID) => new((int)elfID);

        /// <summary>
        /// An untyped, generic integer. Useful for lots of exit parameters! Indices, success codes, a quantity of nuclear bombs, etc.
        /// </summary>
        private readonly int _value;

        public readonly int Index => _value;
        public readonly int Count => _value;
        public readonly int Value => _value;
        public readonly int SaveID => _value;

        public readonly bool IsFlagged => _value != 0;
        public readonly bool QuickExit => IsFlagged;

        public readonly BattleResult BattleResult => (BattleResult)_value;
        public readonly ElfID BattleID => (ElfID)_value;

        public override bool Equals(object obj) => obj is ExitValue b && _value == b._value;

        public bool Equals(ExitValue b) => _value == b._value;

        public override int GetHashCode() => _value;

        public static bool operator ==(ExitValue a,ExitValue b) => a._value == b._value;
        public static bool operator !=(ExitValue a,ExitValue b) => a._value != b._value;
    }
}
