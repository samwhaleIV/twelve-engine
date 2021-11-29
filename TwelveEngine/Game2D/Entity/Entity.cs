namespace TwelveEngine.Game2D {
    public abstract class Entity:ISerializable {

        private const float INTERACTION_BOX_SIZE = 0.25f;

        public string FactoryID = null;
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

        public virtual void Load() {}
        public virtual void Unload() {}

        public virtual void Export(SerialFrame frame) {
            frame.Set("X",X);
            frame.Set("Y",Y);
            frame.Set("Width",Width);
            frame.Set("Height",Height);
            frame.Set("Direction",(int)Direction);

        }
        public virtual void Import(SerialFrame frame) {
            X = frame.GetFloat("X");
            Y = frame.GetFloat("Y");
            Width = frame.GetFloat("Width");
            Height = frame.GetFloat("Height");
            Direction = (Direction)frame.GetInt("Direction");
        }
    }
}
