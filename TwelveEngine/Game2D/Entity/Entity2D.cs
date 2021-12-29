using System;
using Microsoft.Xna.Framework;
using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game2D {
    public abstract class Entity2D:Entity<Grid2D> {

        private float x = 0, y = 0, width = 1, height = 1;
        private Direction direction = Direction.Down;

        protected bool IsKeyDown(Impulse impulse) {
            return Game.ImpulseHandler.IsKeyDown(impulse);
        }

        protected bool IsKeyUp(Impulse impulse) {
            return Game.ImpulseHandler.IsKeyUp(impulse);
        }

        public Hitbox GetInteractionBox() {
            return Hitbox.GetInteractionArea(this);
        }

        public Rectangle GetDestination() {
            var screenSpace = Owner.ScreenSpace;
            var tileSize = screenSpace.TileSize;

            var destination = new Rectangle {
                X = (int)Math.Round((x - screenSpace.X) * tileSize),
                Y = (int)Math.Round((y - screenSpace.Y) * tileSize),

                Width = (int)Math.Floor(width * tileSize),
                Height = (int)Math.Floor(height * tileSize)
            };

            return destination;
        }

        public bool OnScreen() {
            var screenSpace = Owner.ScreenSpace;
            return !(
                x + width <= screenSpace.X ||
                y + height <= screenSpace.Y ||
                x >= screenSpace.X + screenSpace.Width ||
                y >= screenSpace.Y + screenSpace.Height
            );
        }

        public float X {
            get => x;
            set => x = value;
        }
        public float Y {
            get => y;
            set => y = value;
        }
        public float Width {
            get => width;
            set => width = value;
        }
        public float Height {
            get => height;
            set => height = value;
        }
        public Direction Direction {
            get => direction;
            set => direction = value;
        }

        public override void Import(SerialFrame frame) {
            X = frame.GetFloat();
            Y = frame.GetFloat();
            Width = frame.GetFloat();
            Height = frame.GetFloat();
            Direction = frame.GetDirection();
        }

        public override void Export(SerialFrame frame) {
            frame.Set(X);
            frame.Set(Y);
            frame.Set(Width);
            frame.Set(Height);
            frame.Set(Direction);
        }
    }
}
