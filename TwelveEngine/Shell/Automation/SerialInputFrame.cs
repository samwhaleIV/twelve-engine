﻿using Microsoft.Xna.Framework.Input;

namespace TwelveEngine.Shell.Automation {
    internal struct SerialInputFrame {

        internal long elapsedTime;

        internal int mouseX;
        internal int mouseY;

        internal int scrollX;
        internal int scrollY;

        internal byte xButton1;
        internal byte xButton2;

        internal byte leftButton;
        internal byte middleButton;
        internal byte rightButton;

        internal byte[] pressedKeys;

        internal SerialInputFrame(InputFrame frame) {
            Keys[] keysState = frame.KeyboardState.GetPressedKeys();
            pressedKeys = new byte[keysState.Length];
            for(var i = 0;i < pressedKeys.Length;i++) {
                pressedKeys[i] = (byte)keysState[i];
            }
            elapsedTime = frame.FrameDelta.Ticks;

            var mouse = frame.MouseState;

            mouseX = mouse.X;
            mouseY = mouse.Y;

            scrollX = mouse.HorizontalScrollWheelValue;
            scrollY = mouse.ScrollWheelValue;

            xButton1 = (byte)mouse.XButton1;
            xButton2 = (byte)mouse.XButton2;

            leftButton = (byte)mouse.LeftButton;
            middleButton = (byte)mouse.MiddleButton;
            rightButton = (byte)mouse.RightButton;
        }

        private static short IntToShort(int value) {
            return (short)Math.Max(short.MinValue,Math.Min(short.MaxValue,value));
        }
        private static int LongToInt(long value) {
            return (int)Math.Max(int.MinValue,Math.Min(int.MaxValue,value));
        }

        private bool MouseDataEqual(SerialInputFrame frame) {
            /* Sorted by frequency */
            return mouseX == frame.mouseX &&
                   mouseY == frame.mouseY &&
                   leftButton == frame.leftButton &&
                   scrollY == frame.scrollY &&
                   rightButton == frame.rightButton &&
                   middleButton == frame.middleButton &&
                   scrollX == frame.scrollX &&
                   xButton1 == frame.xButton1 &&
                   xButton2 == frame.xButton2;
        }

        internal void Export(BinaryWriter writer,SerialInputFrame lastFrame) {
            writer.Write(LongToInt(elapsedTime));

            if(MouseDataEqual(lastFrame)) {
                writer.Write((byte)1);
            } else {
                writer.Write((byte)0);
                writer.Write(IntToShort(mouseX));
                writer.Write(IntToShort(mouseY));

                writer.Write(IntToShort(scrollX));
                writer.Write(IntToShort(scrollY));

                writer.Write(xButton1);
                writer.Write(xButton2);

                writer.Write(leftButton);
                writer.Write(middleButton);
                writer.Write(rightButton);
            }

            var keyCount = pressedKeys.Length;
            writer.Write((byte)keyCount);
            for(var i = 0;i < keyCount;i++) {
                writer.Write(pressedKeys[i]);
            }
        }

        internal SerialInputFrame(SerialInputFrame lastFrame,BinaryReader reader) {
            elapsedTime = reader.ReadInt32();

            var mouseDataEqual = reader.ReadByte();
            if(mouseDataEqual == 1) {
                mouseX = lastFrame.mouseX;
                mouseY = lastFrame.mouseY;
                scrollX = lastFrame.scrollX;
                scrollY = lastFrame.scrollY;

                xButton1 = lastFrame.xButton1;
                xButton2 = lastFrame.xButton2;

                leftButton = lastFrame.leftButton;
                middleButton = lastFrame.middleButton;
                rightButton = lastFrame.rightButton;
            } else {
                mouseX = reader.ReadInt16();
                mouseY = reader.ReadInt16();
                scrollX = reader.ReadInt16();
                scrollY = reader.ReadInt16();

                xButton1 = reader.ReadByte();
                xButton2 = reader.ReadByte();

                leftButton = reader.ReadByte();
                middleButton = reader.ReadByte();
                rightButton = reader.ReadByte();
            }

            var keyCount = (int)reader.ReadByte();
            pressedKeys = new byte[keyCount];
            for(var i = 0;i < keyCount;i++) {
                pressedKeys[i] = reader.ReadByte();
            }
        }
    }
}
