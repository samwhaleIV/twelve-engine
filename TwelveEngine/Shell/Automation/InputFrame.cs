using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Automation {
    internal struct InputFrame {
        internal InputFrame(SerialInputFrame frame) {
            var pressedKeys = frame.pressedKeys;
            var keysState = new Keys[pressedKeys.Length];
            for(var i = 0;i<keysState.Length;i++) {
                keysState[i] = (Keys)pressedKeys[i];
            }
            KeyboardState = new KeyboardState(keysState);
            FrameDelta = TimeSpan.FromTicks(frame.elapsedTime);

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

        internal TimeSpan FrameDelta;
        
        internal KeyboardState KeyboardState;
        internal MouseState MouseState;
    }
}
