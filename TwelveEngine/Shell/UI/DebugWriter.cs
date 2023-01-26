using System;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TwelveEngine.Shell.UI {
    public sealed class DebugWriter {

        private const string NUMBER_FORMAT = "{0:0.00}";

        private readonly GameManager game;

        public DebugWriter(GameManager game) => this.game = game;
        public DebugWriter(GameState state) => game = state.Game;

        private SpriteFont Font => game.DebugFont;

        public int Padding { get; set; } = Constants.ScreenEdgePadding;

        private readonly StringBuilder writer = new();
        private readonly StringBuilder reverseBuffer = new();

        private (Vector2 Position, Corner Corner,int LineHeight,Color Color) renderState;

        private void DrawString() {
            var position = GetPosition();
            bool rightSide = renderState.Corner == Corner.BottomRight || renderState.Corner == Corner.TopRight;
            if(rightSide) {
                /* RTL text rendering draws text in FIFO, so we have to reverse it to LIFO with a buffer */
                for(int i = writer.Length-1;i>=0;i--) {
                    reverseBuffer.Append(writer[i]);
                }
                game.SpriteBatch.DrawString(Font,reverseBuffer,position,renderState.Color,0f,Vector2.Zero,Vector2.One,SpriteEffects.None,1f,true);
                reverseBuffer.Clear();
            } else {
                game.SpriteBatch.DrawString(Font,writer,position,renderState.Color,0f,Vector2.Zero,Vector2.One,SpriteEffects.None,1f,false);
            }
            writer.Clear();
        }

        private void Begin(Corner corner,Color? color = null) {
            /* The positioning here is pretty hacky, but it is the underlying problem of the default SpriteFont implementation */
            var padding = Padding;
            float x = padding, y = padding / 2 + 1;

            var viewport = game.Viewport;
            var lineHeight = Font.LineSpacing;

            switch(corner) {
                case Corner.TopRight:
                    /* For example, when drawing RTL text, positioning is off by 1 pixel. And who could possibly know why? */
                    x = viewport.Width - padding + 1;
                    break;
                case Corner.BottomLeft:
                    y = viewport.Height - lineHeight;
                    break;
                case Corner.BottomRight:
                    x = viewport.Width - padding + 1;
                    y = viewport.Height - lineHeight;
                    break;
            }

            renderState = (new Vector2(x,y), corner, lineHeight, color ?? Color.White);
        }

        private Vector2 GetPosition() {
            var corner = renderState.Corner;

            var difference = renderState.LineHeight;
            
            if(corner == Corner.BottomLeft || corner == Corner.BottomRight) {
                difference = -difference;
            }

            var position = renderState.Position;

            renderState.Position.Y += difference;

            return position;
        }

        private void WriteLabel(string label) {
            if(label == null) {
                return;
            }
            writer.Append(label);
            writer.Append(": ");
        }

        public void SetColor(Color color) {
            renderState.Color = color;
        }
        public void ResetColor() {
            renderState.Color = Color.White;
        }

        public void WriteTimeMS(TimeSpan time,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(NUMBER_FORMAT,time.TotalMilliseconds);
            writer.Append("ms");
            DrawString();
        }

        public void WriteFPS(double fps) {
            WriteLabel("FPS");
            writer.AppendFormat("{0:0}",fps);
            DrawString();
        }

        public void Write(string text,string label = null) {
            WriteLabel(label);
            writer.Append(text);
            DrawString();
        }

        public void Write(bool value,string label = null) {
            WriteLabel(label);
            writer.Append(value ? "True" : "False");
            DrawString();
        }

        public void Write(int value,string label = null) {
            WriteLabel(label);
            writer.Append(value);
            DrawString();
        }

        public void WriteRange(int value,int max,string label = null) {
            WriteLabel(label);
            writer.Append(value);
            writer.Append(" / ");
            writer.Append(max);
            DrawString();
        }

        public void WriteRange(float value,float max,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(NUMBER_FORMAT,value);
            writer.Append(" / ");
            writer.AppendFormat(NUMBER_FORMAT,max);
            DrawString();
        }

        public void WriteRange(double value,double max,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(NUMBER_FORMAT,value);
            writer.Append(" / ");
            writer.AppendFormat(NUMBER_FORMAT,max);
            DrawString();
        }

        public void Write(float value,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(NUMBER_FORMAT,value);
            DrawString();
        }

        public void Write(double value,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(NUMBER_FORMAT,value);
            DrawString();
        }

        public void Write(TimeSpan time,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(Constants.TimeSpanFormat,time);
            DrawString();
        }

        public void Write(Point point,string label = null) {
            WriteLabel(label);
            writer.Append(point.X);
            writer.Append(", ");
            writer.Append(point.Y);
            DrawString();
        }

        public void Write(Vector2 vector,string label = null) {
            WriteLabel(label);
            writer.AppendFormat(NUMBER_FORMAT,vector.X);
            writer.Append(", ");
            writer.AppendFormat(NUMBER_FORMAT,vector.Y);
            DrawString();
        }

        public void Write(Vector3 vector,string label = null) {
            WriteLabel(label);
            writer.Append("X ");
            writer.AppendFormat(NUMBER_FORMAT,vector.X);
            writer.Append("  Y ");
            writer.AppendFormat(NUMBER_FORMAT,vector.Y);
            writer.Append("  Z ");
            writer.AppendFormat(NUMBER_FORMAT,vector.Z);
            DrawString();
        }

        public void WriteXY(float x,float y,string xLabel,string yLabel) {
            if(xLabel != null) {
                writer.Append(xLabel);
                writer.Append(' ');
            }
            writer.AppendFormat(NUMBER_FORMAT,x);
            writer.Append("  ");
            if(yLabel != null) {
                writer.Append(yLabel);
                writer.Append(' ');
            }
            writer.AppendFormat(NUMBER_FORMAT,y);
            DrawString();
        }

        public void ToTopLeft(Color? color = null) {
            Begin(Corner.TopLeft,color);
        }

        public void ToTopRight(Color? color = null) {
            Begin(Corner.TopRight,color);
        }

        public void ToBottomLeft(Color? color = null) {
            Begin(Corner.BottomLeft,color);
        }

        public void ToBottomRight(Color? color = null) {
            Begin(Corner.BottomRight,color);
        }
    }
}
