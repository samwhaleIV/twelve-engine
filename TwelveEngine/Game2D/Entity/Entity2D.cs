using System;
using TwelveEngine.EntitySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Serial;
using TwelveEngine.Shell.Input;
using TwelveEngine.Game2D.Collision;

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

        private Vector2 position = Vector2.Zero;
        private Vector2 size = Vector2.One;

        public float X {
            get => position.X;
            set => position.X = value;
        }
        public float Y {
            get => position.Y;
            set => position.Y = value;
        }

        public float Width {
            get => size.X;
            set => size.X = value;
        }

        public float Height {
            get => size.Y;
            set => size.Y = value;
        }

        public Vector2 Position {
            get => position;
            set => position = value;
        }

        public Vector2 Size {
            get => size;
            set => size = value;
        }

        public Direction Direction { get; set; } = Direction.Down;

        public bool IsHorizontal() => (int)Direction >= 2;
        public bool IsVertical() => (int)Direction <= 1;

        protected bool IsKeyDown(Impulse impulse) {
            return Owner.Input.IsKeyDown(impulse);
        }
        protected bool IsKeyUp(Impulse impulse) {
            return Owner.Input.IsKeyUp(impulse);
        }

        protected bool TryInteract() {
            var interactionBox = Hitbox.GetInteractionArea(this);
            foreach(var target in Owner.Entities.GetByType<IInteract>()) {
                if(interactionBox.Collides(target.GetHitbox())) {
                    target.Interact(this);
                    return true;
                }
            }
            return false;
        }

        protected void Draw(Texture2D texture,Rectangle source) {
            var destination = GetDestination();
            var depth = GetRenderDepth(destination.Y);
            Game.SpriteBatch.Draw(texture,destination,source,Color.White,0f,Vector2.Zero,SpriteEffects.None,depth);
        }

        public float GetRenderDepth(int destinationY) {
            return 1 - Math.Max(destinationY / (float)Game.Viewport.Height,0);
        }

        public Rectangle GetDestination() {
            var screenSpace = Owner.ScreenSpace;
            var tileSize = screenSpace.TileSize;

            var destination = new Rectangle {
                X = (int)Math.Round((X - screenSpace.X) * tileSize),
                Y = (int)Math.Round((Y - screenSpace.Y) * tileSize),

                Width = (int)Math.Floor(Width * tileSize),
                Height = (int)Math.Floor(Height * tileSize)
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
