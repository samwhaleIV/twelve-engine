namespace TwelveEngine.UI {
    internal struct Padding {
        public Padding(int amount) {
            Left = amount;
            Right = amount;
            Top = amount;
            Bottom = amount;
        }
        public int Left;
        public int Right;
        public int Top;
        public int Bottom;
    }
}
