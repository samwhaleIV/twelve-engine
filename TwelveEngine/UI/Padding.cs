using System;

namespace TwelveEngine.UI {
    public struct Padding:IEquatable<Padding> {

        public Padding(int amount) {
            Top = amount;
            Left = amount;
            Right = amount;
            Bottom = amount;
        }

        public int Top;
        public int Left;
        public int Right;
        public int Bottom;

        public bool IsBalanced {
            get => Top == Left && Top == Right && Top == Bottom;
        }

        internal int Value {
            /* Top is arbitrary. Value intended to be used after an IsBalanced check */
            get => Top;
        }

        public static bool operator == (Padding a,Padding b) {
            return a.Top == b.Top && a.Left == b.Left && a.Right == b.Right && a.Bottom == b.Bottom;
        }

        public static bool operator != (Padding a,Padding b) {
            return a.Top != b.Top || a.Left != b.Left || a.Right != b.Right || a.Bottom != b.Bottom;
        }

        public override bool Equals(object obj) {
            return obj is Padding padding && Equals(padding);
        }

        public bool Equals(Padding other) {
            return Value == other.Value;
        }

        public override int GetHashCode() {
            return HashCode.Combine(Top,Left,Right,Bottom);
        }
    }
}
