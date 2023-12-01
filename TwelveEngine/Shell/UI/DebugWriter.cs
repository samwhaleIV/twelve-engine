using System.Text;

namespace TwelveEngine.Shell.UI {
    public sealed class DebugWriter {

        private const string NUMBER_FORMAT = "{0:0.00}";
        private const string NUMBER_FORMAT_LONG = "{0:0.0000}";

        internal GameStateManager Game { get; private init; }
        internal DebugWriter(GameStateManager game) => Game = game;

        private SpriteFont SpriteFont => Game.DebugFont;
        private SpriteBatch SpriteBatch => Game.SpriteBatch;
        private Viewport Viewport => Game.RenderTarget.GetViewport();

        public int Padding { get; set; } = Constants.ScreenEdgePadding;

        private readonly StringBuilder sb = new(), rtlReversalBuffer = new();

        private (Vector2 Position, Corner Corner,int LineHeight,Color Color) renderState;

        private void DrawString() {
            var position = GetPosition();
            bool rightSide = renderState.Corner == Corner.BottomRight || renderState.Corner == Corner.TopRight;
            if(rightSide) {
                /* RTL text rendering draws text in FIFO, so we have to reverse it to LIFO with a buffer */
                for(int i = sb.Length-1;i>=0;i--) {
                    rtlReversalBuffer.Append(sb[i]);
                }
                SpriteBatch.DrawString(SpriteFont,rtlReversalBuffer,position,renderState.Color,0f,Vector2.Zero,Vector2.One,SpriteEffects.None,1f,true);
                rtlReversalBuffer.Clear();
            } else {
                SpriteBatch.DrawString(SpriteFont,sb,position,renderState.Color,0f,Vector2.Zero,Vector2.One,SpriteEffects.None,1f,false);
            }
            sb.Clear();
        }

        private void Begin(Corner corner,Color? color = null) {
            /* The positioning here is pretty hacky, but it is the underlying problem of the default SpriteFont implementation */
            var padding = Padding;
            float x = padding, y = padding / 2 + 1;

            var viewport = Viewport;
            var lineHeight = SpriteFont.LineSpacing;

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
            sb.Append(label);
            sb.Append(": ");
        }

        public void SetColor(Color color) {
            renderState.Color = color;
        }
        public void ResetColor() {
            renderState.Color = Color.White;
        }

        public void WriteTimeMS(TimeSpan time,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(NUMBER_FORMAT,time.TotalMilliseconds);
            sb.Append("ms");
            DrawString();
        }

        public void WriteFPS(double fps) {
            WriteLabel("FPS");
            sb.AppendFormat("{0:0}",fps);
            DrawString();
        }

        public void Write(string text,string label = null) {
            WriteLabel(label);
            sb.Append(text);
            DrawString();
        }

        public void Write(bool value,string label = null) {
            WriteLabel(label);
            sb.Append(value ? "True" : "False");
            DrawString();
        }

        public void Write(int value,string label = null) {
            WriteLabel(label);
            sb.Append(value);
            DrawString();
        }

        public void WriteRange(int value,int max,string label = null) {
            WriteLabel(label);
            sb.Append(value);
            sb.Append(" / ");
            sb.Append(max);
            DrawString();
        }

        public void WriteRange(float value,float max,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(NUMBER_FORMAT,value);
            sb.Append(" / ");
            sb.AppendFormat(NUMBER_FORMAT,max);
            DrawString();
        }

        public void WriteRange(double value,double max,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(NUMBER_FORMAT,value);
            sb.Append(" / ");
            sb.AppendFormat(NUMBER_FORMAT,max);
            DrawString();
        }

        public void Write(float value,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(NUMBER_FORMAT,value);
            DrawString();
        }

        public void Write(double value,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(NUMBER_FORMAT,value);
            DrawString();
        }

        public void Write(TimeSpan time,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(Constants.TimeSpanFormat,time);
            DrawString();
        }

        public void Write(Point point,string label = null) {
            WriteLabel(label);
            sb.Append(point.X);
            sb.Append(", ");
            sb.Append(point.Y);
            DrawString();
        }

        public void Write(Vector2 vector,string label = null) {
            WriteLabel(label);
            sb.AppendFormat(NUMBER_FORMAT_LONG,vector.X);
            sb.Append(", ");
            sb.AppendFormat(NUMBER_FORMAT_LONG,vector.Y);
            DrawString();
        }

        public void Write(Vector3 vector,string label = null) {
            WriteLabel(label);
            sb.Append("X ");
            sb.AppendFormat(NUMBER_FORMAT,vector.X);
            sb.Append("  Y ");
            sb.AppendFormat(NUMBER_FORMAT,vector.Y);
            sb.Append("  Z ");
            sb.AppendFormat(NUMBER_FORMAT,vector.Z);
            DrawString();
        }

        public void Write(FloatRectangle rectangle,string label = null) {
            WriteLabel(label);
            sb.Append("X ");
            sb.AppendFormat(NUMBER_FORMAT_LONG,rectangle.X);
            sb.Append("  Y ");
            sb.AppendFormat(NUMBER_FORMAT_LONG,rectangle.Y);
            sb.Append("  W ");
            sb.AppendFormat(NUMBER_FORMAT,rectangle.Width);
            sb.Append("  H ");
            sb.AppendFormat(NUMBER_FORMAT,rectangle.Height);
            DrawString();
        }

        public void WriteXY(float x,float y,string xLabel,string yLabel) {
            if(xLabel != null) {
                sb.Append(xLabel);
                sb.Append(' ');
            }
            sb.AppendFormat(NUMBER_FORMAT,x);
            sb.Append("  ");
            if(yLabel != null) {
                sb.Append(yLabel);
                sb.Append(' ');
            }
            sb.AppendFormat(NUMBER_FORMAT,y);
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

        private readonly FPSCounter fpsCounter = new();

        private readonly FrameTimeSmoother updateDurationSmoother = new(), renderDurationSmoother = new();

        private Action<GameStateManager,DebugWriter> _timeWriter = TimeWriters.Get(0);
        private int _timeWriterIndex = 1;

        public void CycleTimeWriter() {
            _timeWriterIndex %= TimeWriters.Count;
            _timeWriter = TimeWriters.Get(_timeWriterIndex++);
        }

        public void DrawGameTimeDebug(TimeSpan updateDuration,TimeSpan renderDuration) {
            TimeSpan now = ProxyTime.GetElapsedTime();

            ToBottomRight();

            renderDurationSmoother.Update(now,renderDuration);
            WriteTimeMS(renderDurationSmoother.Average,"R");

            updateDurationSmoother.Update(now,updateDuration);
            WriteTimeMS(updateDurationSmoother.Average,"U");

            fpsCounter.Update(now);
            WriteFPS(fpsCounter.FPS);

            ToBottomLeft();
            _timeWriter(Game,this);
        }
    }
}
