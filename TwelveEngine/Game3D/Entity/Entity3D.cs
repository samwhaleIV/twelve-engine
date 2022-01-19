using Microsoft.Xna.Framework;
using System;
using TwelveEngine.Serial;
using TwelveEngine.EntitySystem;

namespace TwelveEngine.Game3D.Entity {
    public abstract class Entity3D:Entity<World> {

        public Entity3D() {
            OnImport += Entity3D_OnImport;
            OnExport += Entity3D_OnExport;
        }

        private void Entity3D_OnExport(SerialFrame frame) {
            frame.Set(Position);
            frame.Set(Rotation);
        }

        private void Entity3D_OnImport(SerialFrame frame) {
            Position = frame.GetVector3();
            Rotation = frame.GetVector3();
        }

        protected bool PositionValid { get; set; } = false;
        protected bool RotationValid { get; set; } = false;

        private Vector3 _position, _rotation;

        public Vector3 Position {
            get => _position;
            set {
                _position = value;
                PositionValid = false;
            }
        }

        public Vector3 Rotation {
            get => _rotation;
            set {
                _rotation = value;
                RotationValid = false;
            }
        }

        public event Action<GameTime> OnUpdate, OnRender;

        public void Update(GameTime gameTime) => OnUpdate?.Invoke(gameTime);
        public void Render(GameTime gameTime) => OnRender?.Invoke(gameTime);

        public static void Update(Entity3D entity,GameTime gameTime) => entity.Update(gameTime);
        public static void Render(Entity3D entity,GameTime gameTime) => entity.Render(gameTime);
    }
}
