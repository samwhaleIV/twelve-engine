using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    internal sealed class KeyboardHandler {

        public Action<Keys> KeyDown { get; set; }
        public Action<Keys> KeyUp { get; set; }

        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private HashSet<Keys> keyBuffer = new HashSet<Keys>();
        private HashSet<Keys> swapAddress = null;

        private void keyDown(Keys key) {
            KeyDown?.Invoke(key);
        }
        private void keyUp(Keys key) {
            KeyUp?.Invoke(key);
        }

        public void Update() {
            var keyboardState = Keyboard.GetState();
            var keys = keyboardState.GetPressedKeys();

            foreach(var key in keys) {
                if(pressedKeys.Contains(key)) {
                    keyBuffer.Add(key);
                    pressedKeys.Remove(key);
                } else {
                    keyBuffer.Add(key);
                    keyDown(key);
                }
            }

            foreach(var key in pressedKeys) {
                keyUp(key);
            }
            pressedKeys.Clear();

            swapAddress = pressedKeys;
            pressedKeys = keyBuffer;
            keyBuffer = swapAddress;
            swapAddress = null;
        }
    }
}
