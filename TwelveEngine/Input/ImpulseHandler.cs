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
        public InputMethod InputMethod => inputMethod;

        private KeyState getState(Impulse impulse,KeyboardState keyboardState,GamePadState gamePadState) {
            bool pressed = keyboardState.IsKeyDown(keyBinds[impulse]) || gamePadState.IsButtonDown(controllerBinds[impulse]);
            return pressed ? KeyState.Down : KeyState.Up;
        }

        public void Update(KeyboardState keyboardState,GamePadState gamePadState) {
            foreach(Impulse impulse in impulses) {

                KeyState newState = getState(impulse,keyboardState,gamePadState);
                KeyState oldState = impulseStates[impulse];

                if(newState == oldState) {
                    continue;
                }

                impulseStates[impulse] = newState;
                if(newState == KeyState.Down) {
                    endpoints[impulse].Down();
                } else {
                    endpoints[impulse].Up();
                }
            }
        }

        public bool IsKeyDown(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Down;
        }
        public bool IsKeyUp(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Up;
        }
    }
}
