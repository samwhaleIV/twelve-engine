using System;
using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Automation {
    internal struct InputFrame {
        internal InputFrame(SerialInputFrame frame) {
            var pressedKeys = frame.pressedKeys;
            var keysState = new Keys[pressedKeys.Length];
            for(var i = 0;i<keysState.Length;i++) {
                keysState[i] = (Keys)pressedKeys[i];
            }
            KeyboardState = new KeyboardState(keysState);
            ElapsedTime = TimeSpan.FromTicks(frame.elapsedTime);

            MouseState = new MouseState(
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

        internal TimeSpan ElapsedTime;
        
        internal KeyboardState KeyboardState;
        internal MouseState MouseState;
    }
}
