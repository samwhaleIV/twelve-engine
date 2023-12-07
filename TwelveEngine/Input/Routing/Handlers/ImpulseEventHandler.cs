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

        public void Release() {
            foreach(var impulse in impulses) {
                if(!IsImpulseDown(impulse)) {
                    continue;
                }
                impulseStates[impulse] = KeyState.Up;
                SendEvent(ImpulseEvent.Create(impulse,false));
            }
        }

        public void Update(bool eventsAreAllowed) {
            OldKeyboard = Keyboard;
            OldGamePad = GamePad;

            if(!eventsAreAllowed) {
                Keyboard = new KeyboardState();
                GamePad = GamePadState.Default;
                return;
            }
  
            Keyboard = InputStateCache.Keyboard;
            GamePad = InputStateCache.GamePad;

            foreach(Impulse impulse in impulses) {
                if(!UpdateKeyState(impulse,out KeyState keyState)) {
                    continue;
                }
                SendEvent(ImpulseEvent.Create(impulse,keyState == KeyState.Down));
            }
        }

        public bool IsImpulseDown(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Down;
        }

        public bool IsImpulseUp(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Up;
        }

        private float GetGenericVelocityModifier() {
            bool turbo = Keyboard.IsKeyDown(Keys.LeftShift) || GamePad.Buttons.RightShoulder == ButtonState.Pressed;
            bool snail = Keyboard.IsKeyDown(Keys.LeftAlt) || GamePad.Buttons.LeftShoulder == ButtonState.Pressed;
            float speedModifier = 1;
            if(turbo) {
                speedModifier += 1;
            }
            if(snail) {
                speedModifier *= 0.5f;
            }
            return speedModifier;
        }

        private Vector2 GetDigitalDelta2D(bool useVelocityModifier) {
            int x = 0, y = 0;

            if(IsImpulseDown(Impulse.Up)) y--;
            if(IsImpulseDown(Impulse.Down)) y++;
            if(IsImpulseDown(Impulse.Left)) x--;
            if(IsImpulseDown(Impulse.Right)) x++;

            var delta = new Vector2(x,y);
            if(x != 0 && y != 0) {
                delta *= 0.75f;
            }

            if(useVelocityModifier) {
                delta *= GetGenericVelocityModifier();
            }

            return delta;
        }

        public Vector2 GetDigitalDelta2D() {
            return GetDigitalDelta2D(useVelocityModifier: false);
        }

        public Vector2 GetDigitalDelta2DWithModifier() {
            return GetDigitalDelta2D(useVelocityModifier: true);
        }

        public Vector2 GetDelta2DAnalog() {
            throw new NotImplementedException();
        }

        public Vector3 GetDelta3DDigital() {
            int x = 0, y = 0, z = 0;
            if(IsImpulseDown(Impulse.Left)) x--; //Strafe Left
            if(IsImpulseDown(Impulse.Right)) x++; //Strafe Right
            if(IsImpulseDown(Impulse.Ascend)) y--; //Straight up
            if(IsImpulseDown(Impulse.Descend)) y++; //Straight down
            if(IsImpulseDown(Impulse.Up)) z--; //Forward
            if(IsImpulseDown(Impulse.Down)) z++; //Backwards
            return new Vector3(x,y,z);
        }

        public Vector3 GetDelta3DAnalog() {
            throw new NotImplementedException();
        }
    }
}
