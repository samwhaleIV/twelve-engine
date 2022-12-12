using Microsoft.Xna.Framework;

namespace Elves.UI.Battle {
    public readonly struct ButtonRenderData {

        public ButtonRenderData(Rectangle viewport,int width,int height,int centerX,int centerY,int halfMargin) {
            Viewport = viewport;
            Width = width;
            Height = height;
            CenterX = centerX;
            CenterY = centerY;
            HalfMargin = halfMargin;
        }

        public readonly Rectangle Viewport;

        public readonly int Width;
        public readonly int Height;

        public readonly int CenterX;
        public readonly int CenterY;

        public readonly int HalfMargin;

        public readonly int HalfWidth => Width / 2;
        public readonly int HalfHeight => Height / 2;

        public Rectangle GetPosition(ButtonState buttonState) {

            Point position = buttonState.Position switch {
                ButtonPosition.CenterLeft => new Point(        CenterX - HalfMargin - Width,    CenterY - HalfHeight),
                ButtonPosition.CenterRight => new Point(       CenterX + HalfMargin,            CenterY - HalfHeight),

                ButtonPosition.CenterBottom => new Point(      CenterX - HalfWidth,             CenterY + HalfMargin),

                ButtonPosition.TopLeft => new Point(           CenterX - HalfMargin - Width,    CenterY - HalfMargin - Height),
                ButtonPosition.TopRight => new Point(          CenterX + HalfMargin,            CenterY - HalfMargin - Height),

                ButtonPosition.BottomLeft => new Point(        CenterX - HalfMargin - Width,    CenterY + HalfMargin),
                ButtonPosition.BottomRight => new Point(       CenterX + HalfMargin,            CenterY + HalfMargin),

                ButtonPosition.CenterMiddle or _ => new Point( CenterX - HalfWidth,             CenterY - HalfHeight)
            };

            if(buttonState.OnScreen) {
                return new Rectangle(position.X,position.Y,Width,Height);
            }

            if(buttonState.Position == ButtonPosition.CenterMiddle) {
                return new Rectangle(position.X,Viewport.Bottom,Width,Height);
            }

            int centerX = position.X + Width / 2;
            if(centerX < CenterX) {
                return new Rectangle(Viewport.Left-Width,position.Y,Width,Height);
            } else {
                return new Rectangle(Viewport.Right,position.Y,Width,Height);
            }

        }
    }
}
