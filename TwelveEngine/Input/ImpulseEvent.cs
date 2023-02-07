namespace TwelveEngine.Input {
    public readonly struct ImpulseEvent {
        public readonly Impulse Impulse;

        public readonly bool Pressed;
        public readonly bool Released => !Pressed;

        private ImpulseEvent(Impulse impulse,bool pressed) {
            Impulse = impulse;
            Pressed = pressed;
        }

        public static ImpulseEvent Create(Impulse impulse,bool isPressed) => new(impulse,isPressed);
    }
}
