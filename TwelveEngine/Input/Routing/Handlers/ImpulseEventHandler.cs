using Microsoft.Xna.Framework.Input;
using TwelveEngine.Input.Binding;
using TwelveEngine.Shell;

namespace TwelveEngine.Input.Routing {

    public sealed class ImpulseEventHandler:InputEventHandler<ImpulseEvent,ImpulseEventRouter> {

        private static readonly Impulse[] impulses = (Impulse[])Enum.GetValues(typeof(Impulse));
        private static readonly Dictionary<Impulse,Buttons> gamePadBinds = KeyBinds.GetControllerBinds();

        private readonly Dictionary<Impulse,KeyState> impulseStates = new();

        internal ImpulseEventHandler() {
            foreach(var impulse in impulses) {
                impulseStates[impulse] = KeyState.Up;
            }
        }

        public static Buttons GetGamePadBind(Impulse impulse) {
            if(!gamePadBinds.TryGetValue(impulse,out Buttons button)) {
                return Buttons.None;
            }
            return button;
        }

        public static MultiBindKey GetKeyboardBind(Impulse impulse) {
            if(!KeyBinds.TryGet(impulse,out MultiBindKey key)) {
                return MultiBindKey.None;
            }
            return key;
        }

        public static InputMethod Method { get; private set; } = InputMethod.Unknown;
        public static GamePadType GamePadType { get; private set; } = GamePadType.Default;


        public KeyboardState Keyboard { get; private set; }
        public GamePadState GamePad { get; private set; }

        public KeyboardState OldKeyboard { get; private set; }
        public GamePadState OldGamePad { get; private set; }

        public void Import(ImpulseEventHandler oldHandler) {
            Keyboard = oldHandler.Keyboard;
            OldKeyboard = oldHandler.OldKeyboard;

            GamePad = oldHandler.GamePad;
            OldGamePad = oldHandler.GamePad;

            foreach(var oldState in oldHandler.impulseStates) {
                impulseStates[oldState.Key] = oldState.Value;
            }
        }

        private KeyState GetImpulseState(Impulse impulse) {
            if(GetKeyboardBind(impulse).IsPressed(Keyboard)) {
                return KeyState.Down;
            }
            if(GamePad.IsButtonDown(GetGamePadBind(impulse))) {
                return KeyState.Down;
            }
            return KeyState.Up;
        }

        private bool UpdateKeyState(Impulse impulse,out KeyState keyState) {
            KeyState newState = GetImpulseState(impulse), oldState = impulseStates[impulse];
            keyState = newState;
            if(newState == oldState) {
                return false;
            }
            impulseStates[impulse] = newState;
            return true;
        }

        public void Update(bool eventsAreAllowed) {
            OldKeyboard = Keyboard;
            Keyboard = InputStateCache.Keyboard;

            OldGamePad = GamePad;
            GamePad = InputStateCache.GamePad;

            foreach(Impulse impulse in impulses) {
                if(!UpdateKeyState(impulse,out KeyState keyState) || !eventsAreAllowed) {
                    continue;
                }
                SendEvent(ImpulseEvent.Create(impulse,keyState == KeyState.Down));
            }

            if(GamePad != OldGamePad) {
                Method = InputMethod.GamePad;
            } else if(Keyboard != OldKeyboard) {
                Method = InputMethod.Keyboard;
            }
        }

        public bool IsImpulseDown(Impulse impulse) => impulseStates[impulse] == KeyState.Down;
        public bool IsImpulseUp(Impulse impulse) => impulseStates[impulse] == KeyState.Up;

        private bool TryGetThumbstick(out Vector2 value) {
            if(Method != InputMethod.GamePad) {
                value = Vector2.Zero;
                return false;
            }
            Vector2 leftThumbStick = GamePad.ThumbSticks.Left;
            if(leftThumbStick == Vector2.Zero) {
                value = Vector2.Zero;
                return false;
            }
            leftThumbStick.Y = -leftThumbStick.Y;
            value = leftThumbStick;
            return true;
        }

        private float GetSpeedModifier(bool turbo,bool snail) {
            float speedModifier = 1;
            if(turbo) {
                speedModifier += 1;
            }
            if(snail) {
                speedModifier /= 2;
            }
            return speedModifier;
        }

        private float GetGamePadSpeedModifier() => GetSpeedModifier(
            GamePad.Buttons.RightShoulder == ButtonState.Pressed,GamePad.Buttons.LeftShoulder == ButtonState.Pressed
        );

        private float GetKeyboardSpeedModifier() => GetSpeedModifier(
            Keyboard.IsKeyDown(Keys.LeftShift),Keyboard.IsKeyDown(Keys.LeftAlt)
        );

        private Vector2 GetImpulseDelta2D() {
            int x = 0, y = 0;
            if(IsImpulseDown(Impulse.Up)) y--;
            if(IsImpulseDown(Impulse.Down)) y++;
            if(IsImpulseDown(Impulse.Left)) x--;
            if(IsImpulseDown(Impulse.Right)) x++;

            var delta = new Vector2(x,y);
            if(MathF.Abs(x) == MathF.Abs(y) && MathF.Abs(x) > 0) {
                delta *= 0.75f;
            }
            return delta;
        }

        public Vector2 GetDelta2D() {
            if(!TryGetThumbstick(out Vector2 delta)) {
                delta = GetImpulseDelta2D();
            }
            if(Method == InputMethod.GamePad) {
                delta *= GetGamePadSpeedModifier();
            } else {
                delta *= GetKeyboardSpeedModifier();
            }
            return delta;
        }

        public Vector3 GetDelta3D() {
            int x = 0, y = 0, z = 0;
            if(IsImpulseDown(Impulse.Left)) x--; //Strafe Left
            if(IsImpulseDown(Impulse.Right)) x++; //Strafe Right
            if(IsImpulseDown(Impulse.Ascend)) y--; //Straight up
            if(IsImpulseDown(Impulse.Descend)) y++; //Straight down
            if(IsImpulseDown(Impulse.Up)) z--; //Forward
            if(IsImpulseDown(Impulse.Down)) z++; //Backwards
            return new Vector3(x,y,z);
        }
    }
}
