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
            /* Auto-Generated Code: Microsoft, buddy. What the fuck, man? */
            int hashCode = -1217393117;
            hashCode = hashCode * -1521134295 + Top.GetHashCode();
            hashCode = hashCode * -1521134295 + Left.GetHashCode();
            hashCode = hashCode * -1521134295 + Right.GetHashCode();
            hashCode = hashCode * -1521134295 + Bottom.GetHashCode();
            return hashCode;
        }
    }
}
