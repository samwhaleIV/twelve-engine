namespace TwelveEngine.Input {
    public readonly struct ImpulseEvent {
        public readonly Impulse Impulse;

        public readonly bool Pressed;
        public readonly bool Released => !Pressed;

        private ImpulseEvent(Impulse impulse,bool pressed) {
            Impulse = impulse;
            Pressed = pressed;
        }

        public static ImpulseEvent CreatePressed(Impulse impulse) => new(impulse,true);
        public static ImpulseEvent CreateReleased(Impulse impulse) => new(impulse,false);
    }
}
