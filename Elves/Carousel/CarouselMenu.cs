using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Font;

namespace Elves.Carousel {

    public sealed class CarouselMenu:OrthoBackgroundState {

        public CarouselMenu():base("Backgrounds/glass",false) {
            ScrollingBackground = true;
            ScrollingBackgroundPeriod = Constants.AnimationTiming.CarouselBackgroundScroll;
            Camera.Orthographic = false;
            Name = "Carousel Menu";
            OnLoad += CarouselMenu_OnLoad;
            ClearColor = Color.LightGray;
            Camera.FieldOfView = 40f;
            var position = Camera.Position;
            position.Z = 2.05f;
            Camera.Position = position;
            OnUpdate += CarouselMenu_OnUpdate;
            Input.OnDirectionDown += direction => {
                if(direction == TwelveEngine.Input.Direction.Left) {
                    Move(MoveDirection.Left);
                } else if(direction == TwelveEngine.Input.Direction.Right) {
                    Move(MoveDirection.Right);
                }
            };
            OnRender += CarouselMenu_OnRender;
        }

        private void CarouselMenu_OnRender() {
            var font = Fonts.RetroFontOutlined;
            font.Begin(Game.SpriteBatch);
            Point center = Game.Viewport.Bounds.Center;
            int scale = Math.Max((int)(GetUIScale()*0.5f),1);
            center.Y = 16 * scale;
            font.DrawCentered(centerItem.DisplayName,center,scale,Color.White);
            font.End();
        }

        private CarouselItem centerItem, leftItem, rightItem, backItem;

        private Color currentColor, oldColor;

        private Color GetTintColor() => carouselRotation.Interpolate(oldColor,currentColor);

        private void CarouselMenu_OnUpdate() {
            carouselRotation.Update(Now);
            for(int i = 0;i<items.Count;i++) {
                var item = items[i];
                var start = item.OldRotationPosition;
                var end = item.RotationPosition;
                item.Position = carouselRotation.SmoothStep(positions[start],positions[end]);
            }
        }

        private readonly AnimationInterpolator carouselRotation = new(Constants.AnimationTiming.CarouselRotationDuration);

        private readonly List<CarouselItem> items = new();

        private enum MoveDirection { Left = -1, Right = 1 }

        private static RotationPosition GetLeft(RotationPosition position) => position switch {
            RotationPosition.Center => RotationPosition.Left,
            RotationPosition.Left => RotationPosition.Back,
            RotationPosition.Right => RotationPosition.Center,
            RotationPosition.Back => RotationPosition.Right,
            _ => throw new NotImplementedException()
        };

        private static RotationPosition GetRight(RotationPosition position) => position switch {
            RotationPosition.Center => RotationPosition.Right,
            RotationPosition.Left => RotationPosition.Center,
            RotationPosition.Right => RotationPosition.Back,
            RotationPosition.Back => RotationPosition.Left,
            _ => throw new NotImplementedException()
        };

        private void Move(MoveDirection direction) {
            if(!carouselRotation.IsFinished) {
                return;
            }
            carouselRotation.Reset(Now);
            bool isLeft = direction == MoveDirection.Left;
            for(int i = 0;i < items.Count;i++) {
                var item = items[i];

                var oldPosition = item.RotationPosition;
                item.OldRotationPosition = oldPosition;

                var newPosition = isLeft ? GetLeft(oldPosition) : GetRight(oldPosition);
                item.RotationPosition = newPosition;

                switch(newPosition) {
                    case RotationPosition.Center:
                        centerItem = item;
                        break;
                    case RotationPosition.Left:
                        leftItem = item;
                        break;
                    case RotationPosition.Right:
                        rightItem = item;
                        break;
                    case RotationPosition.Back:
                        backItem = item;
                        break;
                }
            }

            oldColor = currentColor;
            currentColor = centerItem.TintColor;
        }

        private CarouselItem CreateCarouselItem(int x,int y,int width,int height) {
            CarouselItem item = new(Textures.CarouselMenu,new Rectangle(x,y,width,height));
            items.Add(item);
            Entities.Add(item);
            return item;
        }

        private static readonly Dictionary<RotationPosition,Vector3> positions = new() {
            {RotationPosition.Back, new Vector3(0,0f,DepthConstants.MiddleFarthest) },
            {RotationPosition.Left, new Vector3(-0.55f,0f,DepthConstants.Middle) },
            {RotationPosition.Center, new Vector3(0f,0f,DepthConstants.MiddleClose) },
            {RotationPosition.Right, new Vector3(0.55f,0f,DepthConstants.Middle) }
        };

        private void CarouselMenu_OnLoad() {
            centerItem = CreateCarouselItem(48,0,17,47);
            centerItem.RotationPosition = RotationPosition.Center;
            centerItem.TintColor = Color.Red;
            centerItem.DisplayName = "Red Elf";

            oldColor = Color.White;
            currentColor = centerItem.TintColor;

            leftItem = CreateCarouselItem(65,0,18,50);
            leftItem.RotationPosition = RotationPosition.Left;
            leftItem.TintColor = Color.Yellow;
            leftItem.DisplayName = "Yellow Elf";

            rightItem = CreateCarouselItem(83,0,24,52);
            rightItem.RotationPosition = RotationPosition.Right;
            rightItem.TintColor = Color.Purple;
            rightItem.DisplayName = "Purple Elf";

            backItem = CreateCarouselItem(30,32,18,48);
            backItem.RotationPosition = RotationPosition.Back;
            backItem.TintColor = Color.FromNonPremultiplied(new Vector4(0.15f,0.15f,0.15f,1f));
            backItem.DisplayName = "Ninja Elf";

            Background.Scale *= 4f;
        }
    }
}
