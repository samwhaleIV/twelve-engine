using Elves.UI.Font;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Text;

namespace Elves.UI.Battle {

    public enum OffscreenDirection { Left, Right }
    public enum ButtonMoveDirection { Away, Back }

    public sealed class ActionButton:Button {

        public ActionButton() {
            Texture = UITextures.Panel;
            TextureSource = new Rectangle(0,16,32,16);
        }

        private const float MOVEMENT_DURATION = 200;

        private static readonly TimeSpan MovementDuration = TimeSpan.FromMilliseconds(MOVEMENT_DURATION);

        public readonly StringBuilder Label = new StringBuilder();

        public OffscreenDirection OffscreenDirection { get; set; } = OffscreenDirection.Left;

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

        private Color GetButtonColor() {
            if(Pressed) {
                return UIColors.PressedColor;
            } else if(Selected) {
                return UIColors.SelectColor;
            }
            return Color.White;
        }

        public void Update(Rectangle viewport,TimeSpan now) {
            Rectangle area = Area;
            float offscreenX;

            if(OffscreenDirection == OffscreenDirection.Left) {
                offscreenX = viewport.Left - area.Width;
            } else {
                offscreenX = viewport.Right;
            }
            
            currentTime = now;
            float t = GetAnimationNormal();
            if(MoveDirection == ButtonMoveDirection.Back) {
                t = 1 - t;
            }
            area.X = (int)((1 - t) * area.X + t * offscreenX);
            Area = area;
        }

        public void DrawText(UVSpriteFont spriteFont,int scale) {
            spriteFont.DrawCentered(Label,Area.Center,scale,Color.White);
        }

        public override void Draw(SpriteBatch spriteBatch,Color? color = null) {
            if(IsOffscreen) {
                return;
            }
            Color tint;
            if(MoveDirection == ButtonMoveDirection.Away && moveAwayWhileActive) {
                tint = UIColors.PressedColor;
            } else {
                tint = GetButtonColor();
            }
            spriteBatch.Draw(Texture,Area,TextureSource,tint);
        }
    }
}
