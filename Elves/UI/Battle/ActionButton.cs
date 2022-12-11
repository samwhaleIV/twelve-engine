using Elves.UI.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace Elves.UI.Battle {

    public enum ButtonPosition { TopLeft, TopRight, BottomLeft, BottomRight }
    public enum ButtonMoveDirection { Away, Back }

    public sealed class ActionButton:Button {

        private const float MOVEMENT_DURATION = 200;

        private const float PRESSED_DARKENING = 0.25f;
        private const float COLOR_INTENSITY = 0.9f;

        private static readonly Color PressedColor = Color.Lerp(Color.Black,Color.White,PRESSED_DARKENING);

        private static readonly TimeSpan MovementDuration = TimeSpan.FromMilliseconds(MOVEMENT_DURATION);

        public readonly StringBuilder Label = new StringBuilder();

        public ButtonPosition Position { get; set; }

        public ButtonMoveDirection MoveDirection { get; set; } = ButtonMoveDirection.Back;
        private TimeSpan MoveStart = -MovementDuration;

        private TimeSpan currentTime = TimeSpan.Zero;

        private float GetAnimationNormal() {
            var value = (currentTime - MoveStart) / MovementDuration;
            if(value < 0) {
                return 0;
            } else if(value > 1) {
                return 1;
            }
            return (float)value;
        }

        public bool AnimationAtEnd => currentTime - MoveStart >= MovementDuration;

        protected override bool GetIsEnabled() {
            if(MoveDirection == ButtonMoveDirection.Back && AnimationAtEnd) {
                return true;
            }
            return false;
        }

        private bool IsOffscreen {
            get {
                if(MoveDirection == ButtonMoveDirection.Away && AnimationAtEnd) {
                    return true;
                }
                return false;
            }
        }

        private void Move(ButtonMoveDirection direction) {
            float normal = GetAnimationNormal();
            if(normal < 1) {
                MoveStart = currentTime - MovementDuration * (1 - normal);
            } else {
                MoveStart = currentTime;
            }
            MoveDirection = direction;
        }

        public bool moveAwayWhileActive = false;

        public void MoveAway() {
            moveAwayWhileActive = Pressed;
            Move(ButtonMoveDirection.Away);
        }
        public void MoveBack() => Move(ButtonMoveDirection.Back);

        private Color GetPressedColor(Color tint) => Color.Lerp(PressedColor,tint,COLOR_INTENSITY);
        private Color GetSelectedColor(Color tint) => Color.Lerp(Color.White,tint,COLOR_INTENSITY);

        private Color GetButtonColor(Color tint) {
            if(Pressed) {
                return GetPressedColor(tint);
            } else if(Selected) {
                return GetSelectedColor(tint);
            }
            return Color.White;
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen) {
                return;
            }
            Color tint = color ?? Color.White;
            if(MoveDirection == ButtonMoveDirection.Away && moveAwayWhileActive) {
                tint = GetSelectedColor(tint);
            } else {
                tint = GetButtonColor(tint);
            }
            base.Draw(spriteBatch,tint);
        }

        public void Update(
            int width,
            int height,
            int centerX,
            int centerY,
            int minX,
            int maxX,
            int margin,
            TimeSpan now
        ) {
            float x = centerX, offscreenX;
            int y = centerY;
            switch(Position) {
                default:
                case ButtonPosition.TopLeft:
                    x -= margin + width;
                    y -= margin + height;
                    offscreenX = minX - width;
                    break;
                case ButtonPosition.TopRight:
                    x += margin;
                    y -=  margin + height;
                    offscreenX = maxX;
                    break;
                case ButtonPosition.BottomLeft:
                    x -= margin + width;
                    y += margin;
                    offscreenX = minX - width;
                    break;
                case ButtonPosition.BottomRight:
                    x += margin;
                    y += margin;
                    offscreenX = maxX;
                    break;
            }
            currentTime = now;
            float t = GetAnimationNormal();
            if(MoveDirection == ButtonMoveDirection.Back) {
                t = 1 - t;
            }
            x = (1 - t) * x + t * offscreenX;
            Area = new Rectangle((int)x,y,width,height);
        }

        public void DrawText(UVSpriteFont spriteFont,int scale) {
            spriteFont.DrawCentered(Label,Area.Center,scale,Color.White);
        }
    }
}
