using System.Diagnostics;

namespace TwelveEngine.Game2D {
    public abstract class Entity:ISerializable {

        private const float INTERACTION_BOX_SIZE = 0.25f;

        protected abstract EntityType GetEntityType();
        public EntityType Type => GetEntityType();

        public Entity() {
            if(EntityFactory.ContainsType(Type)) {
                return;
            }
            var trueType = GetType();
            EntityFactory.SetType(Type,trueType);
        }

        public string Name = string.Empty;

        public GameManager Game;
        public Grid2D Grid;

        public long ID = 0;
        public EntityManager Owner = null;

        public float X = 0;
        public float Y = 0;
        public float Width = 1;
        public float Height = 1;

        private Direction direction = Direction.Down;
        public Direction Direction {
            get => direction;
            set => direction = value;
        }

        public Hitbox GetInteractionBox() {
            var hitbox = new Hitbox() {
                Width = INTERACTION_BOX_SIZE,
                Height = INTERACTION_BOX_SIZE
            };
            var direction = Direction;
            if(direction == Direction.Left || direction == Direction.Right) {
                hitbox.Y = (Y + Height / 2) - hitbox.Height / 2;
                if(direction == Direction.Left) {
                    hitbox.X = X - hitbox.Width;
                } else {
                    hitbox.X = X + Width;
                }
            } else {
                hitbox.X = (X + Width / 2) - hitbox.Width / 2;
                if(direction == Direction.Up) {
                    hitbox.Y = Y - hitbox.Height;
                } else {
                    hitbox.Y = Y + Height;
                }
            }
            return hitbox;
        }

        public virtual void Load() {
            Debug.WriteLine($"Load {Type} entity, ID {ID}");
        }
        public virtual void Unload() {
            Debug.WriteLine($"Unload {Type} entity, ID {ID}");
        }

        public bool StateLock { get; set; } = false;

        public virtual void Export(SerialFrame frame) {
            frame.Set(Name);
            frame.Set(X);
            frame.Set(Y);
            frame.Set(Width);
            frame.Set(Height);
            frame.Set((int)Direction);
        }
        public virtual void Import(SerialFrame frame) {
            Name = frame.GetString();
            X = frame.GetFloat();
            Y = frame.GetFloat();
            Width = frame.GetFloat();
            Height = frame.GetFloat();
            Direction = (Direction)frame.GetInt();
        }
    }
}
