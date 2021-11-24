using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Input {
    public sealed class KeyboardHandler {

        public event Action<Keys> KeyDown;
        public event Action<Keys> KeyUp;

        private HashSet<Keys> pressedKeys = new HashSet<Keys>();
        private HashSet<Keys> keyBuffer = new HashSet<Keys>();
        private HashSet<Keys> swapAddress = null;

        public void Update(KeyboardState keyboardState) {
            var keys = keyboardState.GetPressedKeys();

            foreach(var key in keys) {
                if(pressedKeys.Contains(key)) {
                    keyBuffer.Add(key);
                    pressedKeys.Remove(key);
                } else {
                    keyBuffer.Add(key);
                    KeyDown?.Invoke(key);
                }
            }

            foreach(var key in pressedKeys) {
                KeyUp?.Invoke(key);
            }
            pressedKeys.Clear();

            swapAddress = pressedKeys;
            pressedKeys = keyBuffer;
            keyBuffer = swapAddress;
            swapAddress = null;
        }
    }
}
