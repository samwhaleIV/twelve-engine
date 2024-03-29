﻿using Microsoft.Xna.Framework;
using TwelveEngine;

namespace Elves.Scenes.Battle.UI {
    public readonly struct ButtonRenderData {

        public ButtonRenderData(FloatRectangle viewport,float width,float height,float centerX,float centerY,float halfMargin) {
            Viewport = viewport;
            Width = width;
            Height = height;
            CenterX = centerX;
            CenterY = centerY;
            HalfMargin = halfMargin;
        }

        public readonly FloatRectangle Viewport;

        public readonly float Width;
        public readonly float Height;

        public readonly float CenterX;
        public readonly float CenterY;

        public readonly float HalfMargin;

        public readonly float HalfWidth => Width * 0.5f;
        public readonly float HalfHeight => Height * 0.5f;

        public FloatRectangle GetPosition(ButtonState buttonState) {

            Vector2 position = buttonState.Position switch {
                ButtonPosition.CenterLeft => new Vector2(        CenterX - HalfMargin - Width,    CenterY - HalfHeight),
                ButtonPosition.CenterRight => new Vector2(       CenterX + HalfMargin,            CenterY - HalfHeight),

                ButtonPosition.CenterBottom => new Vector2(      CenterX - HalfWidth,             CenterY + HalfMargin),

                ButtonPosition.TopLeft => new Vector2(           CenterX - HalfMargin - Width,    CenterY - HalfMargin - Height),
                ButtonPosition.TopRight => new Vector2(          CenterX + HalfMargin,            CenterY - HalfMargin - Height),

                ButtonPosition.BottomLeft => new Vector2(        CenterX - HalfMargin - Width,    CenterY + HalfMargin),
                ButtonPosition.BottomRight => new Vector2(       CenterX + HalfMargin,            CenterY + HalfMargin),

                ButtonPosition.CenterMiddle or _ => new Vector2( CenterX - HalfWidth,             CenterY - HalfHeight)
            };

            if(buttonState.OnScreen) {
                return new FloatRectangle(position.X,position.Y,Width,Height);
            }

            if(buttonState.Position == ButtonPosition.CenterMiddle || buttonState.Position == ButtonPosition.CenterBottom) {
                return new FloatRectangle(position.X,Viewport.Bottom,Width,Height);
            }

            float centerX = position.X + Width * 0.5f;
            if(centerX < CenterX) {
                return new FloatRectangle(Viewport.Left-Width,position.Y,Width,Height);
            } else {
                return new FloatRectangle(Viewport.Right,position.Y,Width,Height);
            }

        }
    }
}
