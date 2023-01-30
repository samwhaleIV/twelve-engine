using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {

    public sealed class InputHandler {

        private readonly Dictionary<Impulse,(Action Down, Action Up)> endpoints;

        private static readonly Impulse[] impulses = (Impulse[])Enum.GetValues(typeof(Impulse));

        private readonly Dictionary<Impulse,KeyState> impulseStates = new();
        private static readonly Dictionary<Impulse,Buttons> gamePadBinds = KeyBinds.GetControllerBinds();

        internal InputHandler() {
            foreach(var impulse in impulses) {
                impulseStates[impulse] = KeyState.Up;
            }
            endpoints = GetEndpoints();
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

        public void Import(InputHandler oldHandler) {
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

        private void UpdateImpulse(Impulse impulse) {
            KeyState newState = GetImpulseState(impulse), oldState = impulseStates[impulse];

            if(newState == oldState) {
                return;
            }

            impulseStates[impulse] = newState;

            if(!endpoints.TryGetValue(impulse,out (Action Down, Action Up) endpoint)) {
                return;
            }
            if(newState == KeyState.Down) {
                endpoint.Down.Invoke();
            } else {
                endpoint.Up.Invoke();
            }
        }

        public void Update(KeyboardState keyboardState,GamePadState gamePadState) {
            KeyboardState = keyboardState;
            GamePadState = gamePadState;

            foreach(Impulse impulse in impulses) {
                UpdateImpulse(impulse);
            }

            if(gamePadState != LastGamePadState) {
                Method = InputMethod.GamePad;
            } else if(keyboardState != LastKeyboardState) {
                Method = InputMethod.Keyboard;
            }

            LastKeyboardState = keyboardState;
            LastGamePadState = gamePadState;
        }

        public bool IsKeyDown(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Down;
        }
        public bool IsKeyUp(Impulse impulse) {
            return impulseStates[impulse] == KeyState.Up;
        }

        public Vector2 GetDelta2D() {
            if(Method == InputMethod.GamePad) {
                Vector2 leftThumbStick = LastGamePadState.ThumbSticks.Left;
                if(leftThumbStick != Vector2.Zero) {
                    leftThumbStick.Y = -leftThumbStick.Y;
                    return leftThumbStick;
                }
            }
            int x = 0, y = 0;
            if(IsKeyDown(Impulse.Up)) {
                y--;
            }
            if(IsKeyDown(Impulse.Down)) {
                y++;
            }
            if(IsKeyDown(Impulse.Left)) {
                x--;
            }
            if(IsKeyDown(Impulse.Right)) {
                x++;
            }
            return new Vector2(x,y);
        }

        public Vector3 GetDelta3D() {
            int x = 0, y = 0, z = 0;
            if(IsKeyDown(Impulse.Left)) {
                x--; //Strafe Left
            }
            if(IsKeyDown(Impulse.Right)) {
                x++; //Strafe Right
            }
            if(IsKeyDown(Impulse.Ascend)) {
                y--; //Straight up
            }
            if(IsKeyDown(Impulse.Descend)) {
                y++; //Straight down
            }
            if(IsKeyDown(Impulse.Up)) {
                z--; //Forward
            }
            if(IsKeyDown(Impulse.Down)) {
                z++; //Backwards
            }
            return new Vector3(x,y,z);
        }

        private Dictionary<Impulse,(Action, Action)> GetEndpoints() => new() {

            {Impulse.Accept,(
                () => OnAcceptDown?.Invoke(),
                () => OnAcceptUp?.Invoke()
            )},

            {Impulse.Cancel,(
                () => OnCancelDown?.Invoke(),
                () => OnCancelUp?.Invoke()
            )},

            {Impulse.Up,(
                () => OnDirectionDown?.Invoke(Direction.Up),
                () => OnDirectionUp?.Invoke(Direction.Up)
            )},

            {Impulse.Down,(
                () => OnDirectionDown?.Invoke(Direction.Down),
                () => OnDirectionUp?.Invoke(Direction.Down)
            )},

            {Impulse.Left,(
                () => OnDirectionDown?.Invoke(Direction.Left),
                () => OnDirectionUp?.Invoke(Direction.Left)
            )},

            {Impulse.Right,(
                () => OnDirectionDown?.Invoke(Direction.Right),
                () => OnDirectionUp?.Invoke(Direction.Right)
            )},

            {Impulse.Focus,(
                () => OnFocusDown?.Invoke(),
                () => OnFocusUp?.Invoke()
            )}
        };

        public event Action OnAcceptDown;
        public event Action OnAcceptUp;

        public event Action OnCancelDown;
        public event Action OnCancelUp;

        public event Action OnFocusDown;
        public event Action OnFocusUp;

        public event Action<Direction> OnDirectionUp;
        public event Action<Direction> OnDirectionDown;
    }
}
