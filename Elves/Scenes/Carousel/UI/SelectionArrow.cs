using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using TwelveEngine;
using TwelveEngine.UI.Book;

namespace Elves.Scenes.Carousel.UI {
    public sealed class SelectionArrow:SpriteElement {

        public SelectionArrow() {
            Texture = Program.Textures.Carousel;
            Offset = new Vector2(-0.5f);
            PositionMode = CoordinateMode.Relative;
            Position = new(0.5f,0.5f);
        }

        public Direction Direction { get; set; } = Direction.Left;

        private readonly Dictionary<Direction,Rectangle> textureSources = new() {
            {Direction.None, Rectangle.Empty},
            {Direction.Left, new(34,55,10,8)},
            {Direction.Right, new(23,55,10,8)},
            {Direction.Up, new(8,10,14,54)},
            {Direction.Down, new(5,54,8,10)} 
        };

        private Rectangle GetTextureSource() => textureSources[Direction];

        private BookElement oldParent;
        private float oldParentScale;

        public void Update(BookElement parent,float pixelSize,TimeSpan now) {
            if(parent != oldParent || (parent is not null && parent.Scale != oldParentScale)) {
                KeyAnimation(now);
            }
            oldParentScale = parent?.Scale ?? float.NaN;
            oldParent = parent;

            if(parent is null || Direction == Direction.None) {
                Scale = 0;
                return;
            }

            FloatRectangle parentArea = parent.GetStaticComputedArea();

            TextureSource = GetTextureSource();
            SizeMode = CoordinateMode.Absolute;

            Scale = 1;
            Size = TextureSource.Size.ToVector2() * pixelSize;

            float gapOffset = pixelSize * 6;

            PositionMode = CoordinateMode.Absolute;
            Position = Direction switch {
                Direction.Left => new(parentArea.Right + gapOffset,parentArea.CenterY),
                Direction.Right => new(parentArea.Left - gapOffset,parentArea.CenterY),
                Direction.Up => new(parentArea.CenterX,parentArea.Bottom + gapOffset),
                Direction.Down => new(parentArea.CenterX,parentArea.Top - gapOffset),
                Direction.None => Vector2.Zero,
                _ => Vector2.Zero
            };
        }
    }
}
