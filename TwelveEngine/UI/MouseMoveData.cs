namespace TwelveEngine.UI {
    public readonly struct MouseMoveData {

        public MouseMoveData(int x,int y,bool elementFocused) {
            X = x;
            Y = y;
            ElementFocused = elementFocused;
        }

        public readonly int X;
        public readonly int Y;
        public readonly bool ElementFocused;
    }
}
