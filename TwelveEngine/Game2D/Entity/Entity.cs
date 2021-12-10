using Microsoft.Xna.Framework;
using System;

namespace TwelveEngine.Game2D {
    public abstract class Entity:ISerializable {

        protected abstract EntityType GetEntityType();
        public EntityType Type => GetEntityType();

        private string name = string.Empty;

        internal event Action<Entity,string> OnNameChanged;

        private int id = 0;
        private EntityManager owner = null;

        private GameManager game;
        private Grid2D grid;

        public EntityManager Owner {
            get => owner;
            internal set => owner = value;
        }
        public Grid2D Grid {
            get => grid;
            internal set => grid = value;
        }
        public GameManager Game {
            get => game;
            internal set => game = value;
        }

        private float x = 0, y = 0, width = 1, height = 1;

        private Direction direction = Direction.Down;

        protected event Action OnLoad, OnUnload;

        internal void Load() => OnLoad?.Invoke();
        internal void Unload() => OnUnload?.Invoke();

        public bool StateLock { get; set; } = false;

        public Hitbox GetInteractionBox() {
            return Hitbox.GetInteractionArea(this);
        }

        public Rectangle GetDestination() {
            var screenSpace = grid.ScreenSpace;
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
            var screenSpace = grid.ScreenSpace;
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
        public string Name {
            get => name;
            set {
                var oldName = name;
                name = value;
                OnNameChanged?.Invoke(this,oldName);
            }
        }
        public Direction Direction {
            get => direction;
            set => direction = value;
        }
        public int ID {
            get => id;
            internal set => id = value;
        }

        public virtual void Export(SerialFrame frame) {
            frame.Set(Name);
            frame.Set(X);
            frame.Set(Y);
            frame.Set(Width);
            frame.Set(Height);
            frame.Set(Direction);
        }
        public virtual void Import(SerialFrame frame) {
            Name = frame.GetString();
            X = frame.GetFloat();
            Y = frame.GetFloat();
            Width = frame.GetFloat();
            Height = frame.GetFloat();
            Direction = frame.GetDirection();
        }
    }
}
