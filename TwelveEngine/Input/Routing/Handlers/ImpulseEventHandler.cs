using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using TwelveEngine.Input.Binding;

namespace TwelveEngine.Input.Routing {

    public sealed class ImpulseEventHandler:InputEventHandler<ImpulseEvent,ImpulseEventRouter> {

        private static readonly Impulse[] impulses = (Impulse[])Enum.GetValues(typeof(Impulse));

        private readonly Dictionary<Impulse,KeyState> impulseStates = new();
        private static readonly Dictionary<Impulse,Buttons> gamePadBinds = KeyBinds.GetControllerBinds();

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

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        public KeyboardState LastKeyboardState { get; private set; } = new KeyboardState();
        public GamePadState LastGamePadState { get; private set; } = new GamePadState();

        private void ImportOldInputState(Dictionary<Impulse,KeyState> oldStates) {
            impulseStates.Clear();
            foreach(var oldState in oldStates) {
                impulseStates.Add(oldState.Key,oldState.Value);
            }
        }

        public void Import(ImpulseEventHandler oldHandler) {
            GamePadState = oldHandler.GamePadState;
            KeyboardState = oldHandler.KeyboardState;
            LastGamePadState = oldHandler.LastGamePadState;
            LastKeyboardState = oldHandler.LastKeyboardState;
            ImportOldInputState(oldHandler.impulseStates);
        }

        private KeyState GetImpulseState(Impulse impulse) {
            if(GetKeyboardBind(impulse).IsPressed(KeyboardState)) {
                return KeyState.Down;
            }
            if(GamePadState.IsButtonDown(GetGamePadBind(impulse))) {
                return KeyState.Down;
            }
            return KeyState.Up;
        }

        private bool TryFireEvent(Impulse impulse) {
            KeyState newState = GetImpulseState(impulse), oldState = impulseStates[impulse];
            if(newState == oldState) {
                return false;
            }
            impulseStates[impulse] = newState;

            if(newState == KeyState.Down) {
                SendEvent(ImpulseEvent.CreatePressed(impulse));
            } else {
                SendEvent(ImpulseEvent.CreateReleased(impulse));
            }
            return true;
        }

        public void Update(KeyboardState keyboardState,GamePadState gamePadState) {
            KeyboardState = keyboardState;
            GamePadState = gamePadState;

            foreach(Impulse impulse in impulses) {
                TryFireEvent(impulse);
            }

            if(gamePadState != LastGamePadState) {
                Method = InputMethod.GamePad;
            } else if(keyboardState != LastKeyboardState) {
                Method = InputMethod.Keyboard;
            }

            LastKeyboardState = keyboardState;
            LastGamePadState = gamePadState;
        }

        public bool IsImpulseDown(Impulse impulse) => impulseStates[impulse] == KeyState.Down;
        public bool IsImpulseUp(Impulse impulse) => impulseStates[impulse] == KeyState.Up;

        private bool TryGetThumbstick(out Vector2 value) {
            if(Method != InputMethod.GamePad) {
                value = Vector2.Zero;
                return false;
            }
            Vector2 leftThumbStick = LastGamePadState.ThumbSticks.Left;
            if(leftThumbStick == Vector2.Zero) {
                value = Vector2.Zero;
                return false;
            }
            leftThumbStick.Y = -leftThumbStick.Y;
            value = leftThumbStick;
            return true;
        }

        public Vector2 GetDelta2D() {
            if(TryGetThumbstick(out Vector2 value)) {
                return value;
            }
            int x = 0, y = 0;
            if(IsImpulseDown(Impulse.Up)) y--;
            if(IsImpulseDown(Impulse.Down)) y++;
            if(IsImpulseDown(Impulse.Left)) x--;
            if(IsImpulseDown(Impulse.Right)) x++;
            return new Vector2(x,y);
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
