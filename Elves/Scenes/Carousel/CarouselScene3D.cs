using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.Font;
using TwelveEngine.Effects;
using Elves.ElfData;

namespace Elves.Scenes.Carousel {

    public abstract class CarouselScene3D:Scene3D {

        private const float BACKGROUND_SCALE = 3f;
        private const float FIELD_OF_VIEW = 40f;
        private const float CAMERA_Z = 2.05f;

        private const float CENTER_OFFSET = 0.55f;
        private const float FAR_DEPTH = Constants.Depth.MiddleFarthest;
        private const float MIDDLE_DEPTH = Constants.Depth.Middle;
        private const float CLOSE_DEPTH = Constants.Depth.MiddleClose;

        private readonly int highestCompletedBattle = GetHighestCompletedBattle();

        private CarouselItem centerItem, leftItem, rightItem;

        private Color currentColor, oldColor;
        public Color GetTintColor() => carouselRotation.Interpolate(oldColor,currentColor);

        private readonly Interpolator carouselRotation = new(Constants.AnimationTiming.CarouselRotationDuration);

        private readonly List<CarouselItem> items = new();

        private static readonly Dictionary<RotationPosition,Vector3> positions = new() {
            {RotationPosition.Back, new Vector3(0,0f,FAR_DEPTH) },
            {RotationPosition.Left, new Vector3(-CENTER_OFFSET,0f,MIDDLE_DEPTH) },
            {RotationPosition.Center, new Vector3(0f,0f,CLOSE_DEPTH) },
            {RotationPosition.Right, new Vector3(CENTER_OFFSET,0f,MIDDLE_DEPTH) }
        };

        public CarouselScene3D() {
            Name = "Carousel Menu";
            OnLoad += CarouselMenu_OnLoad;
            Camera.FieldOfView = FIELD_OF_VIEW;
            var position = Camera.Position;
            position.Z = CAMERA_Z;
            Camera.Position = position;
        }

        private readonly ScrollingBackground background = new() {
            ScrollTime = Constants.AnimationTiming.CarouselBackgroundScroll,
            Scale = BACKGROUND_SCALE
        };

        private void CreateBackground() {
            background.Texture = Program.Textures.Mountains;
            background.Load(Content);
        }

        protected void PreRenderCarouselScene() {
            background.Update(Now);
            background.Render(SpriteBatch,Viewport);
        }

        private void CarouselMenu_OnLoad() {
            CreateItems();
            UpdateIndex(GetStartIndex(),MoveDirection.None);
            ApplyDefaultRotations();
            CreateBackground();
        }

        protected void RenderCarouselScene() {
            var font = Fonts.RetroFontOutlined;
            font.Begin(SpriteBatch);
            Point center = Viewport.Bounds.Center;
            int scale = Math.Max((int)(GetUIScale()*0.5f),1);
            center.Y = 16 * scale;
            font.DrawCentered(centerItem.DisplayName,center,scale,Color.White);
            font.End();
        }

        protected void UpdateCarouselItems() {
            carouselRotation.Update(Now);
            for(int i = 0;i<items.Count;i++) {

                CarouselItem item = items[i];
                if(!item.IsVisible) {
                    continue;
                }

                RotationPosition start = item.OldRotationPosition, end = item.RotationPosition;
                item.Position = carouselRotation.SmoothStep(positions[start],positions[end]);

                float t = carouselRotation.Value;
                if(item.RotationPosition == RotationPosition.Back) {
                    item.Alpha = 1f - t;
                    if(item.Alpha <= 0f) {
                        item.IsVisible = false;
                    }
                } else if(item.OldRotationPosition == RotationPosition.Back) {
                    item.Alpha = t;
                } else {
                    item.Alpha = 1f;
                }
            }
        }

        public void RotateCarousel(MoveDirection direction) {
            if(!carouselRotation.IsFinished) {
                return;
            }
            int index = centerItem.Index;
            switch(direction) {
                default:
                case MoveDirection.None:
                    return;
                case MoveDirection.Left:
                    if(index == 0) {
                        return;
                    }
                    break;
                case MoveDirection.Right:
                    int? nextIndex = GetRightIndex(index);
                    if(!nextIndex.HasValue || items[nextIndex.Value].IsLocked) {
                        return;
                    }
                    break;
            }
            carouselRotation.Reset(Now);
            UpdateIndex(index+(int)direction,direction);
            oldColor = currentColor;
            currentColor = centerItem.TintColor;
        }

        public void StartBattle() {
            if(!carouselRotation.IsFinished) {
                return;
            }
            EndScene(ExitValue.Get(centerItem.ElfID));
        }

        private static int GetHighestCompletedBattle() {
            Program.Save.TryGetInt(SaveKeys.HighestCompletedBattle,out int value,defaultValue: -1);
            return value;
        }

        private void CreateItems() {
            foreach(var elf in ElfManifest.GetAll()) {
                int index = items.Count;
                var item = new CarouselItem(elf.Texture,elf.PreviewSource) {
                    DisplayName = elf.Name,
                    TintColor = elf.Color,
                    Index = index,
                    IsLocked = index > highestCompletedBattle + 1,
                    ElfID = elf.ID
                };
                items.Add(item);
                Entities.Add(item);
            }
        }

        private int GetStartIndex() {
            return highestCompletedBattle + 1;
        }

        private int? GetRightIndex(int index) {
            index += 1;
            if(index >= items.Count) {
                return null;
            }
            return index;
        }

        private static int? GetLeftIndex(int index) {
            index -= 1;
            if(index < 0) {
                return null;
            }
            return index;
        }

        private void UpdateIndex(int index,MoveDirection direction) {
            int minIndex = 0, maxIndex = items.Count - 1;

            if(index < minIndex) {
                index = minIndex;
            } else if(index > maxIndex) {
                index = maxIndex;
            }

            foreach(CarouselItem item in items) {
                item.IsVisible = false;
            }

            centerItem = items[index];

            int? leftIndex = GetLeftIndex(index), rightIndex = GetRightIndex(index);

            CarouselItem oldLeft = leftItem, oldRight = rightItem;

            leftItem = leftIndex.HasValue ? items[leftIndex.Value] : null;
            rightItem = rightIndex.HasValue ? items[rightIndex.Value] : null;

            if(leftItem is not null) {
                leftItem.IsVisible = true;
            }

            if(rightItem is not null) {
                rightItem.IsVisible = true;
            }

            centerItem.IsVisible = true;

            switch(direction) {
                case MoveDirection.None:
                    ApplyDefaultRotations();
                    break;
                case MoveDirection.Left:
                    ApplyRightRotations();
                    if(oldRight is not null) {
                        oldRight.IsVisible = true;
                        oldRight.RotationPosition = RotationPosition.Back;
                        oldRight.OldRotationPosition = RotationPosition.Right;
                    }
                    break;
                case MoveDirection.Right:
                    ApplyLeftRotations();
                    if(oldLeft is not null) {
                        oldLeft.IsVisible = true;
                        oldLeft.RotationPosition = RotationPosition.Back;
                        oldLeft.OldRotationPosition = RotationPosition.Left;
                    }
                    break;
            }
        }

        private void ApplyDefaultRotations() {
            if(leftItem is not null) {
                leftItem.OldRotationPosition = RotationPosition.Left;
                leftItem.RotationPosition = RotationPosition.Left;
            }
            if(rightItem is not null) {
                rightItem.OldRotationPosition = RotationPosition.Right;
                rightItem.RotationPosition = RotationPosition.Right;
            }
            centerItem.OldRotationPosition = RotationPosition.Center;
            centerItem.RotationPosition = RotationPosition.Center;
        }

        private void ApplyRightRotations() {
            if(leftItem is not null) {
                leftItem.OldRotationPosition = RotationPosition.Back;
                leftItem.RotationPosition = RotationPosition.Left;
            }
            if(rightItem is not null) {
                rightItem.OldRotationPosition = RotationPosition.Center;
                rightItem.RotationPosition = RotationPosition.Right;
            }
            centerItem.OldRotationPosition = RotationPosition.Left;
            centerItem.RotationPosition = RotationPosition.Center;
        }

        private void ApplyLeftRotations() {
            if(leftItem is not null) {
                leftItem.OldRotationPosition = RotationPosition.Center;
                leftItem.RotationPosition = RotationPosition.Left;
            }
            if(rightItem is not null) {
                rightItem.OldRotationPosition = RotationPosition.Back;
                rightItem.RotationPosition = RotationPosition.Right;
            }
            centerItem.OldRotationPosition = RotationPosition.Right;
            centerItem.RotationPosition = RotationPosition.Center;
        }
    }
}
