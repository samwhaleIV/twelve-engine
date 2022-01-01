using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {

    public enum InputMethod {
        Unknown, Keyboard, Gamepad
    }

    public sealed partial class ImpulseHandler {

        public ImpulseHandler(KeyBindSet keyBinds) {
            this.keyBinds = keyBinds;

            impulses = (Impulse[])Enum.GetValues(typeof(Impulse));

            impulseStates = new Dictionary<Impulse,KeyState>();
            foreach(var impulse in impulses) {
                impulseStates[impulse] = KeyState.Up;
            }

            endpoints = GetEndpoints();
        }

        private readonly Impulse[] impulses;

        private readonly Dictionary<Impulse,KeyState> impulseStates;
        private Dictionary<Impulse,(Action Down, Action Up)> endpoints;

        private readonly KeyBindSet keyBinds;
        private Dictionary<Impulse,Buttons> controllerBinds = GetControllerBinds();

        public event Action<InputMethod> OnInputMethodChanged; //todo, implement this so we can one day use icon glyphs!

        private InputMethod inputMethod = InputMethod.Unknown;
        public InputMethod InputMethod {
            get => inputMethod;
            private set {
                if(inputMethod == value) {
                    return;
                }
                inputMethod = value;
                OnInputMethodChanged.Invoke(inputMethod);
            }
        }

        private void updateInputMethod(bool fromKeyboard) {
            InputMethod = fromKeyboard ? InputMethod.Keyboard : InputMethod.Gamepad;
        }

        private (KeyState Value,bool FromKeyboard) getImpulseState(
            Impulse impulse,KeyboardState keyboardState,GamePadState gamePadState
        ) {
            bool keyboard = keyboardState.IsKeyDown(keyBinds[impulse]);
            if(keyboard) {
                return (keyboard ? KeyState.Down : KeyState.Up, true);
            }
            bool gamePad = gamePadState.IsButtonDown(controllerBinds[impulse]);
            return (gamePad ? KeyState.Down : KeyState.Up, false);
        }

        public void Update(KeyboardState keyboardState,GamePadState gamePadState) {
            bool fromKeyboard = true;

            foreach(Impulse impulse in impulses) {

                var impulseState = getImpulseState(impulse,keyboardState,gamePadState);

                KeyState newState = impulseState.Value, oldState = impulseStates[impulse];

                if(newState == oldState) {
                    continue;
                }

                fromKeyboard = impulseState.FromKeyboard;

                impulseStates[impulse] = newState;
                if(newState == KeyState.Down) {
                    endpoints[impulse].Down();
                } else {
                    endpoints[impulse].Up();
                }
            }

            updateInputMethod(fromKeyboard);
        }

        public bool IsKeyDown(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Down;
        }
        public bool IsKeyUp(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Up;
        }
    }
}
