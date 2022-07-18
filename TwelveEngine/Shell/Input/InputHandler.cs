using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Input {

    public sealed partial class InputHandler {

        internal InputHandler() {
            impulses = (Impulse[])Enum.GetValues(typeof(Impulse));

            impulseStates = new Dictionary<Impulse,KeyState>();
            foreach(var impulse in impulses) {
                impulseStates[impulse] = KeyState.Up;
            }

            endpoints = GetEndpoints();
        }

        private readonly Impulse[] impulses;

        private readonly Dictionary<Impulse,KeyState> impulseStates;
        private readonly Dictionary<Impulse,(Action Down, Action Up)> endpoints;

        private KeyBinds keyBinds = null;
        internal void Load(KeyBinds keyBinds) => this.keyBinds = keyBinds;

        private readonly Dictionary<Impulse,Buttons> gamePadBinds = GetControllerBinds();

        public Buttons GetGamePadBind(Impulse impulse) {
            return gamePadBinds[impulse];
        }

        public Keys GetKeyboardBind(Impulse impulse) {
            return keyBinds[impulse];
        }

        private static InputMethod _inputMethod = InputMethod.Unknown;

        public InputMethod Method {
            get => _inputMethod;
            private set {
                _inputMethod = value;
            }
        }

        public GamePadType GamePadType { get; set; } = GamePadType.Default;

        private KeyState getImpulseState(
            Impulse impulse,KeyboardState keyboardState,GamePadState gamePadState
        ) {
            bool keyboard = keyboardState.IsKeyDown(keyBinds[impulse]);
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

                if(!endpoints.TryGetValue(impulse,out var endpoint)) {
                    continue;
                }

                if(newState == KeyState.Down) {
                    endpoints[impulse].Down();
                } else {
                    endpoints[impulse].Up();
                }
            }

            if(didUpdate) {
                 if(gamePadState != lastGamePadState) {
                    Method = InputMethod.GamePad;
                } else if(keyboardState != lastKeyboardState) {
                    Method = InputMethod.Keyboard;
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

        public Vector2 GetDelta2D() {

            if(Method == InputMethod.GamePad) {
                var leftThumbStick = lastGamePadState.ThumbSticks.Left;
                if(leftThumbStick != Vector2.Zero) {
                    leftThumbStick.Y = -leftThumbStick.Y;
                    return leftThumbStick;
                }
            }

            var delta = Vector2.Zero;

            if(IsKeyDown(Impulse.Up)) delta.Y--;
            if(IsKeyDown(Impulse.Down)) delta.Y++;

            if(IsKeyDown(Impulse.Left)) delta.X--;
            if(IsKeyDown(Impulse.Right)) delta.X++;

            return delta;
        }

        public Vector3 GetDelta3D() {
            int x = 0, y = 0, z = 0;

            if(IsKeyDown(Impulse.Left)) x--; //Strafe Left
            if(IsKeyDown(Impulse.Right)) x++; //Strafe Right

            if(IsKeyDown(Impulse.Ascend)) y--; //Straight up
            if(IsKeyDown(Impulse.Descend)) y++; //Straight down

            if(IsKeyDown(Impulse.Up)) z--; //Forward 
            if(IsKeyDown(Impulse.Down)) z++; //Backwards

            return new Vector3(x,y,z);
        }
    }
}
