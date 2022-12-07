using TwelveEngine.EntitySystem;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TwelveEngine.Shell.Input;

namespace TwelveEngine.Game2D.Entity {
    public abstract class Entity2D:Entity<Grid2D> {

        public bool OnScreen() => Owner.OnScreen(this);

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

        protected void Draw(Texture2D texture,Rectangle? source = null,SpriteEffects spriteEffects = SpriteEffects.None) {
            if(source == null) {
                source = texture.Bounds;
            }
            var destination = Owner.GetDestination(Position,Size,source.Value.Size);
            float depth = Owner.GetRenderDepth(destination.Location.Y);
            Game.SpriteBatch.Draw(texture,destination.Location,source,Color.White,0f,Vector2.Zero,destination.Size,spriteEffects,depth);
        }

        public virtual bool Contains(Vector2 location) {
            return location.X >= X &&
                location.X < X + Width &&
                location.Y >= Y &&
                location.Y < Y + Height;
        }
    }
}
