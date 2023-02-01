﻿using Microsoft.Xna.Framework.Input;
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

        public void Import(ImpulseEventHandler oldHandler) {
            impulseStates.Clear();
            foreach(var oldState in oldHandler.impulseStates) {
                impulseStates[oldState.Key] = oldState.Value;
            }
        }

        private KeyState GetImpulseState(Impulse impulse) {
            if(GetKeyboardBind(impulse).IsPressed(InputStateCache.Keyboard)) {
                return KeyState.Down;
            }
            if(InputStateCache.GamePad.IsButtonDown(GetGamePadBind(impulse))) {
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
            foreach(Impulse impulse in impulses) {
                if(!UpdateKeyState(impulse,out KeyState keyState) || !eventsAreAllowed) {
                    continue;
                }
                SendEvent(ImpulseEvent.Create(impulse,keyState == KeyState.Down));
            }
            if(InputStateCache.GamePad != InputStateCache.LastGamePad) {
                Method = InputMethod.GamePad;
            } else if(InputStateCache.Keyboard != InputStateCache.LastKeyboard) {
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
            Vector2 leftThumbStick = InputStateCache.GamePad.ThumbSticks.Left;
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
