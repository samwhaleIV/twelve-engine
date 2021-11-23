using System;
using System.IO;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine {
    internal struct InputFrame {
        internal InputFrame(SerialInputFrame frame) {
            var pressedKeys = frame.pressedKeys;
            var keysState = new Keys[pressedKeys.Length];
            for(var i = 0;i<keysState.Length;i++) {
                keysState[i] = (Keys)pressedKeys[i];
            }
            keyboardState = new KeyboardState(keysState);
            totalTime = TimeSpan.FromTicks(frame.totalTime);
            elapsedTime = TimeSpan.FromTicks(frame.elapsedTime);

            mouseState = new MouseState(
                frame.mouseX,
                frame.mouseY,
                frame.scrollY,
                (ButtonState)frame.leftButton,
                (ButtonState)frame.middleButton,
                (ButtonState)frame.rightButton,
                (ButtonState)frame.xButton1,
                (ButtonState)frame.xButton2,
                frame.scrollX
            );
        }

        internal TimeSpan totalTime;
        internal TimeSpan elapsedTime;
        
        internal KeyboardState keyboardState;
        internal MouseState mouseState;
    }
}
