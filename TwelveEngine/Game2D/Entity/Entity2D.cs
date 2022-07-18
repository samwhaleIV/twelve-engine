using System;
using TwelveEngine.EntitySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial;
using TwelveEngine.Shell.Input;

namespace TwelveEngine.Game2D.Entity {
    public abstract class Entity2D:Entity<Grid2D> {

        public Entity2D() {
            OnExport += Entity2D_OnExport;
            OnImport += Entity2D_OnImport;
        }

        private void Entity2D_OnImport(SerialFrame frame) {
            Position = frame.GetVector2();
            Size = frame.GetVector2();
            Direction = frame.GetDirection();
        }

        private void Entity2D_OnExport(SerialFrame frame) {
            frame.Set(Position);
            frame.Set(Size);
            frame.Set(Direction);
        }

        public bool IsHorizontal() => (int)Direction >= 2;
        public bool IsVertical() => (int)Direction <= 1;

        public Direction Direction { get; set; } = Direction.Down;

        public float X {
            get => GetPosition().X;
            set {
                Vector2 position = GetPosition();
                position.X = value;
                SetPosition(position);
            }
        }
        public float Y {
            get => GetPosition().Y;
            set {
                Vector2 position = GetPosition();
                position.Y = value;
                SetPosition(position);
            }
        }

        public float Width {
            get => GetSize().X;
            set {
                Vector2 size = GetPosition();
                size.X = value;
                SetSize(size);
            }
        }

        public float Height {
            get => GetSize().Y;
            set {
                Vector2 size = GetPosition();
                size.Y = value;
                SetSize(size);
            }
        }

        public float Bottom => Y + Height;
        public float Right => X + Width;

        private Vector2 _position = Vector2.Zero;
        private Vector2 _size = Vector2.One;

        protected virtual Vector2 GetPosition() {
            return _position;
        }
        protected virtual void SetPosition(Vector2 position) {
            _position = position;
        }

        protected virtual Vector2 GetSize() {
            return _size;
        }

        protected virtual void SetSize(Vector2 size) {
            _size = size;
        }

        public Vector2 Position {
            get => GetPosition();
            set => SetPosition(value);
        }

        public Vector2 Size {
            get => GetSize();
            set => SetSize(value);
        }

        protected bool IsKeyDown(Impulse impulse) {
            return Owner.Input.IsKeyDown(impulse);
        }
        protected bool IsKeyUp(Impulse impulse) {
            return Owner.Input.IsKeyUp(impulse);
        }

        protected void Draw(Texture2D texture,Rectangle? source = null) {
            var destination = GetDestination();
            var depth = GetRenderDepth(destination.Y);
            Game.SpriteBatch.Draw(texture,destination,source,Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
        }

        public float GetRenderDepth(int destinationY) {
            return 1f - Math.Max(destinationY / (float)Game.Viewport.Height,0f);
        }

        public Rectangle GetDestination() {
            var screenSpace = Owner.ScreenSpace;
            var tileSize = screenSpace.TileSize;

            Vector2 position = Position, size = Size;

            var destination = new Rectangle {
                X = (int)Math.Round((position.X - screenSpace.X) * tileSize),
                Y = (int)Math.Round((position.Y - screenSpace.Y) * tileSize),

                Width = (int)Math.Floor(size.X * tileSize),
                Height = (int)Math.Floor(size.Y * tileSize)
            };

            return destination;
        }

        public bool OnScreen() {
            var screenSpace = Owner.ScreenSpace;
            return !(
                X + Width <= screenSpace.X ||
                Y + Height <= screenSpace.Y ||
                X >= screenSpace.X + screenSpace.Width ||
                Y >= screenSpace.Y + screenSpace.Height
            );
        }

        public virtual bool Contains(Vector2 location) {
            return location.X >= X &&
                location.X < X + Width &&
                location.Y >= Y &&
                location.Y < Y + Height;
        }

        public event Action<GameTime> OnUpdate, OnRender;

        protected void Update(GameTime gameTime) => OnUpdate?.Invoke(gameTime);
        protected void Render(GameTime gameTime) => OnRender?.Invoke(gameTime);

        internal static void Update(Entity2D entity,GameTime gameTime) => entity.Update(gameTime);
        internal static void Render(Entity2D entity,GameTime gameTime) => entity.Render(gameTime);
    }
}
