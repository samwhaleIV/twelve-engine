using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace TwelveEngine {
    internal struct SerialInputFrame {

        /* Todo write ints and long to byte array instead of string array */

        private const char SEPERATOR = ',';
        internal int[] pressedKeys;

        internal long totalTime;
        internal long elapsedTime;

        internal int mouseX;
        internal int mouseY;

        internal int scrollX;
        internal int scrollY;

        internal int xButton1;
        internal int xButton2;

        internal int leftButton;
        internal int rightButton;
        internal int middleButton;

        internal SerialInputFrame(InputFrame frame) {
            Keys[] keysState = frame.keyboardState.GetPressedKeys();
            pressedKeys = new int[keysState.Length];
            for(var i = 0;i < pressedKeys.Length;i++) {
                pressedKeys[i] = (int)keysState[i];
            }
            totalTime = frame.totalTime.Ticks;
            elapsedTime = frame.elapsedTime.Ticks;

            var mouse = frame.mouseState;

            mouseX = mouse.X;
            mouseY = mouse.Y;

            scrollY = mouse.ScrollWheelValue;
            scrollX = mouse.HorizontalScrollWheelValue;

            leftButton = (int)mouse.LeftButton;
            rightButton = (int)mouse.RightButton;

            middleButton = (int)mouse.MiddleButton;

            xButton1 = (int)mouse.XButton1;
            xButton2 = (int)mouse.XButton2;
        }

        private static void addInt(StringBuilder builder,int value) {
            builder.Append(value);
            builder.Append(SEPERATOR);
        }
        private static void addLong(StringBuilder builder,long value) {
            builder.Append(value);
            builder.Append(SEPERATOR);
        }

        private static long readLong(Queue<string> queue) {
            return long.Parse(queue.Dequeue());
        }
        private static int readInt(Queue<string> queue) {
            return int.Parse(queue.Dequeue());
        }

        internal void Export(StringBuilder builder) {
            addLong(builder,elapsedTime);
            addLong(builder,totalTime);

            addInt(builder,mouseX);
            addInt(builder,mouseY);

            addInt(builder,scrollX);
            addInt(builder,scrollY);

            addInt(builder,xButton1);
            addInt(builder,xButton2);

            addInt(builder,leftButton);
            addInt(builder,middleButton);
            addInt(builder,rightButton);

            if(pressedKeys.Length > 0) {
                builder.Append(string.Join(SEPERATOR,pressedKeys));
            } else {
                builder.Remove(builder.Length - 1,1);
            }

            builder.Append(Environment.NewLine);
        }
        internal SerialInputFrame(string data) {
            var values = new Queue<string>(data.Split(SEPERATOR));
            elapsedTime = readLong(values);
            totalTime = readLong(values);

            mouseX = readInt(values);
            mouseY = readInt(values);

            scrollX = readInt(values);
            scrollY = readInt(values);

            xButton1 = readInt(values);
            xButton2 = readInt(values);

            leftButton = readInt(values);
            middleButton = readInt(values);
            rightButton = readInt(values);

            var keys = new Queue<int>();
            string value;
            while(values.TryDequeue(out value)) {
                keys.Enqueue(int.Parse(value));
            }

            pressedKeys = keys.ToArray();
        }
    }
}
