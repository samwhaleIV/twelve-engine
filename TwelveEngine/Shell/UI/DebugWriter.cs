using System;
using System.Globalization;
using System.Runtime.Serialization;
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

        private readonly StringBuilder writer = new StringBuilder();

        private (Vector2 Position, Corner Corner,int LineHeight,Color Color) renderState;

        private static int GetStartXRight(Viewport viewport,int padding) {
            return viewport.Width - padding;
        }

        private static int GetStartYBottom(Viewport viewport,int padding,int lineHeight) {
            return viewport.Height - lineHeight;
        }

        private void Begin(Corner corner,Vector2? offset = null,Color? color = null) {
            var padding = Padding;
            float x = padding, y = padding;

            var viewport = game.Viewport;
            var lineHeight = Font.LineSpacing;

            switch(corner) {
                case Corner.TopRight:
                    x = GetStartXRight(viewport,padding);
                    break;
                case Corner.BottomLeft:
                    y = GetStartYBottom(viewport,padding,lineHeight);
                    break;
                case Corner.BottomRight:
                    x = GetStartXRight(viewport,padding);
                    y = GetStartYBottom(viewport,padding,lineHeight);
                    break;
            }

            var position = new Vector2(x,y);
            if(offset.HasValue) {
                position += offset.Value;
            }
            renderState = (position, corner, lineHeight, color ?? Color.White);
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

        private void DrawString() {
            var position = GetPosition();
            game.SpriteBatch.DrawString(Font,writer,position,renderState.Color);
            writer.Clear();
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
            writer.AppendFormat(NUMBER_FORMAT,fps);
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
            writer.Append(value);
            DrawString();
        }

        public void Write(double value,string label = null) {
            WriteLabel(label);
            writer.Append(value);
            DrawString();
        }

        public void Write(TimeSpan time,string label = null) {
            WriteLabel(label);
            writer.AppendFormat("{0:hh\\:mm\\:ss\\.ff}",time);
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

        public void ToTopLeft(Color? color = null) => Begin(Corner.TopLeft,Vector2.Zero,color);
        public void ToTopRight(int maxWidth = 0,Color? color = null) => Begin(Corner.TopRight,new Vector2(-maxWidth,0f),color);

        public void ToBottomLeft(Color? color = null) => Begin(Corner.BottomLeft,Vector2.Zero,color);
        public void ToBottomRight(int maxWidth = 0,Color? color = null) => Begin(Corner.BottomRight,new Vector2(-maxWidth,0f),color);
    }
}
