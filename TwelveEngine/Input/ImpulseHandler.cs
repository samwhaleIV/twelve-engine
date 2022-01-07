using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {

    public sealed partial class ImpulseHandler {

        public ImpulseHandler(KeyBindSet keyBindSet) {
            this.keyBindSet = keyBindSet;

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

        private readonly KeyBindSet keyBindSet;
        private Dictionary<Impulse,Buttons> gamePadBinds = GetControllerBinds();

        public Buttons GetGamePadBind(Impulse impulse) {
            return gamePadBinds[impulse];
        }

        public Keys GetKeyboardBind(Impulse impulse) {
            return keyBindSet[impulse];
        }

        public KeyBindSet KeyBindSet => keyBindSet;

        public InputMethod InputMethod { get; private set; } = InputMethod.Unknown;
        public GamePadType GamePadType { get; set; } = GamePadType.Default;

        private KeyState getImpulseState(
            Impulse impulse,KeyboardState keyboardState,GamePadState gamePadState
        ) {
            bool keyboard = keyboardState.IsKeyDown(keyBindSet[impulse]);
            if(keyboard) {
                return KeyState.Down;
            }
            bool gamePad = gamePadState.IsButtonDown(gamePadBinds[impulse]);
            if(gamePad) {
                return KeyState.Down;
            } else {
                return KeyState.Up;
            }
        }

        private KeyboardState lastKeyboardState;
        private GamePadState lastGamePadState;

        private bool didUpdate = false;

        public void Update(KeyboardState keyboardState,GamePadState gamePadState) {
            foreach(Impulse impulse in impulses) {

                var newState = getImpulseState(impulse,keyboardState,gamePadState);
                var oldState = impulseStates[impulse];

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

            if(didUpdate) {
                if(keyboardState != lastKeyboardState) {
                    InputMethod = InputMethod.Keyboard;
                } else if(gamePadState != lastGamePadState) {
                    InputMethod = InputMethod.GamePad;
                }
            }

            didUpdate = true;

            lastKeyboardState = keyboardState;
            lastGamePadState = gamePadState;
        }

        public bool IsKeyDown(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Down;
        }
        public bool IsKeyUp(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Up;
        }

        public Point GetDirectionDelta() {
            var delta = Point.Zero;

            if(IsKeyDown(Impulse.Up)) {
                delta.Y--;
            }
            if(IsKeyDown(Impulse.Down)) {
                delta.Y++;
            }
            if(IsKeyDown(Impulse.Left)) {
                delta.X--;
            }
            if(IsKeyDown(Impulse.Right)) {
                delta.X++;
            }

            return delta;
        }
    }
}
